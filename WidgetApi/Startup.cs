using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WidgetApi.BasicAuthenticationHandler;
using WidgetApi.HMACAuthenticationHandler;

namespace WidgetApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // TODO: Get from configuration or User Secrets
            var authenticationUrl = "https://localhost:44386";
            var scope = "widgetapi";
            var key = "1F92F130-D8E5-45D6-9792-9138F29920E0";

            services
                .AddMvcCore()
                .AddAuthorization()
                .AddJsonFormatters();

            services
                .AddAuthentication("Bearer")
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = authenticationUrl;
                    options.RequireHttpsMetadata = true;
                    options.ApiName = scope;
                })
                .AddBasicAuthentication<BasicAuthenticationService>(o =>
                {
                    o.DiscoveryUrl = authenticationUrl;
                    o.Scope = scope;
                })
                .AddHMACAuthentication<HMACAuthenticationService>(o =>
                {
                    o.Key = key;
                });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}