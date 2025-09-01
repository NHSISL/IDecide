// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Portal.Server.Tests.Acceptance.Models.DecisionTypes;

namespace LondonDataServices.IDecide.Portal.Server.Tests.Acceptance.Apis
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
            actualDecisionType.Should().BeEquivalentTo(expectedDecisionType, options => options
                .Excluding(property => property.CreatedBy)
                .Excluding(property => property.CreatedDate)
                .Excluding(property => property.UpdatedBy)
                .Excluding(property => property.UpdatedDate));

            await this.apiBroker.DeleteDecisionTypeByIdAsync(actualDecisionType.Id);
        }
    }
}