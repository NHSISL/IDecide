// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using Attrify.Extensions;
using Attrify.InvisibleApi.Models;
using LondonDataServices.IDecide.Core.Brokers.Storages.Sql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;

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
     //     AddProviders(builder.Services, builder.Configuration);
     //     AddBrokers(builder.Services, builder.Configuration);
     //     AddFoundationServices(builder.Services);
     //     AddProcessingServices(builder.Services);
     //     AddOrchestrationServices(builder.Services, builder.Configuration);
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
        { }

        private static void AddBrokers(IServiceCollection services, IConfiguration configuration)
        { }

        private static void AddFoundationServices(IServiceCollection services)
        { }

        private static void AddProcessingServices(IServiceCollection services)
        { }

        private static void AddOrchestrationServices(IServiceCollection services, IConfiguration configuration)
        { }

        private static void AddCoordinationServices(IServiceCollection services, IConfiguration configuration)
        { }
    }
}
