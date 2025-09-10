// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Portal.Server.Tests.Integration.Models.DecisionTypes;

namespace LondonDataServices.IDecide.Portal.Server.Tests.Integration.Apis.DecisionTypes
{
    public partial class DecisionTypeApiTests
    {
        [Fact]
        public async Task ShouldPutDecisionTypeAsync()
        {
            // given
            DecisionType randomDecisionType = await PostRandomDecisionTypeAsync();
            DecisionType modifiedDecisionType = UpdateDecisionTypeWithRandomValues(randomDecisionType);

            // when
            await this.apiBroker.PutDecisionTypeAsync(modifiedDecisionType);
            DecisionType actualDecisionType = await this.apiBroker.GetDecisionTypeByIdAsync(randomDecisionType.Id);

            // then
            actualDecisionType.Should().BeEquivalentTo(
                modifiedDecisionType,
                options => options
                    .Excluding(decisionType => decisionType.CreatedBy)
                    .Excluding(decisionType => decisionType.CreatedDate)
                    .Excluding(decisionType => decisionType.UpdatedBy)
                    .Excluding(decisionType => decisionType.UpdatedDate));

            await this.apiBroker.DeleteDecisionTypeByIdAsync(actualDecisionType.Id);
        }
    }
}
