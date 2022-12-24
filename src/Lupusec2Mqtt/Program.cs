using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Lupusec2Mqtt.Homeassistant;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Lupusec2Mqtt
{
    public class Program
    {
        private static IConfiguration _configuration;
        public static int Main(string[] args)
        {
            _configuration = BuildConfiguration();

            Log.Logger = new LoggerConfiguration()
                           .ReadFrom.Configuration(_configuration)
                           .Enrich.FromLogContext()
                           .WriteTo.Debug()
                           .WriteTo.Console(
                               outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                           .CreateLogger();

            try
            {
                Log.Information("Lupusec2Mqtt is starting up...");

                CreateHostBuilder(args).Build().Run();

                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureAppConfiguration(config => config.AddHomeassistantConfig());
                    webBuilder.UseStartup<Startup>();
                });

        private static IConfiguration BuildConfiguration()
        {
            var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
            .AddEnvironmentVariables()
            .AddHomeassistantConfig(logging: true);

            return config.Build();
        }

    }
}
