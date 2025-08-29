// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Portal.Server.Tests.Acceptance.Models.DecisionTypes;

namespace LondonDataServices.IDecide.Portal.Server.Tests.Acceptance.Brokers
{
    public partial class ApiBroker
    {
        private const string decisionTypesRelativeUrl = "api/decisiontypes";

        public async ValueTask<DecisionType> PostDecisionTypeAsync(DecisionType decisionType) =>
            await this.apiFactoryClient.PostContentAsync(decisionTypesRelativeUrl, decisionType);

        public async ValueTask<List<DecisionType>> GetAllDecisionTypesAsync() =>
            await this.apiFactoryClient.GetContentAsync<List<DecisionType>>($"{decisionTypesRelativeUrl}/");

        public async ValueTask<List<DecisionType>> GetSpecificDecisionTypeByIdAsync(Guid decisionTypeId) =>
            await this.apiFactoryClient.GetContentAsync<List<DecisionType>>(
                $"{decisionTypesRelativeUrl}?$filter=Id eq {decisionTypeId}");

        public async ValueTask<DecisionType> GetDecisionTypeByIdAsync(Guid decisionTypeId) =>
            await this.apiFactoryClient
                .GetContentAsync<DecisionType>($"{decisionTypesRelativeUrl}/{decisionTypeId}");

        public async ValueTask<DecisionType> DeleteDecisionTypeByIdAsync(Guid decisionTypeId) =>
            await this.apiFactoryClient
                .DeleteContentAsync<DecisionType>($"{decisionTypesRelativeUrl}/{decisionTypeId}");

        public async ValueTask<DecisionType> PutDecisionTypeAsync(DecisionType decisionType) =>
            await this.apiFactoryClient.PutContentAsync(decisionTypesRelativeUrl, decisionType);
    }
}
