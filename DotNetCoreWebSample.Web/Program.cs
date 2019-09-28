using System;
using DotNetCoreWebSample.Web.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DotNetCoreWebSample.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();
            using (var serviceScope = host.Services.CreateScope())
            {
                try
                {
                    var context = serviceScope.ServiceProvider.GetService<DotnetCoreWebSampleContext>();
                    context.Database.Migrate();
                }
                catch (Exception ex)
                {
                    var logger = serviceScope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred initializing the DB.");
                }
            }
            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
