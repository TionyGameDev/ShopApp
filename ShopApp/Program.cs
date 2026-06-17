using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using ShopApp.Application.CQRS.Products.Queries;
using ShopApp.Application.Interfaces;
using ShopApp.Application.Services.AuthServices;
using ShopApp.Application.Services.Kafka;
using ShopApp.Application.Services.OrderServices;
using ShopApp.Application.Services.ProductServices;
using ShopApp.Application.Transations;
using ShopApp.BackgroundServices;
using ShopApp.Hubs;
using ShopApp.Infrastructure.Data;
using ShopApp.Infrastructure.Repositores;
using ShopApp.Infrastructure.Services;
using ShopApp.Midlleware;
using ShopApp.Services;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

AddServices(builder);
AddSingletons(builder);

AddCors(builder);
AddLogger(builder);

AddScoped(builder.Services);
AddSwagger(builder);
Authorization();

var app = builder.Build();
app.MapGrpcService<ProductGrpcService>();

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

app.UseCors();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseMiddleware<ExceptionMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
app.MapControllers();

app.MapHub<OrderHub>("/hubs/orders");

var port = Environment.GetEnvironmentVariable("PORT");
if (port != null)
    app.Run($"http://+:{port}");
else
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
    builderServices.AddScoped<IUnitOfWork, UnitOfWork>();
    builder.Services.AddScoped<IOrderNotificationService, OrderNotificationService>();
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
    
    builder.Services.AddSingleton<IEventBus, KafkaEventBus>();
}

void AddServices(WebApplicationBuilder webApplication)
{
    webApplication.Services.AddDbContext<AppDbContext>(options 
        => options.UseNpgsql(webApplication.Configuration.GetConnectionString("DefaultConnection")));

    webApplication.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
    webApplication.Services.AddEndpointsApiExplorer();
    webApplication.Services.AddGrpc();
    webApplication.Services.AddSignalR();
    
    webApplication.Services.AddHostedService<OrderNotificationConsumer>();
    webApplication.Services.AddHostedService<OrderKafkaConsumer>();
    
    webApplication.Services.AddMediatR(cfg => 
        cfg.RegisterServicesFromAssembly(typeof(GetProductsQuery).Assembly));
    
}

void AddLogger(WebApplicationBuilder webApplicationBuilder1)
{
    Log.Logger = new LoggerConfiguration()
        .WriteTo.Console()
        .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
        .MinimumLevel.Information()
        .CreateLogger();

    webApplicationBuilder1.Host.UseSerilog();
}

void AddCors(WebApplicationBuilder builder2)
{
    builder2.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            policy.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
    });
}

