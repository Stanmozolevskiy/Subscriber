using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;


namespace WebAppHelper
{
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
