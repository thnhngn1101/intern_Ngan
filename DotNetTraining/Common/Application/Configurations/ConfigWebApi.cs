using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Application.Configurations
{
    public static class ConfigWebApi
    {
        public static void Config<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation>(IServiceCollection services, string xmlPath)
                     where TImplementation : BaseConfigureSwaggerOptions
        {
            services.AddControllers();
            services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
            });

            //Add Swagger
            ConfigSwagger.Config<TImplementation>(services, xmlPath);

            //Add API version
            ConfigApiVersion.Config(services);

            //Config URL lower case
            services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
        }
    }
}
