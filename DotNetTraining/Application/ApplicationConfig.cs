using Application.Settings;
using Common.Application.Configurations;
using Common.Application.Settings;

namespace Application
{
    public class ApplicationConfig(IWebHostEnvironment? environment) : BaseAppConfiguration<ApplicationSetting>(environment)
    {
        protected override void ConfigBackgroundServices(IServiceCollection services)
        {
            // No background services needed for MKT
        }

        protected override void LoadAdditionalSetting(ApplicationSetting setting, IConfiguration configuration)
        {
            setting.DatabaseSetting = ConfigAppSetting.LoadToObject<DatabaseSetting>(configuration, "DatabaseSetting");
        }
    }
}
