using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data
{
    public static class PrepDb
    {
        public static void PrepPlatform(IApplicationBuilder app, IWebHostEnvironment env)
        {
            Console.WriteLine("PrepPlatform");
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<AppDbContext>();
                if (context is null)
                {
                    throw new ArgumentNullException(nameof(AppDbContext));
                }
                SeedData(context, env);
            }
        }

        private static void SeedData(AppDbContext context, IWebHostEnvironment env)
        {

            Console.WriteLine("***Attempting to apply migrations.....");
            try
            {
                context.Database.Migrate();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"***Could not run migrations: {ex.Message}");
            }

            if (!context.Platforms.Any())
            {
                Console.WriteLine("Seeding Data");
                context.Platforms.AddRange(
                    new Platform
                    {
                        Name = "DotNet",
                        Publisher = "Microsoft",
                        Cost = "Free"
                    },
                    new Platform
                    {
                        Name = "SQL Server Express",
                        Publisher = "Microsoft",
                        Cost = "Free"
                    },
                    new Platform
                    {
                        Name = "Kubernetes",
                        Publisher = "Cloud Native Computing Foundation",
                        Cost = "Free"
                    });
                context.SaveChanges();
            }
            Console.WriteLine("We already have data");
        }
    }
}