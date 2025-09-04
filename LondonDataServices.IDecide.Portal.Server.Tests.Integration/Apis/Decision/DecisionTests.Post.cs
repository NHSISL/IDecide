// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Portal.Server.Tests.Integration.Models.Decisions;

namespace LondonDataServices.IDecide.Portal.Server.Tests.Integration.Apis
{
    public partial class DecisionApiTests
    {
        [Fact]
        public async Task ShouldPostDecisionAsync()
        {
            // given
            Decision randomDecision = CreateRandomDecision();
            Decision expectedDecision = randomDecision;

            // when 
            await this.apiBroker.PostDecisionAsync(randomDecision);

            Decision actualDecision =
                await this.apiBroker.GetDecisionByIdAsync(randomDecision.Id);

            // then
            actualDecision.Should().BeEquivalentTo(
                expectedDecision,
                options => options
                    .Excluding(decision => decision.CreatedBy)
                    .Excluding(decision => decision.CreatedDate)
                    .Excluding(decision => decision.UpdatedBy)
                    .Excluding(decision => decision.UpdatedDate));

            await this.apiBroker.DeleteDecisionByIdAsync(actualDecision.Id);
        }
    }
}
