using EventBus.Messages.Common;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Ordering.API.EventBusConsumer;
using Ordering.API.Mapping;
using Ordering.Application;
using Ordering.Infrastructure;

namespace Ordering.API
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper(typeof(OrderingProfile));
            services.AddApplicationServices();
            services.AddInfrastructureServices(_configuration);
            
            services.AddMassTransit(config =>
            {
                config.AddConsumer<BasketCheckoutConsumer>();
                config.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(_configuration["EventBusSettings:HostAddress"]);
                    cfg.ReceiveEndpoint(EventBusConstants.BasketCheckoutQueue, c =>
                    {
                        c.ConfigureConsumer<BasketCheckoutConsumer>(context);
                    });
                });
            });
            services.AddMassTransitHostedService();

            services.AddScoped<BasketCheckoutConsumer>();
            
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "Ordering.API", Version = "v1"});
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ordering.API v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}