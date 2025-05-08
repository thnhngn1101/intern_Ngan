using System.Reflection;
using System.Text;
using Application.Settings;
using Common.Application;
using Common.Databases;
using Common.Loggers.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Application
{
    public class Startup(WebApplicationBuilder? builder, string xmlPath, Assembly assembly)
        : BaseApplication<ApplicationConfig, ApplicationSetting, SwaggerOptions>(builder, xmlPath, assembly)
    {
        protected override void GenerateSqlScripts(ApplicationSetting setting)
        {
            var path = setting.FolderGenerateSqlScript
                ?? throw new Exception("Missing setting folder to generate sql scripts");
            SqlGenerator.GetInstance(setting.DatabaseSetting)
                        .GenerateCreateTableSqlScripts(typeof(Startup), path);
        }

        protected override ApplicationConfig GetConfiguration()
        {
            return new ApplicationConfig(_builder?.Environment);
        }

        protected override void AdditionalExecute(ApplicationSetting setting)
        {
            var services = _builder.Services.BuildServiceProvider();
            ILogManager logger = services.GetService<ILogManager>()
                ?? throw new Exception("Missing logger configured");

            logger.Info("Processing additional configuration...");

            // ✅ Cấu hình JWT từ JwtTokenSetting
            var jwtConfig = _builder.Configuration.GetSection("JwtTokenSetting");

            _builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtConfig["Issuer"],
                        ValidAudience = jwtConfig["Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtConfig["SymmetricSecurityKey"]!)
                        )
                    };
                });

            _builder.Services.AddAuthorization();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<ApplicationSetting>(
                _builder.Configuration.GetSection("ApplicationSetting"));
        }
    }
}
