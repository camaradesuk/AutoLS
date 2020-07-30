using System;
using GreenPipes;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Lamar;
using SyRF.AppServices.FileServices;
using SyRF.Mongo.Common;
using SyRF.SharedKernel.Infrastructure;
using SyRF.SharedKernel.Interfaces;
using SyRF.WebHostConfig.Common.Extensions;

namespace SyRF.LiteratureSearch.Endpoint
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private readonly IConfiguration _configuration;

        public void ConfigureContainer(ServiceRegistry services)
        {
            services.AddSyrfDefaultServices(_configuration);
            services.AddSyrfDataServices(_configuration);
            services.AddSyrfFileService(_configuration);
            services.AddHealthChecks();
            services.AddControllers().AddControllersAsServices();
            services.ConfigureSyrfMassTransit(_configuration, null, (cfg, provider) =>
            {
                cfg.UseCircuitBreaker(cb =>
                {
                    cb.TrackingPeriod = TimeSpan.FromMinutes(1);
                    cb.TripThreshold = 15;
                    cb.ActiveThreshold = 10;
                    cb.ResetInterval = TimeSpan.FromMinutes(5);
                });
                cfg.UseRateLimit(50, TimeSpan.FromSeconds(10));
            });
            services.Configure<HealthCheckPublisherOptions>(options =>
            {
                options.Delay = TimeSpan.FromSeconds(2);
                options.Predicate = check => check.Tags.Contains("ready");
            });

            services.IncludeRegistry<SyrfRegistry>();
            services.IncludeRegistry<MongoLamarRegistry>();
            services.For<IFileService>().Use<S3FileService>();
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            var container = app.ApplicationServices.GetService<IContainer>();
            container.AssertConfigurationIsValid();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.UseSyrfHealthChecks();
            });
        }
    }
}