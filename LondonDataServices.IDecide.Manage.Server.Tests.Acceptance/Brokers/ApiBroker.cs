// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Net.Http;
using Attrify.InvisibleApi.Models;
using LondonDataServices.IDecide.Core.Services.Orchestrations.NhsDigitalApis;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using RESTFulSense.Clients;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Brokers
{
    public partial class ApiBroker
    {
        private readonly TestWebApplicationFactory<Program> webApplicationFactory;
        private readonly HttpClient httpClient;
        private readonly HttpClient httpClientNoRedirect;
        private readonly IRESTFulApiFactoryClient apiFactoryClient;
        internal readonly InvisibleApiKey invisibleApiKey;
        internal readonly IConfiguration configuration;
        internal readonly Mock<INhsDigitalApiOrchestrationService> nhsDigitalApiOrchestrationServiceMock;

        public ApiBroker()
        {
            this.webApplicationFactory = new TestWebApplicationFactory<Program>();
            this.invisibleApiKey = this.webApplicationFactory.Services.GetService<InvisibleApiKey>();
            this.httpClient = this.webApplicationFactory.CreateClient();
            this.httpClient.DefaultRequestHeaders.Add(this.invisibleApiKey.Key, this.invisibleApiKey.Value);

            this.httpClientNoRedirect = this.webApplicationFactory.CreateClient(
                new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

            this.httpClientNoRedirect.DefaultRequestHeaders.Add(
                this.invisibleApiKey.Key, this.invisibleApiKey.Value);

            this.apiFactoryClient = new RESTFulApiFactoryClient(this.httpClient);
            this.configuration = this.webApplicationFactory.Services.GetService<IConfiguration>();

            this.nhsDigitalApiOrchestrationServiceMock =
                this.webApplicationFactory.NhsDigitalApiOrchestrationServiceMock;
        }
    }
}