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
        public async Task ShouldGetDecisionByIdAsync()
        {
            // given
            Decision randomDecision = await PostRandomDecisionAsync();
            Decision expectedDecision = randomDecision;

            // when
            Decision actualDecision = 
                await this.apiBroker.GetDecisionByIdAsync(randomDecision.Id);

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