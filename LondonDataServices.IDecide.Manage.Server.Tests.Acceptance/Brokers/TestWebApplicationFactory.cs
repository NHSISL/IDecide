// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using Attrify.InvisibleApi.Models;
using ISL.Providers.Notifications.Abstractions;
using ISL.Providers.PDS.Abstractions;
using LondonDataServices.IDecide.Core.Brokers.NhsDigitalApi;
using LondonDataServices.IDecide.Core.Services.Foundations.NhsDigitalApis;
using LondonDataServices.IDecide.Core.Services.Orchestrations.NhsDigitalApis;
using ISL.Providers.PDS.FakeFHIR.Models;
using ISL.Providers.PDS.FakeFHIR.Providers.FakeFHIR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Brokers
{
    public class TestWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        public Mock<INhsDigitalApiOrchestrationService> NhsDigitalApiOrchestrationServiceMock { get; } =
            new Mock<INhsDigitalApiOrchestrationService>();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((context, config) =>
            {
                var testProjectPath = Path.GetFullPath(
                    Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));

                config
                    .AddJsonFile("appsettings.json", optional: true)
                    .AddJsonFile("appsettings.Development.json", optional: true)
                    .AddJsonFile(Path.Combine(testProjectPath, "appsettings.json"), optional: true)
                    .AddEnvironmentVariables();
            });

            builder.ConfigureServices((context, services) =>
            {
                OverrideSecurityForTesting(services);
                OverrideFhirProviderForTesting(services, context.Configuration);
                OverrideNotificationProviderForTesting(services, context.Configuration);
                OverrideNhsDigitalApiBrokerForTesting(services);
                OverrideNhsDigitalApiServiceForTesting(services);
                OverrideNhsDigitalApiOrchestrationServiceForTesting(services);
            });
        }

        private static void OverrideSecurityForTesting(IServiceCollection services)
        {
            var invisibleApiKeyDescriptor = services
                .FirstOrDefault(d => d.ServiceType == typeof(InvisibleApiKey));

            InvisibleApiKey invisibleApiKey = null;

            if (invisibleApiKeyDescriptor != null)
            {
                using (var serviceProvider = services.BuildServiceProvider())
                {
                    invisibleApiKey = serviceProvider.GetService<InvisibleApiKey>();
                }
            }

            // Remove existing authentication and authorization
            var authenticationDescriptor = services
                .FirstOrDefault(d => d.ServiceType == typeof(IAuthenticationSchemeProvider));

            if (authenticationDescriptor != null)
            {
                services.Remove(authenticationDescriptor);
            }

            // Override authentication and authorization
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "TestScheme";
                options.DefaultChallengeScheme = "TestScheme";
            })
            .AddScheme<CustomAuthenticationSchemeOptions, TestAuthHandler>("TestScheme", options =>
            {
                options.InvisibleApiKey = invisibleApiKey;
            })
            .AddCookie("bff-cookie");

            services.AddAuthorization(options =>
            {
                options.AddPolicy("TestPolicy", policy => policy.RequireAssertion(_ => true));
            });

            services.AddDistributedMemoryCache();
            services.AddSession();
            services.AddTransient<IStartupFilter, SessionStartupFilter>();
        }

        private static void OverrideFhirProviderForTesting(
            IServiceCollection services,
            IConfiguration configuration)
        {
            var fhirDescriptor = services
                .FirstOrDefault(d => d.ServiceType == typeof(IPdsProvider));

            if (fhirDescriptor != null)
            {
                services.Remove(fhirDescriptor);
            }

            FakeFHIRProviderConfigurations fakeFHIRProviderConfigurations = configuration
                 .GetSection("FakeFHIRProviderConfigurations")
                     .Get<FakeFHIRProviderConfigurations>();

            services.AddSingleton(fakeFHIRProviderConfigurations);
            services.AddTransient<IPdsProvider, FakeFHIRProvider>();
        }

        private static void OverrideNotificationProviderForTesting(
            IServiceCollection services,
            IConfiguration configuration)
        {
            var notificationDescriptor = services
                .FirstOrDefault(d => d.ServiceType == typeof(INotificationAbstractionProvider));

            if (notificationDescriptor != null)
            {
                services.Remove(notificationDescriptor);
            }

            var mockNotificationAbstractionProvider = new Mock<INotificationAbstractionProvider>();

            services.AddTransient<INotificationAbstractionProvider>(
                serviceProvider => mockNotificationAbstractionProvider.Object);
        }

        private void OverrideNhsDigitalApiOrchestrationServiceForTesting(IServiceCollection services)
        {
            var descriptor = services
                .FirstOrDefault(d => d.ServiceType == typeof(INhsDigitalApiOrchestrationService));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.AddTransient<INhsDigitalApiOrchestrationService>(
                serviceProvider => NhsDigitalApiOrchestrationServiceMock.Object);
        }

        private static void OverrideNhsDigitalApiBrokerForTesting(IServiceCollection services)
        {
            var descriptor = services
                .FirstOrDefault(d => d.ServiceType == typeof(INhsDigitalApiBroker));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            var mockBroker = new Mock<INhsDigitalApiBroker>();
            services.AddTransient<INhsDigitalApiBroker>(serviceProvider => mockBroker.Object);
        }

        private static void OverrideNhsDigitalApiServiceForTesting(IServiceCollection services)
        {
            var descriptor = services
                .FirstOrDefault(d => d.ServiceType == typeof(INhsDigitalApiService));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            var mockService = new Mock<INhsDigitalApiService>();
            services.AddTransient<INhsDigitalApiService>(serviceProvider => mockService.Object);
        }
    }

    internal sealed class SessionStartupFilter : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next) =>
            app =>
            {
                app.UseSession();
                next(app);
            };
    }
}
