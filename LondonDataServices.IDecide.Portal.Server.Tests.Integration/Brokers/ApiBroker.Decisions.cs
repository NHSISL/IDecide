// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Portal.Server.Tests.Integration.Models.Decisions;

namespace LondonDataServices.IDecide.Portal.Server.Tests.Integration.Brokers
{
    public partial class ApiBroker
    {
        private const string decisionsRelativeUrl = "api/decisions";

        public async ValueTask<Decision> PostDecisionAsync(Decision decision) =>
            await this.authenticatedApiFactoryClient.PostContentAsync(decisionsRelativeUrl, decision);

        public async ValueTask<List<Decision>> GetAllDecisionsAsync() =>
            await this.authenticatedApiFactoryClient.GetContentAsync<List<Decision>>(decisionsRelativeUrl);

        public async ValueTask<Decision> GetDecisionByIdAsync(Guid decisionId) =>
            await this.authenticatedApiFactoryClient.GetContentAsync<Decision>($"{decisionsRelativeUrl}/{decisionId}");

        public async ValueTask<Decision> PutDecisionAsync(Decision decision) =>
            await this.authenticatedApiFactoryClient.PutContentAsync(decisionsRelativeUrl, decision);

        public async ValueTask<Decision> DeleteDecisionByIdAsync(Guid decisionId) =>
            await this.authenticatedApiFactoryClient.DeleteContentAsync<Decision>($"{decisionsRelativeUrl}/{decisionId}");
    }
}