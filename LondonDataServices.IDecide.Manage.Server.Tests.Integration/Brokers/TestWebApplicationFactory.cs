// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using Attrify.InvisibleApi.Models;
using LondonDataServices.IDecide.Core.Brokers.NhsDigitalApi;
using LondonDataServices.IDecide.Core.Services.Foundations.NhsDigitalApis;
using LondonDataServices.IDecide.Core.Services.Orchestrations.NhsDigitalApis;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Integration.Brokers
{
    public class TestWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((context, config) =>
            {
                config
                    .AddJsonFile("appsettings.json", optional: true)
                    .AddJsonFile("appsettings.Development.json", optional: true)
                    .AddEnvironmentVariables();
            });

            builder.ConfigureServices((context, services) =>
            {
                OverrideSecurityForTesting(services);
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
            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("TestScheme", null);

            services.AddAuthorization(options =>
            {
                options.AddPolicy("TestPolicy", policy => policy.RequireAssertion(_ => true));
            });

            services.AddDistributedMemoryCache();
            services.AddSession();
            services.AddTransient<IStartupFilter, SessionStartupFilter>();
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

        private static void OverrideNhsDigitalApiOrchestrationServiceForTesting(
            IServiceCollection services)
        {
            var descriptor = services
                .FirstOrDefault(d => d.ServiceType == typeof(INhsDigitalApiOrchestrationService));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            var mockOrchestrationService = new Mock<INhsDigitalApiOrchestrationService>();

            mockOrchestrationService
                .Setup(service => service.BuildLoginUrlAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync("https://cis2.nhs.uk/authorize");

            services.AddTransient<INhsDigitalApiOrchestrationService>(
                serviceProvider => mockOrchestrationService.Object);
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
