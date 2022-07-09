using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;

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
        public static string GetRequestURL(this HttpContext context) =>
            (new UriBuilder
            {
                Scheme = context.Request.Scheme,
                Host = context.Request.Host.Value,
                Path = $"{context.Request.PathBase}{context.Request.Path}",
                Query = context.Request.QueryString.Value
            }).ToString();
    }
}
