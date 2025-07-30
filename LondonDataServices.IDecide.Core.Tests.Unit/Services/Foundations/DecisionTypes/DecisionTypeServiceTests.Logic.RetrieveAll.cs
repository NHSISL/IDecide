// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Core.Models.Foundations.DecisionTypes;
using Moq;

namespace StandardlyTestProject.Api.Tests.Unit.Services.Foundations.DecisionTypes
{
    public partial class DecisionTypeServiceTests
    {
        [Fact]
        public async Task ShouldReturnDecisionTypes()
        {
            // given
            IQueryable<DecisionType> randomDecisionTypes = CreateRandomDecisionTypes();
            IQueryable<DecisionType> storageDecisionTypes = randomDecisionTypes;
            IQueryable<DecisionType> expectedDecisionTypes = storageDecisionTypes;

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllDecisionTypesAsync())
                    .ReturnsAsync(storageDecisionTypes);

            // when
            IQueryable<DecisionType> actualDecisionTypes =
                await this.decisionTypeService.RetrieveAllDecisionTypesAsync();

            // then
            actualDecisionTypes.Should().BeEquivalentTo(expectedDecisionTypes);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllDecisionTypesAsync(),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}