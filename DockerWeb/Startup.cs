using System;
using DockerWeb.HealthChecks;
using DockerWeb.Interfaces;
using DockerWeb.Services;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

using Microsoft.OpenApi.Models;

namespace DockerWeb
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

            var dbServiceHost = Environment.GetEnvironmentVariable("ServiceHost") ?? "localhost";
            var dbServicePort = Environment.GetEnvironmentVariable("ServicePort") ?? "5002";
            var connection = Environment.GetEnvironmentVariable("DefaultConnection") ?? Configuration.GetConnectionString("DefaultConnection");

            services.AddControllers();
            services.AddHttpClient<IDbService, DBService>(); // Register the DB Service

            services
                .AddHealthChecks()
                .AddCheck("Self", new ServiceHealthCheck("http://localhost:5003/Liveness"), HealthStatus.Unhealthy, new string[] { "Self" })
                .AddCheck("DBService", new ServiceHealthCheck($"http://{dbServiceHost}:{dbServicePort}/Liveness"), HealthStatus.Unhealthy, new string[] { "DbService" })
                .AddCheck("PostgreSQL", new PostgreSqlHealthCheck(connection), HealthStatus.Unhealthy, new string[] { "PostgreSQL" });

            services.AddHealthChecksUI();

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "DockerWebApiDemo API",
                    Description = "A simple example ASP.NET Core Web API",
                    TermsOfService = new Uri("https://example.com/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "Steve Washko",
                        Email = "StephenWashko@comcast.net"
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Use under LICX",
                        Url = new Uri("https://example.com/license"),
                    }
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.UseRouting();
            app.UseAuthorization();
            app.UseHealthChecksUI();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            const string healthCheckApiPath = "/health";

            app.UseHealthChecks(healthCheckApiPath,
            new HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            })
            .UseHealthChecksUI(options =>
            {
                options.UIPath = $"{healthCheckApiPath}-ui";
                options.ApiPath = $"{healthCheckApiPath}-api";
                options.UseRelativeApiPath = false;
                options.UseRelativeResourcesPath = false;
            });
        }
    }
}