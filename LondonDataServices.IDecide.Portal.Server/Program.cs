// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
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
using LondonDataServices.IDecide.Core.Brokers.Audits;
using LondonDataServices.IDecide.Core.Brokers.DateTimes;
using LondonDataServices.IDecide.Core.Brokers.Identifiers;
using LondonDataServices.IDecide.Core.Brokers.Loggings;
using LondonDataServices.IDecide.Core.Brokers.Notifications;
using LondonDataServices.IDecide.Core.Brokers.Pds;
using LondonDataServices.IDecide.Core.Brokers.Securities;
using LondonDataServices.IDecide.Core.Brokers.Storages.Sql;
using LondonDataServices.IDecide.Core.Clients.Audits;
using LondonDataServices.IDecide.Core.Models.Brokers.Notifications;
using LondonDataServices.IDecide.Core.Models.Foundations.Audits;
using LondonDataServices.IDecide.Core.Models.Foundations.Consumers;
using LondonDataServices.IDecide.Core.Models.Foundations.ConsumerStatuses;
using LondonDataServices.IDecide.Core.Models.Foundations.Decisions;
using LondonDataServices.IDecide.Core.Models.Foundations.DecisionTypes;
using LondonDataServices.IDecide.Core.Models.Foundations.Notifications;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using LondonDataServices.IDecide.Core.Models.Orchestrations.Decisions;
using LondonDataServices.IDecide.Core.Services.Foundations.Audits;
using LondonDataServices.IDecide.Core.Services.Foundations.Consumers;
using LondonDataServices.IDecide.Core.Services.Foundations.ConsumerStatuses;
using LondonDataServices.IDecide.Core.Services.Foundations.Decisions;
using LondonDataServices.IDecide.Core.Services.Foundations.DecisionTypes;
using LondonDataServices.IDecide.Core.Services.Foundations.Notifications;
using LondonDataServices.IDecide.Core.Services.Foundations.Patients;
using LondonDataServices.IDecide.Core.Services.Foundations.Pds;
using LondonDataServices.IDecide.Core.Services.Orchestrations.Patients;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

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

            using (var scope = app.Services.CreateScope())
            {
                var storageBroker = scope.ServiceProvider.GetRequiredService<StorageBroker>();
                storageBroker.Database.Migrate();
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
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            // Add services to the container.
            var azureAdOptions = builder.Configuration.GetSection("AzureAd");

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApi(azureAdOptions);

            var instance = builder.Configuration["AzureAd:Instance"];
            var tenantId = builder.Configuration["AzureAd:TenantId"];
            var scopes = builder.Configuration["AzureAd:Scopes"];
            var missingKeys = new System.Collections.Generic.List<string>();
            if (string.IsNullOrEmpty(instance)) missingKeys.Add("Instance");
            if (string.IsNullOrEmpty(tenantId)) missingKeys.Add("TenantId");
            if (string.IsNullOrEmpty(scopes)) missingKeys.Add("Scopes");

            if (missingKeys.Count > 0)
            {
                throw new InvalidOperationException(
                    $"AzureAd configuration is incomplete. Missing keys: {string.Join(", ", missingKeys)}. " +
                    $"Please check appsettings.json.");
            }

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
            AddClients(builder.Services);
            //     AddProcessingServices(builder.Services);
            //     AddCoordinationServices(builder.Services, builder.Configuration);

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

        private static IEdmModel GetEdmModel()
        {
            ODataConventionModelBuilder builder =
               new ODataConventionModelBuilder();

            builder.EntitySet<Consumer>("Consumers");
            builder.EntitySet<ConsumerStatus>("ConsumerStatuses");
            builder.EntitySet<DecisionType>("DecisionTypes");
            builder.EntitySet<Decision>("Decisions");
            builder.EntitySet<Patient>("Patients");
            builder.EntitySet<Audit>("Audits");
            builder.EnableLowerCamelCase();

            return builder.GetEdmModel();
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
            services.AddTransient<IConsumerStatusService, ConsumerStatusService>();
        }

        private static void AddProcessingServices(IServiceCollection services)
        { }

        private static void AddOrchestrationServices(IServiceCollection services, IConfiguration configuration)
        {
            DecisionConfigurations decisionConfigurations = configuration
                .GetSection("DecisionConfigurations")
                    .Get<DecisionConfigurations>() ??
                        new DecisionConfigurations();

            services.AddSingleton(decisionConfigurations);
            services.AddTransient<IPatientOrchestrationService, PatientOrchestrationService>();
        }

        private static void AddCoordinationServices(IServiceCollection services, IConfiguration configuration)
        { }

        private static void AddClients(IServiceCollection services)
        {
            services.AddTransient<IAuditClient, AuditClient>();
        }
    }
}
