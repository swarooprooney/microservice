using Microsoft.EntityFrameworkCore;
using PlatformService.Data;

namespace PlatformService.DIExtensions
{
    public static class DatabaseExtensions
    {
        public static void AddDatabaseSettings(this IServiceCollection services, IWebHostEnvironment environment, ConfigurationManager configurationManager)
        {
            // if (environment.IsProduction())
            // {
            Console.WriteLine("***Using in SQL server***");
            services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(configurationManager.GetConnectionString("PlatformConnection")));
            // }
            // else
            // {
            //     Console.WriteLine("***Using in memory database***");
            //     services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("InMem"));
            // }
        }
    }
}