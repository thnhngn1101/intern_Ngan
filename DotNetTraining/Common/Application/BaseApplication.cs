using System.Data;
using System.Reflection;
using Application.Settings;
using BPMaster.Common.Databases.Interfaces;
using BPMaster.Common.Services.CronJobService;
using Common.Application.Configurations;
using Common.Application.Middlewares;
using Common.Application.Settings;
using Common.Databases;
using Common.Loggers.Interfaces;
using Common.Services;
using Microsoft.Extensions.FileProviders;

namespace Common.Application
{
    public abstract class BaseApplication<Config, Setting, Swagger> where Config : BaseAppConfiguration<Setting> where Setting : BaseAppSetting where Swagger : BaseConfigureSwaggerOptions
    {
        protected readonly WebApplicationBuilder _builder;
        protected Config _appConfig;

        protected string _xmlPath;

        protected  Assembly _assembly;

        protected abstract Config GetConfiguration();

        protected abstract void GenerateSqlScripts(Setting setting);
        protected abstract void AdditionalExecute(Setting setting);

        public BaseApplication(WebApplicationBuilder? builder, string xmlPath, Assembly assembly) 
        {
            _builder = builder ?? throw new Exception("Cannot setup web application Builder");
            _xmlPath = xmlPath;
            _assembly = assembly;
            _appConfig = GetConfiguration();

        }     

        public void Start()
        {
            var services = _builder.Services;
            //1. Configure App Settings
            var appSetting = _appConfig.ConfigSettings(services);

            //2. Configure services
            _appConfig.ConfigServices(services, _assembly, appSetting);

            //3.Setup Api configuration/documentation
            _appConfig.ConfigApi<Swagger>(services, _xmlPath);

            var sp = services.BuildServiceProvider();
            var logger = sp.GetRequiredService<ILogManager>();


            //4. Setup DB Warehouse Connetion, if setting
            if (appSetting.DatabaseWarehouseSetting != null)
            {
                services.AddTransient<IWarehouseDbConnection>(sp =>
                {
                    IWarehouseDbConnection connectionWarehouse = DbConnectionFactory.GetWarehouseConnection(appSetting.DatabaseWarehouseSetting);
                    connectionWarehouse.Open();
                    return connectionWarehouse;
                    ;
                });
            }
            _builder.Services.AddSingleton(sp =>
            {
                IDbConnection connection = DbConnectionFactory.GetConnection(appSetting.DatabaseSetting);
                return new PermissionSetting(connection);
            });
            //5. Setup DB BPMaster Connection, if setting
            if (appSetting.DatabaseSetting != null)
            {
                services.AddTransient<IBPMasterDbConnection>(sp =>
                {
                    IBPMasterDbConnection connection = DbConnectionFactory.GetBPMasterConnection(appSetting.DatabaseSetting);
                    connection.Open();
                    return connection;
                });
            }
            if (appSetting.DatabaseSetting != null)
            {
                services.AddTransient<IDbConnection>(sp =>
                {
                    IDbConnection connection = DbConnectionFactory.GetConnection(appSetting.DatabaseSetting);
                    connection.Open();
                    return connection;
                });
            }
            //5. Setup DB General Connection, if setting
            if (appSetting.DatabaseSetting != null)
            {
                services.AddTransient<IDbConnection>(sp =>
                {
                    IDbConnection connection = DbConnectionFactory.GetConnection(appSetting.DatabaseSetting);
                    connection.Open();
                    return connection;
                });
            }
            if (appSetting.DatabaseSetting != null)
            {
                services.AddTransient<IDbConnection>(sp =>
                {
                    IDbConnection connection = DbConnectionFactory.GetConnection(appSetting.DatabaseSetting);
                    connection.Open();
                    return connection;
                });
            }
            //
            //services.AddTransient<ConnectionManager>(sp =>
            //{
            //    IDbConnection connectionWarehouse = DbConnectionFactory.GetConnection(appSetting.DatabaseWarehouseSetting);
            //    IDbConnection connection = DbConnectionFactory.GetConnection(appSetting.DatabaseSetting);
            //    return new ConnectionManager(connection, connectionWarehouse);
            //});
            //
            //6. Generate SQL script, if setting
            if (!string.IsNullOrEmpty(appSetting.FolderGenerateSqlScript))
            {
                if (Directory.Exists(appSetting.FolderGenerateSqlScript))
                {
                    GenerateSqlScripts(appSetting);
                }
                else
                {
                    var fullPath = Path.GetFullPath(appSetting.FolderGenerateSqlScript);
                    logger.Error($"Missing folder {fullPath} to generate sql script");
                }
            }
            //7. Add Validate Request By Token 
            services.AddHttpContextAccessor();
            services.AddSingleton<BaseAppSetting, ApplicationSetting>();
            services.AddScoped<AuthUserService<BaseAppSetting>>();
            services.AddScoped<ValidateCurrentUserFilter>();
            services.AddControllers(options =>
            {
                options.Filters.Add<ValidateCurrentUserFilter>(); // Apply the filter globally
            })
            .ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            //8. Additional setting
            AdditionalExecute(appSetting);
            //9. // Add HTTPContext
            services.AddHttpContextAccessor();
            //10 add cron
            string cronSchedule = "*/10 * * * *"; // default to every 10 minute
            services.AddSingleton(new CronJobService(services.BuildServiceProvider(), cronSchedule));
            services.AddHostedService(provider => provider.GetRequiredService<CronJobService>());

            //11. Start application
            RunApp();

        }

        private void RunApp()
        {
            var services = _builder.Services;
            var appSetting = _appConfig.ConfigSettings(services);
            var app = _builder.Build();
            _appConfig.ConfigApp(app, appSetting);
            app.Run();
        }

    }
}
