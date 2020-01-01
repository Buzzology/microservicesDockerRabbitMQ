
using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace publisher_api
{
    public class Program
    {
        public static void Main(string[] args)
        {

            CreateLogger();

            try {
                Log.Information("\r\n\r\n---------------------------------------------------------------------------------\r\n\r\n");
                Log.Information("Starting publisher API...");
                CreateHostBuilder(args).Build().Run();
                Log.Information("Started publisher API!");
                Log.Information("\r\n\r\n---------------------------------------------------------------------------------\r\n\r\n");
            }
            catch(Exception e)
            {
                Log.Fatal(e, "Host terminated unexpectedly");
            }
            finally {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>

            Host.CreateDefaultBuilder(args)

                // Added for azure key vault using eShopContainers as an example
                .ConfigureAppConfiguration((context, config) =>
                {
                    GetConfiguration(config);
                })

                // Pre-existing
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                
                .UseSerilog();


        private static void GetConfiguration(IConfigurationBuilder builder)
        {
            // Add appsetting files and override with local settings if available
            builder
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            // Retrieve config from azure key vault if it's enabled
            var config = builder.Build();
            if (config.GetValue<bool>("azure-key-vault:enabled", false))
            {
                builder.AddAzureKeyVault(
                    $"https://{config["azure-key-vault:name"]}.vault.azure.net/",
                    config["azure-key-vault:client-id"],
                    config["azure-key-vault:client-secret"]);
            }

            builder.Build();
        }


        public static void CreateLogger()
        {
            var levelSwitch = new LoggingLevelSwitch();
            
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.ControlledBy(levelSwitch)
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.Seq("http://seq:5341", apiKey: "x79sgVcILv4AiUWQjKLv", controlLevelSwitch: levelSwitch)
                .CreateLogger();
        }
    }
}
