// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Models.Decisions;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Apis
{
    public partial class DecisionApiTests
    {
        [Fact]
        public async Task ShouldDeleteDecisionByIdAsync()
        {
            // given
            Decision randomDecision = await PostRandomDecisionAsync();
            Decision inputDecision = randomDecision;
            Decision expectedDecision = inputDecision;

            // when
            Decision deletedDecision =
                await this.apiBroker.DeleteDecisionByIdAsync(inputDecision.Id);

            List<Decision> actualResult =
                await this.apiBroker.GetSpecificDecisionByIdAsync(inputDecision.Id);

            // then
            actualResult.Count().Should().Be(0);
        }
    }
}