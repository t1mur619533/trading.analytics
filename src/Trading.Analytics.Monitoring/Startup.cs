using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Coravel;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Trading.Analytics.Domain;
using Trading.Analytics.Monitoring.Services;

namespace Trading.Analytics.Monitoring
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
            var connection = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<TradingContext>(builder => builder.UseNpgsql(connection));
            services.AddControllers();
            services.AddScoped(provider =>
            {
                var httpClient = new HttpClient
                {
                    BaseAddress = new Uri(Configuration["BaseUri"]),
                };
                httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", Configuration["Token"]);
                return httpClient;
            });
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "API",
                    Description = "API",
                });
            });
            services.AddScoped<TradingSnapshotService>();
            services.AddScoped<CurrentRubPerUsdExchangeRateService>();
            services.AddScheduler();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                try
                {
                    var dbContext = serviceProvider.GetRequiredService<TradingContext>();
                    dbContext.Database.Migrate();
                }
                catch (Exception ex)
                {
                    var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred seeding the DB.");
                }

                var provider = app.ApplicationServices;
                provider.UseScheduler(scheduler =>
                {
                    scheduler.Schedule<TradingSnapshotService>()
                        .EveryThirtyMinutes()
                        .Weekday();
                }).OnError(exception =>
                {
                    var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
                    logger.LogError(exception.Message);
                });
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
                c.RoutePrefix = string.Empty;
            });

            app.UseHttpsRedirection();

            app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod());

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}