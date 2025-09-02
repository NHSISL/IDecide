// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Portal.Server.Tests.Acceptance.Models.Decisions;

namespace LondonDataServices.IDecide.Portal.Server.Tests.Acceptance.Apis
{
    public partial class DecisionApiTests
    {
        [Fact]
        public async Task ShouldPostDecisionAsync()
        {
            // given
            Decision randomDecision = CreateRandomDecision();
            Decision inputDecision = randomDecision;
            Decision expectedDecision = inputDecision;

            // when 
            await this.apiBroker.PostDecisionAsync(inputDecision);

            Decision actualDecision =
                await this.apiBroker.GetDecisionByIdAsync(inputDecision.Id);

            // then
            actualDecision.Should().BeEquivalentTo(expectedDecision, options => options
                .Excluding(property => property.CreatedBy)
                .Excluding(property => property.CreatedDate)
                .Excluding(property => property.UpdatedBy)
                .Excluding(property => property.UpdatedDate));

            await this.apiBroker.DeleteDecisionByIdAsync(actualDecision.Id);
        }
    }
}