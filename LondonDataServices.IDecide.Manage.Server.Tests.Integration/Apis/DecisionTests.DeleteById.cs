// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using LondonDataServices.IDecide.Manage.Server.Tests.Integration.Models.Decisions;
using RESTFulSense.Exceptions;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Integration.Apis
{
    public partial class DecisionApiTests
    {
        [Fact]
        public async Task ShouldDeleteDecisionAsync()
        {
            // given
            Decision randomDecision = await PostRandomDecisionAsync();

            // when
            await this.apiBroker.DeleteDecisionByIdAsync(randomDecision.Id);

            // then
            ValueTask<Decision> getDecisionByIdTask = 
                this.apiBroker.GetDecisionByIdAsync(randomDecision.Id);

            await Assert.ThrowsAsync<HttpResponseNotFoundException>(getDecisionByIdTask.AsTask);
        }
    }
}
