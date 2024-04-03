using Serilog;

namespace MTask.Extensions
{
    public static class SerilogExtensions
    {
        public static WebApplicationBuilder UseSerilogConfiguration(this WebApplicationBuilder builder)
        {
            builder.Host.UseSerilog((hostingContext, services, loggerConfiguration) => loggerConfiguration
                .ReadFrom.Configuration(hostingContext.Configuration)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File("logs/myapp_.txt", rollingInterval: RollingInterval.Day));

            return builder;
        }
    }
}
