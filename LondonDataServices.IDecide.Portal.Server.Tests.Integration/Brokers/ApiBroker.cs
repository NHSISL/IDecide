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
        private readonly TestWebApplicationFactory<Program> authenticatedWebApplicationFactory;
        private readonly HttpClient authenticatedHttpClient;
        private readonly IRESTFulApiFactoryClient authenticatedApiFactoryClient;
        internal readonly InvisibleApiKey invisibleApiKey;
        private readonly TestWebApplicationFactory<Program> anonymousWebApplicationFactory;
        private readonly HttpClient anonymousHttpClient;
        private readonly IRESTFulApiFactoryClient anonymousApiFactoryClient;

        public ApiBroker()
        {
            authenticatedWebApplicationFactory = new TestWebApplicationFactory<Program>(true);
            invisibleApiKey = this.authenticatedWebApplicationFactory.Services.GetService<InvisibleApiKey>();
            authenticatedHttpClient = authenticatedWebApplicationFactory.CreateClient();

            this.authenticatedHttpClient.DefaultRequestHeaders
                .Add(this.invisibleApiKey.Key, this.invisibleApiKey.Value);

            authenticatedApiFactoryClient = new RESTFulApiFactoryClient(authenticatedHttpClient);

            anonymousWebApplicationFactory = new TestWebApplicationFactory<Program>(false);
            anonymousHttpClient = anonymousWebApplicationFactory.CreateClient();
            anonymousApiFactoryClient = new RESTFulApiFactoryClient(anonymousHttpClient);
        }
    }
}