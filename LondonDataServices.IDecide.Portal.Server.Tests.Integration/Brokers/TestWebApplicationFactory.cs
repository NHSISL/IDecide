// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Linq;
using ISL.Providers.Captcha.Abstractions;
using ISL.Providers.Captcha.Abstractions.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace LondonDataServices.IDecide.Portal.Server.Tests.Integration.Brokers
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
                OverrideCaptchaForTesting(services);
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
