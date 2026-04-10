using ShopApp.Domain.Exceptions;

namespace ShopApp.Midlleware;

public class ExceptionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context); 
        }
        catch (Exception ex)
        {
            var statusCode = ex switch
            {
                NotFoundException => 404,
                DomainException => 400,
                _ => 500
            };
            
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsJsonAsync(new
            {
                error = ex.Message,
                status = statusCode
            });
        }
    }
}