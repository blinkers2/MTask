using Microsoft.EntityFrameworkCore;
using MTask.Data;

namespace MTask.Extensions
{
    public static class DataBaseExtensions
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddDbContext<TagDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("MTaskDb")));

            return services;
        }
    }
}
