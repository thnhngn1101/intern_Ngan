using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.Json;
using BPMaster.Common.Application.Settings;
using Common.Application.Exceptions;
using Common.Application.Middlewares;
using Common.Application.Settings;
using Common.Loggers.Interfaces;
using Common.Loggers.SeriLog;
using Common.Security;
using Common.Services;
using Kpmg.Blue.Common.Services;
using Kpmg.Blue.Common.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;

namespace Common.Application.Configurations
{
    public abstract class BaseAppConfiguration<S> where S : BaseAppSetting
    {
        protected List<Type> _middlewareList = new()
		{
			typeof(GlobalExceptionHandlerMiddleware),

            //typeof(AuthenticateMiddleware),
            //typeof(SignatureMiddleware)
        };
        protected readonly IWebHostEnvironment _environment;

		protected abstract void LoadAdditionalSetting(S setting, IConfiguration configuration);

		protected abstract void ConfigBackgroundServices(IServiceCollection services);

        public BaseAppConfiguration(IWebHostEnvironment? environment)
        {
            _environment = environment ?? throw new Exception("Cannot get evironment of application....");

        }

        public virtual void ConfigApi<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation>(IServiceCollection services, string xmlPath)
					where TImplementation : BaseConfigureSwaggerOptions
		{
			ConfigWebApi.Config<TImplementation>(services, xmlPath);
		}

		public virtual void ConfigApp(WebApplication app, BaseAppSetting setting)
		{
            //Swagger setting
            //only use swagger for develop local
            //app.Environment.IsDevelopment()
            if (true)
			{
				app.UseSwagger();
				app.UseSwaggerUI(options =>
				{
					var descriptions = app.DescribeApiVersions();

					// Build a swagger endpoint for each discovered API version
					foreach (var description in descriptions)
					{
						var url = "";
                        url = $"/swagger/{description.GroupName}/swagger.json";
						if (app.Environment.IsDevelopment() == false)
						{
							url = $"https://gateway.dev.meu-solutions.com/bpmaster-dev/swagger/v1/swagger.json";
						}

						var name = description.GroupName.ToUpperInvariant();
						options.SwaggerEndpoint(url, name);
					}
				});
			}
            // Enable CORS
            app.UseCors("MyPolicy");

            app.UseRouting();

            app.Use(async (context, next) =>
            {
                context.Response.Headers.AccessControlAllowOrigin = "*";
                await next();
            });

            app.UseHttpsRedirection();
			app.UseAuthentication();
			app.UseAuthorization();
			app.MapControllers();


            var attachmentPath = Path.Combine(setting.STORAGE_PUBLIC.TrimEnd('\\'), setting.STORAGE_ATTACHMENTS.TrimStart('\\')).Replace("\\", "/");
            //Delare static physical File provider
            //app.UseStaticFiles();
            //app.UseStaticFiles(new StaticFileOptions
            //{
            //    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), setting.STORAGE_ROOT)),
            //    RequestPath = "/StaticFiles",
            //    OnPrepareResponse = context =>
            //    {
            //        context.Context.Response.Headers["Access-Control-Allow-Origin"] = "*";
            //    }

            //});

            //if(!Directory.Exists(Path.Combine(setting.STORAGE_ROOT, attachmentPath)))
            //{
            //    Directory.CreateDirectory(Path.Combine(setting.STORAGE_ROOT, attachmentPath));
            //}

            //app.UseStaticFiles(new StaticFileOptions
            //{
            //    FileProvider = new PhysicalFileProvider(Path.Combine(setting.STORAGE_ROOT, attachmentPath)),
            //    RequestPath = "/public",
            //    ServeUnknownFileTypes = true,
            //    OnPrepareResponse = context =>
            //    {
            //        context.Context.Response.Headers["Access-Control-Allow-Origin"] = "*";
            //    }
            //});


            //app.UseStaticFiles(new StaticFileOptions
            //{
            //    FileProvider = new PhysicalFileProvider(
            //Path.Combine(app.Environment.ContentRootPath, "StaticFiles")),
            //    RequestPath = "/StaticFiles"
            //});

            //Add middlewares
            foreach (var middleware in _middlewareList)
			{
				app.UseMiddleware(middleware);
			}
		}

		public virtual void ConfigServices(IServiceCollection services, Assembly assembly, BaseAppSetting setting)
        {
			//Generic services need to inject
			services.AddSingleton<ILogManager>(new LogManager(_environment.EnvironmentName));
			services.AddScoped<IUnitsOfWork, UnitsOfWork>();
            services.AddScoped<AuthUserService<S>>();
            services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
            });

            //Add Authentication
            services.AddAuthentication("Jwt").AddScheme<JwtAuthenticationOptions, JwtAuthenticationHandler<S>>("Jwt", null);
            services.AddSignalR();

            //Auto configure by service attribute
            ConfigService.RegisterByServiceAttribute(services, assembly);

			//Configure background services
			ConfigBackgroundServices(services);

            //Configure client
            //services.AddHttpClient();

            //Configure validation error response
            services.PostConfigure<ApiBehaviorOptions>(options =>
				options.InvalidModelStateResponseFactory = actionContext =>
				{
					var message = string.Join(",", actionContext.ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList());
					throw new ValidationException(message);
					//return new BadRequestObjectResult(ResponseModel.Error(message, System.Net.HttpStatusCode.BadRequest, "ValidationFailed"));
				}
			);

            // Configure CORS policy
            services.AddCors(options =>
            {
                options.AddPolicy("MyPolicy", builder =>
                {
                    builder.SetIsOriginAllowedToAllowWildcardSubdomains()
                           .WithOrigins(
                                "http://localhost",
                                "capacitor://localhost*",
                                "capacitor://localhost",
                                "http://localhost:3000",
                                "http://localhost*",
                                "https://bpmaster.dev.meu-solutions.com",
                                "https://bpmaster.erp.meu-solutions.com",
                                "https://bpmaster-dev.lsp.com",
                                "http://192.168.10.18:3000",
                                "*")
                           .AllowAnyHeader()
                           .AllowAnyMethod()
                           .AllowCredentials();
                });
            });
        }

		public virtual S ConfigSettings(IServiceCollection services) {
			var (setting, configuration) = ConfigAppSetting.LoadSetting<S>(services, _environment);
            
			setting.JwtTokenSetting = ConfigAppSetting.LoadToObject<JwtTokenSetting>(configuration, "JwtTokenSetting");
            setting.FolderGenerateSqlScript = ConfigAppSetting.LoadToObjectAllowNull<string>(configuration, "FolderGenerateSqlScript");
			setting.ExternalServicesSetting = ConfigAppSetting.LoadToObjectAllowNull<ExternalServicesSetting>(configuration, "ExternalServicesSetting");
			setting.SAPInterfaceSetting = ConfigAppSetting.LoadToObjectAllowNull<SAPInterfaceSetting>(configuration, "SAPIterfaceSetting");
            setting.SAPInterfaceConnection = ConfigAppSetting.LoadToObjectAllowNull<SAPInterfaceConnection>(configuration, "SAPInterfaceConnection");
            setting.EmailServerSetting = ConfigAppSetting.LoadToObjectAllowNull<EmailServerSetting>(configuration, "EmailServerSetting");
            setting.SystemIntergrationSetting = ConfigAppSetting.LoadToObjectAllowNull<SystemIntergrationSetting>(configuration, "SystemIntergrationSetting");
            setting.LoginInfoSetting = ConfigAppSetting.LoadToObjectAllowNull<LoginInfoSetting>(configuration, "LoginInfoSetting");
            setting.STORAGE_ROOT = ConfigAppSetting.LoadToObjectAllowNull<string>(configuration, "STORAGE_ROOT")!;
            setting.STORAGE_PUBLIC = ConfigAppSetting.LoadToObjectAllowNull<string>(configuration, "STORAGE_PUBLIC")!;
            setting.STORAGE_ATTACHMENTS = ConfigAppSetting.LoadToObjectAllowNull<string>(configuration, "STORAGE_ATTACHMENTS")!;
            setting.OMSInterfaceSetting = ConfigAppSetting.LoadToObjectAllowNull<OMSInterfaceSetting>(configuration, "OMSInterfaceSetting")!;
            setting.DOMAIN_URL = ConfigAppSetting.LoadToObjectAllowNull<string>(configuration, "DOMAIN_URL");
            //Add more specific setting here
            LoadAdditionalSetting(setting, configuration);

            services.AddSingleton(setting);

			return setting;
        }
    }
}
