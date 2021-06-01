using System;
using Basket.API.GrpcServices;
using Basket.API.Mapper;
using Basket.API.Repositories;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;

namespace Basket.API
{
    public class Startup
    {
        private readonly IConfiguration _config;

        public Startup(IConfiguration configuration)
        {
            _config = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper(typeof(BasketProfile));
            services.AddScoped<ICartRepo, CartRepo>();
            services.AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>(
                o => o.Address = new Uri(_config["GrpcSettings:DiscountUrl"])
            );
            services.AddScoped<DiscountGrpcService>();

            services.AddMassTransit(c =>
            {
                c.UsingRabbitMq((_, cfg) =>
                {
                    cfg.Host(_config["EventBusSettings:HostAddress"]);
                });
            });
            services.AddMassTransitHostedService();

            services.AddControllers();

            services.AddSingleton<IConnectionMultiplexer>(_ =>
            {
                var config = ConfigurationOptions.Parse(_config.GetValue<string>("ConnectionStrings:Redis"), true);
                return ConnectionMultiplexer.Connect(config);
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "Basket.API", Version = "v1"});
            });
        }
        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Basket.API v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}