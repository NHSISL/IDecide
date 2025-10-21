// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Portal.Server.Tests.Integration.Models.DecisionTypes;

namespace LondonDataServices.IDecide.Portal.Server.Tests.Integration.Brokers
{
    public partial class ApiBroker
    {
        private const string decisionTypesRelativeUrl = "api/decisiontypes";

        public async ValueTask<DecisionType> PostDecisionTypeAsync(DecisionType decisionType) =>
            await this.authenticatedApiFactoryClient.PostContentAsync(decisionTypesRelativeUrl, decisionType);

        public async ValueTask<List<DecisionType>> GetAllDecisionTypesAsync() =>
            await this.authenticatedApiFactoryClient.GetContentAsync<List<DecisionType>>(decisionTypesRelativeUrl);

        public async ValueTask<DecisionType> GetDecisionTypeByIdAsync(Guid decisionTypeId) =>
            await this.authenticatedApiFactoryClient.GetContentAsync<DecisionType>($"{decisionTypesRelativeUrl}/{decisionTypeId}");

        public async ValueTask<DecisionType> PutDecisionTypeAsync(DecisionType decisionType) =>
            await this.authenticatedApiFactoryClient.PutContentAsync(decisionTypesRelativeUrl, decisionType);

        public async ValueTask<DecisionType> DeleteDecisionTypeByIdAsync(Guid decisionTypeId) =>
            await this.authenticatedApiFactoryClient.DeleteContentAsync<DecisionType>($"{decisionTypesRelativeUrl}/{decisionTypeId}");
    }
}