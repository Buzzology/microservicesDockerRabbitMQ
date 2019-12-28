
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace publisher_api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
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
                });


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
    }
}
