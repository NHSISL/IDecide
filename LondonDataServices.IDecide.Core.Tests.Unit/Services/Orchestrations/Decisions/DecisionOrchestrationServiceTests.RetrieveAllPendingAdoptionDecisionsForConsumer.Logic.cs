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
using LondonDataServices.IDecide.Core.Models.Foundations.DecisionTypes;
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
            Guid consumerId = consumers.First().Id;

            List<Decision> expectedPendingAdoptionDecisions = decisions
                .Where(decision =>
                    !decision.ConsumerAdoptions.Any(consumerAdoption => consumerAdoption.ConsumerId == consumerId)
                    || (
                        decision.ConsumerAdoptions.Any(consumerAdoption => consumerAdoption.ConsumerId == consumerId)
                        && decision.ConsumerAdoptions
                            .Where(consumerAdoption => consumerAdoption.ConsumerId == consumerId)
                            .Max(consumerAdoption => consumerAdoption.CreatedDate) < decision.CreatedDate
                    )
                )
                .ToList();

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
        public async Task ShouldRetrieveAllPendingAdoptionDecisionsForConsumerForPopulatedChangesSinceDateAsync()
        {
            // given
            DateTimeOffset nowDateTimeOffset = DateTimeOffset.Now;
            DateTimeOffset changesSinceDate = nowDateTimeOffset.AddMinutes(-5);
            string decisionType = null;
            User randomUser = CreateRandomUser();
            IQueryable<Consumer> consumers = CreateRandomConsumersWithMatchingEntraIdEntry(randomUser.UserId);
            IQueryable<Decision> decisions = CreateRandomDecisions();
            Guid consumerId = consumers.First().Id;
            List<Decision> allDecisions = decisions.ToList();
            allDecisions.First().CreatedDate = changesSinceDate.AddMinutes(-10);

            List<Decision> expectedPendingAdoptionDecisions = allDecisions
                .Where(decision => decision.CreatedDate > changesSinceDate)
                .Where(decision =>
                    !decision.ConsumerAdoptions.Any(consumerAdoption => consumerAdoption.ConsumerId == consumerId)
                    || (
                        decision.ConsumerAdoptions.Any(consumerAdoption => consumerAdoption.ConsumerId == consumerId)
                        && decision.ConsumerAdoptions
                            .Where(consumerAdoption => consumerAdoption.ConsumerId == consumerId)
                            .Max(consumerAdoption => consumerAdoption.CreatedDate) < decision.CreatedDate
                    )
                )
                .ToList();

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

        [Fact]
        public async Task
            ShouldRetrieveAllPendingAdoptionDecisionsForConsumerForPopulatedChangesSinceDateAndDecisionTypeAsync()
        {
            // given
            DateTimeOffset nowDateTimeOffset = DateTimeOffset.Now;
            DateTimeOffset changesSinceDate = nowDateTimeOffset.AddMinutes(-5);
            string decisionType = GetRandomString();
            User randomUser = CreateRandomUser();
            IQueryable<Consumer> consumers = CreateRandomConsumersWithMatchingEntraIdEntry(randomUser.UserId);
            List<Decision> decisions = CreateRandomDecisions().ToList();
            Guid consumerId = consumers.First().Id;
            Decision excludedDecisionByDate = new Decision();

            excludedDecisionByDate.DecisionType = new DecisionType
            {
                Id = Guid.NewGuid(),
                Name = decisionType,
                CreatedBy = decisions.First().CreatedBy,
                UpdatedBy = decisions.First().UpdatedBy,
                UpdatedDate = decisions.First().UpdatedDate
            };

            excludedDecisionByDate.CreatedDate = changesSinceDate.AddMinutes(-10);
            decisions.Add(excludedDecisionByDate);
            Decision excludedDecisionByDecisionType = new Decision();

            excludedDecisionByDecisionType.DecisionType = new DecisionType
            {
                Id = Guid.NewGuid(),
                Name = GetRandomString(),
                CreatedBy = decisions.First().CreatedBy,
                UpdatedBy = decisions.First().UpdatedBy,
                UpdatedDate = decisions.First().UpdatedDate
            };

            excludedDecisionByDecisionType.CreatedDate = changesSinceDate.AddMinutes(2);
            decisions.Add(excludedDecisionByDecisionType);
            Decision includedDecision = new Decision();

            includedDecision.DecisionType = new DecisionType
            {
                Id = Guid.NewGuid(),
                Name = decisionType,
                CreatedBy = decisions.First().CreatedBy,
                UpdatedBy = decisions.First().UpdatedBy,
                UpdatedDate = decisions.First().UpdatedDate
            };

            includedDecision.CreatedDate = changesSinceDate.AddMinutes(1);
            decisions.Add(includedDecision);

            List<Decision> expectedPendingAdoptionDecisions = decisions
                .Where(decision =>
                    decision.CreatedDate > changesSinceDate &&
                    decision.DecisionType?.Name == decisionType)
                .Where(decision =>
                    !decision.ConsumerAdoptions.Any(consumerAdoption => consumerAdoption.ConsumerId == consumerId)
                    || (
                        decision.ConsumerAdoptions.Any(consumerAdoption => consumerAdoption.ConsumerId == consumerId)
                        && decision.ConsumerAdoptions
                            .Where(consumerAdoption => consumerAdoption.ConsumerId == consumerId)
                            .Max(consumerAdoption => consumerAdoption.CreatedDate) < decision.CreatedDate
                    )
                )
                .ToList();

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomUser);

            this.consumerServiceMock.Setup(service =>
                service.RetrieveAllConsumersAsync())
                    .ReturnsAsync(consumers);

            this.decisionServiceMock.Setup(service =>
                service.RetrieveAllDecisionsAsync())
                    .ReturnsAsync(decisions.AsQueryable());

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
