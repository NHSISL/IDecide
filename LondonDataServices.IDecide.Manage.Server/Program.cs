// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Attrify.Extensions;
using Attrify.InvisibleApi.Models;
using ISL.Providers.Captcha.Abstractions;
using ISL.Providers.Captcha.FakeCaptcha.Providers.FakeCaptcha;
using ISL.Providers.Captcha.GoogleReCaptcha.Models.Brokers.GoogleReCaptcha;
using ISL.Providers.Captcha.GoogleReCaptcha.Providers;
using ISL.Providers.Notifications.Abstractions;
using ISL.Providers.Notifications.GovukNotify.Models;
using ISL.Providers.Notifications.GovukNotify.Providers.Notifications;
using ISL.Providers.Notifications.NotifyIntercept.Providers.Notifications;
using ISL.Providers.PDS.Abstractions;
using ISL.Providers.PDS.FHIR.Models.Brokers.PdsFHIR;
using ISL.Providers.PDS.FHIR.Providers;
using ISL.Security.Client.Models.Clients;
using LondonDataServices.IDecide.Core.Brokers.Audits;
using LondonDataServices.IDecide.Core.Brokers.DateTimes;
using LondonDataServices.IDecide.Core.Brokers.Identifiers;
using LondonDataServices.IDecide.Core.Brokers.Loggings;
using LondonDataServices.IDecide.Core.Brokers.Notifications;
using LondonDataServices.IDecide.Core.Brokers.Pds;
using LondonDataServices.IDecide.Core.Brokers.Securities;
using LondonDataServices.IDecide.Core.Brokers.Storages.Sql;
using LondonDataServices.IDecide.Core.Clients.Audits;
using LondonDataServices.IDecide.Core.Models.Brokers.Securities;
using LondonDataServices.IDecide.Core.Models.Foundations.Audits;
using LondonDataServices.IDecide.Core.Models.Foundations.ConsumerAdoptions;
using LondonDataServices.IDecide.Core.Models.Foundations.Consumers;
using LondonDataServices.IDecide.Core.Models.Foundations.Decisions;
using LondonDataServices.IDecide.Core.Models.Foundations.Notifications;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using LondonDataServices.IDecide.Core.Models.Foundations.Users;
using LondonDataServices.IDecide.Core.Models.Orchestrations.Decisions;
using LondonDataServices.IDecide.Core.Services.Foundations.Audits;
using LondonDataServices.IDecide.Core.Services.Foundations.ConsumerAdoptions;
using LondonDataServices.IDecide.Core.Services.Foundations.Consumers;
using LondonDataServices.IDecide.Core.Services.Foundations.Decisions;
using LondonDataServices.IDecide.Core.Services.Foundations.DecisionTypes;
using LondonDataServices.IDecide.Core.Services.Foundations.NhsLogins;
using LondonDataServices.IDecide.Core.Services.Foundations.Notifications;
using LondonDataServices.IDecide.Core.Services.Foundations.Patients;
using LondonDataServices.IDecide.Core.Services.Foundations.Pds;
using LondonDataServices.IDecide.Core.Services.Orchestrations.Consumers;
using LondonDataServices.IDecide.Core.Services.Orchestrations.Decisions;
using LondonDataServices.IDecide.Core.Services.Orchestrations.Patients;
using LondonDataServices.IDecide.Manage.Server.Data;
using LondonDataServices.IDecide.Manage.Server.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using NHSDigital.ApiPlatform.Sdk;
using NHSDigital.ApiPlatform.Sdk.AspNetCore;
using NHSDigital.ApiPlatform.Sdk.Models.Configurations;

namespace LondonDataServices.IDecide.Manage.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var invisibleApiKey = new InvisibleApiKey();
            ConfigureServices(builder, builder.Configuration, invisibleApiKey);
            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var storageBroker = scope.ServiceProvider.GetRequiredService<StorageBroker>();
                storageBroker.Database.Migrate();

                var applicationDbContext =
                    scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                applicationDbContext.Database.Migrate();
            }

            ConfigurePipeline(app, invisibleApiKey);
            app.Run();
        }

        public static void ConfigureServices(
            WebApplicationBuilder builder,
            IConfiguration configuration,
            InvisibleApiKey invisibleApiKey)
        {
            // Load settings from launchSettings.json (for testing)
            var projectDir = Directory.GetCurrentDirectory();
            var launchSettingsPath = Path.Combine(projectDir, "Properties", "launchSettings.json");

            if (File.Exists(launchSettingsPath))
            {
                builder.Configuration.AddJsonFile(launchSettingsPath);
            }

            builder.Configuration
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile(
                    $"appsettings.{builder.Environment.EnvironmentName}.json",
                    optional: true,
                    reloadOnChange: true)
                .AddEnvironmentVariables();

            // Configure session cache using SQL Server
            builder.Services.AddDistributedSqlServerCache(options =>
            {
                options.ConnectionString = configuration.GetConnectionString("SessionCache");
                options.SchemaName = "dbo";
                options.TableName = "SessionCache";
            });

            // Configure session
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.Name = ".IDecide.Session";
            });

            // Configure Data Protection
            var keysPath = builder.Environment.IsProduction()
                ? Path.Combine(builder.Environment.ContentRootPath, "DataProtectionKeys")
                : Path.Combine(Path.GetTempPath(), "IDecide-DataProtection-Dev");

            Directory.CreateDirectory(keysPath);

            builder.Services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(keysPath))
                .SetApplicationName("IDecide")
                .SetDefaultKeyLifetime(TimeSpan.FromDays(90));

            // Add services to the container.
            var azureAdOptions = builder.Configuration.GetSection("AzureAd");

            // Configure dual authentication: JWT for API calls and Cookie for BFF pattern
            builder.Services.AddAuthentication("bff-cookie")
                .AddCookie("bff-cookie", options =>
                {
                    options.Events.OnRedirectToLogin = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return Task.CompletedTask;
                    };
                    //options.LoginPath = "/Login";
                    options.LogoutPath = "/Logout";
                    options.ExpireTimeSpan = TimeSpan.FromDays(7);
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.Cookie.SameSite = SameSiteMode.Lax;
                    options.Cookie.Name = "bff-cookie";
                });

            // NHS Digital API Platform SDK (Core + AspNetCore/session storage)
            ApiPlatformConfigurations apiPlatformConfigurations = new()
            {
                CareIdentity = new CareIdentityConfigurations
                {
                    ClientId = builder.Configuration["CIS:ClientId"] ?? string.Empty,
                    ClientSecret = builder.Configuration["CIS:ClientSecret"] ?? string.Empty,
                    RedirectUri = builder.Configuration["CIS:RedirectUri"] ?? string.Empty,
                    AuthEndpoint = builder.Configuration["CIS:AuthEndpoint"] ?? string.Empty,
                    TokenEndpoint = builder.Configuration["CIS:TokenEndpoint"] ?? string.Empty,
                    UserInfoEndpoint = builder.Configuration["CIS:UserInfoEndpoint"] ?? string.Empty,
                    AcrValues = builder.Configuration["CIS:AALLevel"]
                },
                PersonalDemographicsService = new PersonalDemographicsServiceConfigurations
                {
                    BaseUrl = builder.Configuration["PDS:BaseUrl"]
                        ?? "https://int.api.service.nhs.uk/personal-demographics/FHIR/R4"
                }
            };

            var instance = builder.Configuration["AzureAd:Instance"];
            var tenantId = builder.Configuration["AzureAd:TenantId"];
            var scopes = builder.Configuration["AzureAd:Scopes"];

            if (string.IsNullOrEmpty(instance) || string.IsNullOrEmpty(tenantId) || string.IsNullOrEmpty(scopes))
            {
                throw new InvalidOperationException(
                    "AzureAd configuration is incomplete. Please check appsettings.json.");
            }

            builder.Services.AddSwaggerGen();
            builder.Services.AddScoped<ISecureTokenStorage, SecureTokenStorage>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddApiPlatformSdkCore(
                apiPlatformConfigurations);
            builder.Services.AddApiPlatformSdkAspNetCore();
            builder.Services.AddSingleton(invisibleApiKey);
            builder.Services.AddAuthorization();
            builder.Services.AddDbContext<StorageBroker>();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("IDecideConnectionString")));

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddHttpClient();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddControllers();
            AddProviders(builder.Services, builder.Configuration);
            AddBrokers(builder.Services, builder.Configuration);
            AddFoundationServices(builder.Services);
            AddOrchestrationServices(builder.Services, builder.Configuration);
            AddClients(builder.Services);

            // Register IConfiguration to be available for dependency injection
            builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
            JsonNamingPolicy jsonNamingPolicy = JsonNamingPolicy.CamelCase;

            builder.Services.AddControllers()
               .AddOData(options =>
               {
                   options.AddRouteComponents("odata", GetEdmModel());
                   options.Select().Filter().Expand().OrderBy().Count().SetMaxTop(100);
               })
               .AddJsonOptions(options =>
               {
                   options.JsonSerializerOptions.PropertyNamingPolicy = jsonNamingPolicy;
                   options.JsonSerializerOptions.DictionaryKeyPolicy = jsonNamingPolicy;
                   options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                   options.JsonSerializerOptions.WriteIndented = true;
                   options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
               });
        }

        public static void ConfigurePipeline(WebApplication app, InvisibleApiKey invisibleApiKey)
        {
            app.UseDefaultFiles();
            app.UseStaticFiles();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(configuration =>
                {
                    configuration.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");

                    // Configure OAuth2 for Swagger UI
                    configuration.OAuthClientId(app.Configuration["AzureAd:ClientId"]);
                    configuration.OAuthClientSecret("");
                    configuration.OAuthUsePkce();
                    configuration.OAuthScopes(app.Configuration["AzureAd:Scopes"]);
                });
            }

            app.UseHttpsRedirection();
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseInvisibleApiMiddleware(invisibleApiKey);
            app.MapControllers().WithOpenApi();
            app.MapFallbackToFile("/index.html");
        }

        private static IEdmModel GetEdmModel()
        {
            ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
            builder.EntitySet<Audit>("Audits");
            builder.EntitySet<Patient>("Patients");
            builder.EntitySet<Decision>("Decisions");
            builder.EntitySet<Consumer>("Consumers");
            builder.EntitySet<ConsumerAdoption>("ConsumerAdoptions");
            builder.EnableLowerCamelCase();

            return builder.GetEdmModel();
        }

        private static void AddProviders(IServiceCollection services, IConfiguration configuration)
        {
            NotificationConfig notificationConfig =
                configuration.GetSection("NotificationConfig").Get<NotificationConfig>();

            services.AddSingleton(notificationConfig);
            services.AddTransient<IPdsAbstractionProvider, PdsAbstractionProvider>();
            services.AddTransient<ICaptchaAbstractionProvider, CaptchaAbstractionProvider>();

            bool fakeCaptchaProviderMode =
                configuration.GetSection("FakeCaptchaProviderMode").Get<bool>();

            bool interceptNotificationProviderMode =
                configuration.GetSection("InterceptNotificationProviderMode").Get<bool>();

            PdsFHIRConfigurations pdsFhirConfigurations =
                configuration.GetSection("pdsFHIRConfigurations").Get<PdsFHIRConfigurations>();

            services.AddSingleton(pdsFhirConfigurations);
            services.AddTransient<IPdsProvider, PdsFHIRProvider>();

            if (fakeCaptchaProviderMode == true)
            {
                services.AddTransient<ICaptchaProvider, FakeCaptchaProvider>();
            }
            else
            {
                GoogleReCaptchaConfigurations reCaptchaConfigurations =
                    configuration.GetSection("googleReCaptchaConfigurations")
                        .Get<GoogleReCaptchaConfigurations>();

                services.AddSingleton(reCaptchaConfigurations);
                services.AddTransient<ICaptchaProvider, GoogleReCaptchaProvider>();
            }

            if (interceptNotificationProviderMode == true)
            {
                ISL.Providers.Notifications.NotifyIntercept.Models.NotifyConfigurations notifyConfigurations =
                    configuration.GetSection("NotifyConfigurations")
                        .Get<ISL.Providers.Notifications.NotifyIntercept.Models.NotifyConfigurations>();

                NotifyConfigurations govUkNotifyConfigurations =
                    configuration.GetSection("NotifyConfigurations").Get<NotifyConfigurations>();

                var govUkNotifyProvider = new GovUkNotifyProvider(govUkNotifyConfigurations);

                var notifyInterceptProvider =
                    new NotifyInterceptProvider(notifyConfigurations, govUkNotifyProvider);

                var notificationAbstractionProvider =
                    new NotificationAbstractionProvider(notifyInterceptProvider);

                services.AddTransient<INotificationAbstractionProvider>(_ => notificationAbstractionProvider);
            }
            else
            {
                NotifyConfigurations notifyConfigurations =
                    configuration.GetSection("NotifyConfigurations").Get<NotifyConfigurations>();

                var govUkNotifyProvider = new GovUkNotifyProvider(notifyConfigurations);

                var notificationAbstractionProvider =
                    new NotificationAbstractionProvider(govUkNotifyProvider);

                services.AddTransient<INotificationAbstractionProvider>(_ => notificationAbstractionProvider);
            }
        }

        private static void AddBrokers(IServiceCollection services, IConfiguration configuration)
        {
            SecurityConfigurations securityConfigurations = new SecurityConfigurations();
            services.AddSingleton(securityConfigurations);

            SecurityBrokerConfigurations securityBrokerConfigurations =
                configuration.GetSection("SecurityBrokerConfigurations")
                    .Get<SecurityBrokerConfigurations>();

            services.AddSingleton(securityBrokerConfigurations);
            services.AddTransient<IDateTimeBroker, DateTimeBroker>();
            services.AddTransient<IIdentifierBroker, IdentifierBroker>();
            services.AddTransient<ILoggingBroker, LoggingBroker>();
            services.AddTransient<ISecurityAuditBroker, SecurityAuditBroker>();
            services.AddTransient<ISecurityBroker, SecurityBroker>();
            services.AddTransient<IStorageBroker, StorageBroker>();
            services.AddTransient<INotificationBroker, NotificationBroker>();
            services.AddTransient<IPdsBroker, PdsBroker>();
            services.AddTransient<IAuditBroker, AuditBroker>();
        }

        private static void AddFoundationServices(IServiceCollection services)
        {
            services.AddTransient<IAuditService, AuditService>();
            services.AddTransient<IPdsService, PdsService>();
            services.AddTransient<IDecisionService, DecisionService>();
            services.AddTransient<IDecisionTypeService, DecisionTypeService>();
            services.AddTransient<IPatientService, PatientService>();
            services.AddTransient<INotificationService, NotificationService>();
            services.AddTransient<IConsumerService, ConsumerService>();
            services.AddTransient<IConsumerAdoptionService, ConsumerAdoptionService>();
            services.AddTransient<INhsLoginService, NhsLoginService>();
        }

        private static void AddProcessingServices(IServiceCollection services)
        { }

        private static void AddOrchestrationServices(IServiceCollection services, IConfiguration configuration)
        {
            DecisionConfigurations decisionConfigurations =
                configuration.GetSection("DecisionConfigurations").Get<DecisionConfigurations>() ??
                    new DecisionConfigurations();

            services.AddSingleton(decisionConfigurations);
            services.AddTransient<IPatientOrchestrationService, PatientOrchestrationService>();
            services.AddTransient<IDecisionOrchestrationService, DecisionOrchestrationService>();
            services.AddTransient<IConsumerOrchestrationService, ConsumerOrchestrationService>();
        }

        private static void AddCoordinationServices(IServiceCollection services, IConfiguration configuration)
        { }

        private static void AddClients(IServiceCollection services)
        {
            services.AddTransient<IAuditClient, AuditClient>();
        }
    }
}