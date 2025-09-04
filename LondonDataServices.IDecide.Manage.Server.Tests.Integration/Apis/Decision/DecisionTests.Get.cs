// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Manage.Server.Tests.Integration.Models.Decisions;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Integration.Apis
{
    public partial class DecisionApiTests
    {
        [Fact]
        public async Task ShouldGetAllDecisionsAsync()
        {
            // given
            List<Decision> randomDecisions = await PostRandomDecisionsAsync();
            List<Decision> expectedDecisions = randomDecisions;

            // when
            List<Decision> actualDecisions = await this.apiBroker.GetAllDecisionsAsync();

            // then
            actualDecisions.Should().NotBeNull();

            foreach (Decision expectedDecision in expectedDecisions)
            {
                Decision actualDecision = actualDecisions
                    .Single(decision => decision.Id == expectedDecision.Id);

                actualDecision.Should().BeEquivalentTo(
                    expectedDecision,
                    options => options
                        .Excluding(property => property.CreatedBy)
                        .Excluding(property => property.CreatedDate)
                        .Excluding(property => property.UpdatedBy)
                        .Excluding(property => property.UpdatedDate));
            }

            // cleanup
            foreach (Decision createdDecision in expectedDecisions)
            {
                await this.apiBroker.DeleteDecisionByIdAsync(createdDecision.Id);
            }
        }
    }
}
