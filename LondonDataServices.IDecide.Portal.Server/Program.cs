// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.IO;
using System.Text.Json;
using Attrify.Extensions;
using Attrify.InvisibleApi.Models;
using ISL.Providers.Captcha.Abstractions;
using ISL.Providers.Captcha.FakeCaptcha.Providers.FakeCaptcha;
using ISL.Providers.Captcha.GoogleReCaptcha.Models.Brokers.GoogleReCaptcha;
using ISL.Providers.Captcha.GoogleReCaptcha.Providers;
using ISL.Providers.Notifications.Abstractions;
using ISL.Providers.Notifications.GovukNotify.Models;
using ISL.Providers.Notifications.GovukNotify.Providers.Notifications;
using ISL.Providers.PDS.Abstractions;
using ISL.Providers.PDS.FakeFHIR.Models;
using ISL.Providers.PDS.FakeFHIR.Providers.FakeFHIR;
using ISL.Providers.PDS.FHIR.Models.Brokers.PdsFHIR;
using ISL.Providers.PDS.FHIR.Providers;
using ISL.Security.Client.Models.Clients;
using LondonDataServices.IDecide.Core.Brokers.DateTimes;
using LondonDataServices.IDecide.Core.Brokers.Identifiers;
using LondonDataServices.IDecide.Core.Brokers.Loggings;
using LondonDataServices.IDecide.Core.Brokers.Notifications;
using LondonDataServices.IDecide.Core.Brokers.Pds;
using LondonDataServices.IDecide.Core.Brokers.Securities;
using LondonDataServices.IDecide.Core.Brokers.Storages.Sql;
using LondonDataServices.IDecide.Core.Models.Brokers.Notifications;
using LondonDataServices.IDecide.Core.Models.Foundations.Notifications;
using LondonDataServices.IDecide.Core.Models.Orchestrations.Patients;
using LondonDataServices.IDecide.Core.Services.Foundations.Audits;
using LondonDataServices.IDecide.Core.Services.Foundations.Consumers;
using LondonDataServices.IDecide.Core.Services.Foundations.Decisions;
using LondonDataServices.IDecide.Core.Services.Foundations.DecisionTypes;
using LondonDataServices.IDecide.Core.Services.Foundations.Notifications;
using LondonDataServices.IDecide.Core.Services.Foundations.Patients;
using LondonDataServices.IDecide.Core.Services.Foundations.Pds;
using LondonDataServices.IDecide.Core.Services.Orchestrations.Patients;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LondonDataServices.IDecide.Portal.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var invisibleApiKey = new InvisibleApiKey();
            ConfigureServices(builder, builder.Configuration, invisibleApiKey);
            var app = builder.Build();
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
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();


            builder.Services.AddSwaggerGen(configuration =>
            {
                // Add an OAuth2 security definition for Azure AD
            });

            builder.Services.AddSingleton(invisibleApiKey);
            builder.Services.AddAuthorization();
            builder.Services.AddDbContext<StorageBroker>();
            builder.Services.AddHttpContextAccessor();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddControllers();
            AddProviders(builder.Services, builder.Configuration);
            AddBrokers(builder.Services, builder.Configuration);
            AddFoundationServices(builder.Services);
            AddOrchestrationServices(builder.Services, builder.Configuration);
            //     AddProcessingServices(builder.Services);
            //     AddCoordinationServices(builder.Services, builder.Configuration);

            // Register IConfiguration to be available for dependency injection
            builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
            JsonNamingPolicy jsonNamingPolicy = JsonNamingPolicy.CamelCase;

            builder.Services.AddControllers()
               .AddJsonOptions(options =>
               {
                   options.JsonSerializerOptions.PropertyNamingPolicy = jsonNamingPolicy;
                   options.JsonSerializerOptions.DictionaryKeyPolicy = jsonNamingPolicy;
                   options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                   options.JsonSerializerOptions.WriteIndented = true;
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
                    configuration.OAuthClientId(app.Configuration["AzureAd:ClientId"]); // Use the application ClientId
                    configuration.OAuthClientSecret("");
                    configuration.OAuthUsePkce(); // Enable PKCE (Proof Key for Code Exchange)
                    configuration.OAuthScopes(app.Configuration["AzureAd:Scopes"]); // Add required scopes
                });
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseInvisibleApiMiddleware(invisibleApiKey);
            app.MapControllers().WithOpenApi();
            app.MapFallbackToFile("/index.html");
        }

        private static void AddProviders(IServiceCollection services, IConfiguration configuration)
        {
            NotificationConfigurations notificationConfigurations = configuration
                .GetSection("NotificationConfigurations")
                    .Get<NotificationConfigurations>();

            NotifyConfigurations notifyConfigurations = new NotifyConfigurations
            {
                ApiKey = notificationConfigurations.ApiKey
            };

            NotificationConfig notificationConfig = configuration.GetSection("NotificationConfig")
                .Get<NotificationConfig>();

            services.AddSingleton(notificationConfigurations);
            services.AddSingleton(notifyConfigurations);
            services.AddSingleton(notificationConfig);
            services.AddTransient<IPdsAbstractionProvider, PdsAbstractionProvider>();
            services.AddTransient<INotificationAbstractionProvider, NotificationAbstractionProvider>();
            services.AddTransient<INotificationProvider, GovukNotifyProvider>();
            services.AddTransient<ICaptchaAbstractionProvider, CaptchaAbstractionProvider>();

            bool fakeFHIRProviderMode = configuration
                .GetSection("FakeFHIRProviderMode").Get<bool>();

            bool fakeCaptchaProviderMode = configuration
                .GetSection("FakeCaptchaProviderMode").Get<bool>();

            if (fakeFHIRProviderMode == true)
            {
                FakeFHIRProviderConfigurations fakeFHIRProviderConfigurations = configuration
                .GetSection("FakeFHIRProviderConfigurations")
                    .Get<FakeFHIRProviderConfigurations>();

                services.AddSingleton(fakeFHIRProviderConfigurations);
                services.AddTransient<IPdsProvider, FakeFHIRProvider>();
            }
            else
            {
                PdsFHIRConfigurations pdsFhirConfigurations = configuration
                .GetSection("pdsFHIRConfigurations")
                    .Get<PdsFHIRConfigurations>();

                services.AddSingleton(pdsFhirConfigurations);
                services.AddTransient<IPdsProvider, PdsFHIRProvider>();
            }

            if (fakeCaptchaProviderMode == true)
            {
                services.AddTransient<ICaptchaProvider, FakeCaptchaProvider>();
            }
            else
            {
                GoogleReCaptchaConfigurations reCaptchaConfigurations = configuration
                .GetSection("googleReCaptchaConfigurations")
                    .Get<GoogleReCaptchaConfigurations>();

                services.AddSingleton(reCaptchaConfigurations);
                services.AddTransient<ICaptchaProvider, GoogleReCaptchaProvider>();
            }
        }

        private static void AddBrokers(IServiceCollection services, IConfiguration configuration)
        {
            SecurityConfigurations securityConfigurations = new SecurityConfigurations();
            services.AddSingleton(securityConfigurations);
            services.AddTransient<IDateTimeBroker, DateTimeBroker>();
            services.AddTransient<IIdentifierBroker, IdentifierBroker>();
            services.AddTransient<ILoggingBroker, LoggingBroker>();
            services.AddTransient<ISecurityAuditBroker, SecurityAuditBroker>();
            services.AddTransient<ISecurityBroker, SecurityBroker>();
            services.AddTransient<IStorageBroker, StorageBroker>();
            services.AddTransient<INotificationBroker, NotificationBroker>();
            services.AddTransient<IPdsBroker, PdsBroker>();
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
        }

        private static void AddProcessingServices(IServiceCollection services)
        { }

        private static void AddOrchestrationServices(IServiceCollection services, IConfiguration configuration)
        {
            PatientOrchestrationConfigurations patientOrchestrationConfigurations = configuration
                .GetSection("PatientOrchestrationConfigurations")
                    .Get<PatientOrchestrationConfigurations>() ??
                        new PatientOrchestrationConfigurations();

            services.AddSingleton(patientOrchestrationConfigurations);
            services.AddTransient<IPatientOrchestrationService, PatientOrchestrationService>();
        }

        private static void AddCoordinationServices(IServiceCollection services, IConfiguration configuration)
        { }
    }
}
