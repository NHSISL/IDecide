// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Models.Decisions;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Brokers
{
    public partial class ApiBroker
    {
        private const string decisionsRelativeUrl = "api/decisions";

        public async ValueTask<Decision> PostDecisionAsync(Decision decision) =>
            await this.apiFactoryClient.PostContentAsync(decisionsRelativeUrl, decision);

        public async ValueTask<List<Decision>> GetAllDecisionsAsync() =>
            await this.apiFactoryClient.GetContentAsync<List<Decision>>($"{decisionsRelativeUrl}/");

        public async ValueTask<List<Decision>> GetSpecificDecisionByIdAsync(Guid decisionId) =>
            await this.apiFactoryClient.GetContentAsync<List<Decision>>(
                $"{decisionsRelativeUrl}?$filter=Id eq {decisionId}");

        public async ValueTask<Decision> GetDecisionByIdAsync(Guid decisionId) =>
            await this.apiFactoryClient
                .GetContentAsync<Decision>($"{decisionsRelativeUrl}/{decisionId}");

        public async ValueTask<Decision> DeleteDecisionByIdAsync(Guid decisionId) =>
            await this.apiFactoryClient
                .DeleteContentAsync<Decision>($"{decisionsRelativeUrl}/{decisionId}");

        public async ValueTask<Decision> PutDecisionAsync(Decision decision) =>
            await this.apiFactoryClient.PutContentAsync(decisionsRelativeUrl, decision);
    }
}
