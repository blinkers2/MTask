using Serilog;
using System.Text.Json;

namespace MTask.Middleware
{
    public class ErrorLoggingMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occured: {Message}", ex.Message);

                if (!context.Response.HasStarted)
                {
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                    var result = JsonSerializer.Serialize(new { error = "Something went wrong" });
                    await context.Response.WriteAsync(result);
                }
            }
        }
    }
}