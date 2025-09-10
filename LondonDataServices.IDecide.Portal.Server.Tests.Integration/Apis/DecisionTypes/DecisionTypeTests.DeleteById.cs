// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using LondonDataServices.IDecide.Portal.Server.Tests.Integration.Models.DecisionTypes;
using RESTFulSense.Exceptions;

namespace LondonDataServices.IDecide.Portal.Server.Tests.Integration.Apis.DecisionTypes
{
    public partial class DecisionTypeApiTests
    {
        [Fact]
        public async Task ShouldDeleteDecisionTypeAsync()
        {
            // given
            DecisionType randomDecisionType = await PostRandomDecisionTypeAsync();

            // when
            await this.apiBroker.DeleteDecisionTypeByIdAsync(randomDecisionType.Id);

            // then
            ValueTask<DecisionType> getDecisionTypeByIdTask =
                this.apiBroker.GetDecisionTypeByIdAsync(randomDecisionType.Id);

            await Assert.ThrowsAsync<HttpResponseNotFoundException>(getDecisionTypeByIdTask.AsTask);
        }
    }
}
