using System;
using System.Reflection;
using Lamar;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using SyRF.SharedKernel.Infrastructure;
using SyRF.WebHostConfig.Common.Extensions;

namespace SyRF.BiorxivParser.Endpoint
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private readonly IConfiguration _configuration;


        public virtual void ConfigureContainer(ServiceRegistry services)
        {
            services.AddHealthChecks();
            services.AddControllers().AddControllersAsServices();
            services.ConfigureSyrfMassTransit(_configuration, cfg =>
                {
                    cfg.AddSagaStateMachines(Assembly.GetEntryAssembly());
                    cfg.AddSagas(Assembly.GetEntryAssembly());
                },
                (rabbitMqConfig, provider) => { rabbitMqConfig.UseDelayedExchangeMessageScheduler(); }
            );
            services.Configure<HealthCheckPublisherOptions>(options =>
            {
                options.Delay = TimeSpan.FromSeconds(2);
                options.Predicate = check => check.Tags.Contains("ready");
            });
            services.AddSingleton(_configuration.GetSection("MessageBusConfig")
                .Get<MessageBusConfig>());
            services.IncludeRegistry<SyrfRegistry>();
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