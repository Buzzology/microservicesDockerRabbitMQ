using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using publisher_api.Services;
using Microsoft.OpenApi.Models;
using Serilog;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using publisher_api.HealthChecks;
using HealthChecks.UI.Client;

namespace publisher_api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddControllers()
                .AddGitHubWebHooks()
                .AddNewtonsoftJson();

            // Add swagger: https://github.com/domaindrivendev/Swashbuckle.AspNetCore
            services
                .AddSwaggerGen(c => c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" }));

            // Add DI services
            services.AddSingleton<IMessageService, MessageService>();

            // Add misc services
            services.AddGrpc();
            services.AddHealthChecks()
                .AddCheck<RabbitMqHealthCheck>(
                    "Rabbit MQ Health Check",
                    failureStatus: HealthStatus.Unhealthy,
                    tags: new[] { "ready" }
                );
            services.AddHealthChecksUI();

            // Adding health checks as singleton services: https://stackoverflow.com/a/52029647/522859
            services.AddSingleton<IHealthCheck, RabbitMqHealthCheck>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Add serilog request logging - will only log endpoints that are generated after it. Can eliminate static files by adding above
            app.UseSerilogRequestLogging();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGrpcService<WeatherService>();
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health");
                endpoints.MapHealthChecksUI();
            });

            app.UseHealthChecks("/hc", new HealthCheckOptions {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
            app.UseHealthChecksUI();
        }
    }
}
