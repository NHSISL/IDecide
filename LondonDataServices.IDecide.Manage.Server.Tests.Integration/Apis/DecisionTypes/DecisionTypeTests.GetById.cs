// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Manage.Server.Tests.Integration.Models.DecisionTypes;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Integration.Apis
{
    public partial class DecisionTypeApiTests
    {
        [Fact]
        public async Task ShouldGetDecisionTypeByIdAsync()
        {
            // given
            DecisionType randomDecisionType = await PostRandomDecisionTypeAsync();
            DecisionType expectedDecisionType = randomDecisionType;

            // when
            DecisionType actualDecisionType = 
                await this.apiBroker.GetDecisionTypeByIdAsync(randomDecisionType.Id);

            // then
            actualDecisionType.Should().BeEquivalentTo(expectedDecisionType);
            await this.apiBroker.DeleteDecisionTypeByIdAsync(actualDecisionType.Id);
        }
    }
}
