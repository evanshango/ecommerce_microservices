using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace OcelotApiGw
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
            services.AddOcelot(_configuration).AddCacheManager(settings => settings.WithDictionaryHandle());
            services.AddSwaggerForOcelot(_configuration);
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
        }

        public async void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UsePathBase("/");
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseSwaggerForOcelotUI(opt =>
            {
                opt.DownstreamSwaggerEndPointBasePath = "/swagger/docs";
                opt.PathToSwaggerGenerator = "/swagger/docs";
            });

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context => { await context.Response.WriteAsync("API GATEWAY..."); });
            });

            await app.UseOcelot();
        }
    }
}