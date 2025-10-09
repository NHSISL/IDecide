// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Core.Models.Foundations.Consumers;
using LondonDataServices.IDecide.Core.Models.Foundations.Decisions;
using LondonDataServices.IDecide.Core.Models.Securities;
using Moq;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Orchestrations.Decisions
{
    public partial class DecisionOrchestrationServiceTests
    {
        [Fact]
        public async Task
            ShouldRetrieveAllPendingAdoptionDecisionsForConsumerForDefaultChangesSinceDateAndDecisionTypeParamsAsync()
        {
            // given
            DateTimeOffset changesSinceDate = default;
            string decisionType = null;
            User randomUser = CreateRandomUser();
            IQueryable<Consumer> consumers = CreateRandomConsumersWithMatchingEntraIdEntry(randomUser.UserId);
            IQueryable<Decision> decisions = CreateRandomDecisions();
            List<Decision> expectedPendingAdoptionDecisions = decisions.ToList();

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomUser);

            this.consumerServiceMock.Setup(service =>
                service.RetrieveAllConsumersAsync())
                    .ReturnsAsync(consumers);

            this.decisionServiceMock.Setup(service =>
                service.RetrieveAllDecisionsAsync())
                    .ReturnsAsync(decisions);

            // when
            List<Decision> actualPendingAdoptionDecisions =
                await this.decisionOrchestrationService.RetrieveAllPendingAdoptionDecisionsForConsumer(
                    changesSinceDate, decisionType);

            // then
            actualPendingAdoptionDecisions.Should().BeEquivalentTo(expectedPendingAdoptionDecisions);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.consumerServiceMock.Verify(service =>
                service.RetrieveAllConsumersAsync(),
                    Times.Once);

            this.decisionServiceMock.Verify(service =>
                service.RetrieveAllDecisionsAsync(),
                    Times.Once);

            this.securityBrokerMock.VerifyNoOtherCalls();
            this.consumerServiceMock.VerifyNoOtherCalls();
            this.decisionServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task
            ShouldRetrieveAllPendingAdoptionDecisionsForConsumerForPopulatedChangesSinceDateAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            DateTimeOffset changesSinceDate = randomDateTimeOffset.AddMinutes(-5);
            string decisionType = null;
            User randomUser = CreateRandomUser();
            IQueryable<Consumer> consumers = CreateRandomConsumersWithMatchingEntraIdEntry(randomUser.UserId);
            IQueryable<Decision> decisions = CreateRandomDecisions();
            List<Decision> allDecisions = decisions.ToList();
            allDecisions.First().CreatedDate = changesSinceDate.AddMinutes(-10);

            List<Decision> expectedPendingAdoptionDecisions =
                allDecisions.Where(d => d.CreatedDate > changesSinceDate).ToList();

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomUser);

            this.consumerServiceMock.Setup(service =>
                service.RetrieveAllConsumersAsync())
                    .ReturnsAsync(consumers);

            this.decisionServiceMock.Setup(service =>
                service.RetrieveAllDecisionsAsync())
                    .ReturnsAsync(allDecisions.AsQueryable());

            // when
            List<Decision> actualPendingAdoptionDecisions =
                await this.decisionOrchestrationService.RetrieveAllPendingAdoptionDecisionsForConsumer(
                    changesSinceDate, decisionType);

            // then
            actualPendingAdoptionDecisions.Should()
                .BeEquivalentTo(expectedPendingAdoptionDecisions);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.consumerServiceMock.Verify(service =>
                service.RetrieveAllConsumersAsync(),
                    Times.Once);

            this.decisionServiceMock.Verify(service =>
                service.RetrieveAllDecisionsAsync(),
                    Times.Once);

            this.securityBrokerMock.VerifyNoOtherCalls();
            this.consumerServiceMock.VerifyNoOtherCalls();
            this.decisionServiceMock.VerifyNoOtherCalls();
        }
    }
}
