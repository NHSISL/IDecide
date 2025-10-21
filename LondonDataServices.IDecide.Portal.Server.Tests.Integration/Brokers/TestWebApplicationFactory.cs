// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Linq;
using Attrify.InvisibleApi.Models;
using ISL.Providers.Captcha.Abstractions;
using ISL.Providers.Captcha.Abstractions.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace LondonDataServices.IDecide.Portal.Server.Tests.Integration.Brokers
{
    public class TestWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        private readonly bool requireAuthentication;

        public TestWebApplicationFactory(bool requireAuthentication)
        {
            this.requireAuthentication = requireAuthentication;
        }

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
                if (this.requireAuthentication)
                {
                    OverrideSecurityForTesting(services);
                }
                else
                {
                    OverrideCaptchaForTesting(services);
                }
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
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("TestPolicy", policy => policy.RequireAssertion(_ => true));
            });
        }

        private static void OverrideCaptchaForTesting(IServiceCollection services)
        {
            var captchaDescriptor = services
                .FirstOrDefault(d => d.ServiceType == typeof(ICaptchaAbstractionProvider));

            if (captchaDescriptor != null)
            {
                services.Remove(captchaDescriptor);
            }

            var mockCaptchaAbstractionProvider = new Mock<ICaptchaAbstractionProvider>();

            var successResult = new CaptchaResult
            {
                Success = true,
                Score = 1.0
            };

            mockCaptchaAbstractionProvider.Setup(provider =>
                provider.ValidateCaptchaAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .ReturnsAsync(successResult);

            services.AddTransient<ICaptchaAbstractionProvider>(
                serviceProvider => mockCaptchaAbstractionProvider.Object);
        }
    }
}
