// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Net.Http;
using Attrify.InvisibleApi.Models;
using LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Tests.Acceptance.Brokers;
using Microsoft.Extensions.DependencyInjection;
using RESTFulSense.Clients;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Brokers
{
    public partial class ApiBroker
    {
        private readonly TestWebApplicationFactory<Program> webApplicationFactory;
        private readonly HttpClient httpClient;
        private readonly IRESTFulApiFactoryClient apiFactoryClient;
        internal readonly InvisibleApiKey invisibleApiKey;

        public ApiBroker()
        {
            this.webApplicationFactory = new TestWebApplicationFactory<Program>();
            this.invisibleApiKey = this.webApplicationFactory.Services.GetService<InvisibleApiKey>();
            this.httpClient = this.webApplicationFactory.CreateClient();
            this.httpClient.DefaultRequestHeaders.Add(this.invisibleApiKey.Key, this.invisibleApiKey.Value);
            this.apiFactoryClient = new RESTFulApiFactoryClient(this.httpClient);
        }
    }
}