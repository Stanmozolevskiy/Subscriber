using DataProviderInterfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebAppHelper;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;


namespace Sbuscriber
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .ConfigureMVC()
                .AddControllers();

            services.AddSpaStaticFiles(configuration => configuration.RootPath = "ClientApp/dist");
            services.AddHttpContextAccessor();
            services.AddHttpClient();


            services.AddScoped<IFacebookProvider, FacebookProvider.Provider>();
            services.AddScoped<IFirebaseProvider, FirebaseProvider.Provider>();
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {


            app.Use(async (context, next) =>
            {
                context.Request.EnableBuffering();
                await next();
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            if (!env.IsDevelopment())
                app.UseSpaStaticFiles();

            app.UseRouting();
            app.UseEndpoints(endpoints => endpoints.MapControllers());

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });

        }
     

        private readonly IConfiguration configuration;
    }

    public static class ConfigurationExtensions
    {
        public static IServiceCollection ConfigureMVC(this IServiceCollection services)
        {
            services
                .AddMvc(options => options.RespectBrowserAcceptHeader = true)
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                });
            return services;
        }
    }

}
