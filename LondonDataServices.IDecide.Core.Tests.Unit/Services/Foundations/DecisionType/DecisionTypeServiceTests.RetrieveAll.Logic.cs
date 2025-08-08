// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Core.Models.Foundations.DecisionType;
using Moq;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.DecisionType
{
    public partial class DecisionTypeServiceTests
    {
        [Fact]
        public async Task ShouldReturnDecisionType()
        {
            // given
            IQueryable<DecisionType> randomDecisionType = CreateRandomDecisionType();
            IQueryable<DecisionType> storageDecisionType = randomDecisionType;
            IQueryable<DecisionType> expectedDecisionType = storageDecisionType;

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllDecisionTypeAsync())
                    .ReturnsAsync(storageDecisionType);

            // when
            IQueryable<DecisionType> actualDecisionType =
                await this.decisionTypeService.RetrieveAllDecisionTypeAsync();

            // then
            actualDecisionType.Should().BeEquivalentTo(expectedDecisionType);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllDecisionTypeAsync(),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}