// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Manage.Server.Tests.Integration.Models.Decisions;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Integration.Apis
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
            actualDecision.Should().BeEquivalentTo(expectedDecision);
            await this.apiBroker.DeleteDecisionByIdAsync(actualDecision.Id);
        }
    }
}
