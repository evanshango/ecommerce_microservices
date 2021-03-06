using Discount.Grpc.Mapper;
using Discount.Grpc.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Discount.Grpc.Services;

namespace Discount.Grpc
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IDiscountRepo, DiscountRepo>();
            services.AddAutoMapper(typeof(DiscountProfile));
            services.AddGrpc();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<DiscountService>();

                endpoints.MapGet("/",
                    async context =>
                    {
                        await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a " +
                                                          "gRPC client. To learn how to create a client, " +
                                                          "visit: https://go.microsoft.com/fwlink/?linkid=2086909"
                        );
                    });
            });
        }
    }
}