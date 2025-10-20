// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Net.Http;
using Attrify.InvisibleApi.Models;
using Microsoft.Extensions.DependencyInjection;
using RESTFulSense.Clients;

namespace LondonDataServices.IDecide.Portal.Server.Tests.Integration.Brokers
{
    public partial class ApiBroker
    {
        private readonly TestWebApplicationFactory<Program> webApplicationFactory;
        private readonly HttpClient httpClient;
        private readonly IRESTFulApiFactoryClient apiFactoryClient;
        internal readonly InvisibleApiKey invisibleApiKey;

        public ApiBroker()
        {
            webApplicationFactory = new TestWebApplicationFactory<Program>();
            invisibleApiKey = this.webApplicationFactory.Services.GetService<InvisibleApiKey>();
            httpClient = webApplicationFactory.CreateClient();

            this.httpClient.DefaultRequestHeaders
                .Add(this.invisibleApiKey.Key, this.invisibleApiKey.Value);

            apiFactoryClient = new RESTFulApiFactoryClient(httpClient);
        }
    }
}