// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Core.Models.Foundations.Decisions;
using Moq;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.Decisions
{
    public partial class DecisionServiceTests
    {
        [Fact]
        public async Task ShouldReturnDecisions()
        {
            // given
            IQueryable<Decision> randomDecisions = CreateRandomDecisions();
            IQueryable<Decision> storageDecisions = randomDecisions;
            IQueryable<Decision> expectedDecisions = storageDecisions;

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllDecisionsAsync())
                    .ReturnsAsync(storageDecisions);

            // when
            IQueryable<Decision> actualDecisions =
                await this.decisionService.RetrieveAllDecisionsAsync();

            // then
            actualDecisions.Should().BeEquivalentTo(expectedDecisions);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllDecisionsAsync(),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}