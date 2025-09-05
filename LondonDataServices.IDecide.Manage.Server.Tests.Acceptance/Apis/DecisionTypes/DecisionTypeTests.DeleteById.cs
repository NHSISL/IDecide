// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Models.DecisionTypes;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Apis.DecisionTypes
{
    public partial class DecisionTypeApiTests
    {
        [Fact]
        public async Task ShouldDeleteDecisionTypeByIdAsync()
        {
            // given
            DecisionType randomDecisionType = await PostRandomDecisionTypeAsync();
            DecisionType inputDecisionType = randomDecisionType;
            DecisionType expectedDecisionType = inputDecisionType;

            // when
            DecisionType deletedDecisionType =
                await this.apiBroker.DeleteDecisionTypeByIdAsync(inputDecisionType.Id);

            List<DecisionType> actualResult =
                await this.apiBroker.GetSpecificDecisionTypeByIdAsync(inputDecisionType.Id);

            // then
            actualResult.Count().Should().Be(0);
        }
    }
}