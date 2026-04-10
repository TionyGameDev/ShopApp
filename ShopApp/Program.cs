using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ShopApp.Application.Interfaces;
using ShopApp.Application.Services.AuthServices;
using ShopApp.Application.Services.OrderServices;
using ShopApp.Application.Services.ProductServices;
using ShopApp.BackgroundServices;
using ShopApp.Infrastructure.Data;
using ShopApp.Infrastructure.Repositores;
using ShopApp.Infrastructure.Services;
using ShopApp.Midlleware;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options 
    => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddEndpointsApiExplorer();

AddSingletons(builder);

AddScoped(builder.Services);
AddSwagger(builder);
Authorization();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        if (dbContext.Database.CanConnect())
        {
            Console.WriteLine("Database connected successfully!");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database connection error: {ex.Message}");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();
app.MapControllers();

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Run($"http://+:{port}");


app.Run();


void Authorization()
{
    builder.Services.AddAuthorization();
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]))
        };
    });
}

void AddScoped(IServiceCollection builderServices)
{
    builderServices.AddScoped<IOrderService, OrderService>();
    builderServices.AddScoped<IProductService, ProductService>();
    builderServices.AddScoped<IAuthService, AuthService>();
    builderServices.AddScoped<IJwtService, JwtService>();

    builderServices.AddScoped<IAuthRepository, UserRepository>();
    builderServices.AddScoped<IProductRepository, ProductRepository>();
    builderServices.AddScoped<IOrderRepository, OrderRepository>();
    builderServices.AddScoped<ICacheService, CacheService>();
    
    builderServices.AddHostedService<OrderNotificationConsumer>();
}

void AddSwagger(WebApplicationBuilder builder1)
{
    builder1.Services.AddSwaggerGen(options =>
    {
        options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
            Scheme = "Bearer",
            In = Microsoft.OpenApi.Models.ParameterLocation.Header
        });
        options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
        {
            {
                new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Reference = new Microsoft.OpenApi.Models.OpenApiReference
                    {
                        Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
    });
}

void AddSingletons(WebApplicationBuilder webApplicationBuilder)
{
    webApplicationBuilder.Services.AddSingleton<IConnectionMultiplexer>(
        ConnectionMultiplexer.Connect(webApplicationBuilder.Configuration.GetConnectionString("Redis")!));
    
    builder.Services.AddSingleton<IMessageBus>(sp =>
    {
        var config = sp.GetRequiredService<IConfiguration>();
        return RabbitMQMessageBus.CreateAsync(config).GetAwaiter().GetResult();
    });
}

