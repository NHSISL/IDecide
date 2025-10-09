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
using LondonDataServices.IDecide.Core.Models.Orchestrations.Decisions.Exceptions;
using LondonDataServices.IDecide.Core.Models.Securities;
using Moq;
using Xeptions;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Orchestrations.Decisions
{
    public partial class DecisionOrchestrationServiceTests
    {
        [Fact]
        public async Task
            ShouldThrowErrorOnRetrieveAllPendingAdoptionDecisionsIfUserIdNotFoundInConsumersEntraIdAndLogItAsync()
        {
            // given
            DateTimeOffset changesSinceDate = GetRandomDateTimeOffset();
            string decisionType = GetRandomString();
            User randomUser = CreateRandomUser();
            IQueryable<Consumer> consumers = CreateRandomConsumers();

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomUser);

            this.consumerServiceMock.Setup(service =>
                service.RetrieveAllConsumersAsync())
                    .ReturnsAsync(consumers);

            var expectedUnauthorizedDecisionOrchestrationServiceException =
                new UnauthorizedDecisionOrchestrationServiceException(
                    "The current user is not authorized to perform this operation.");

            // when
            ValueTask<List<Decision>> retrieveAllPendingAdoptionDecisionsForConsumerTask =
                this.decisionOrchestrationService.RetrieveAllPendingAdoptionDecisionsForConsumer(
                    changesSinceDate, decisionType);

            UnauthorizedDecisionOrchestrationServiceException
                actualUnauthorizedDecisionOrchestrationServiceException =
                    await Assert.ThrowsAsync<UnauthorizedDecisionOrchestrationServiceException>(() =>
                        retrieveAllPendingAdoptionDecisionsForConsumerTask.AsTask());

            // then
            actualUnauthorizedDecisionOrchestrationServiceException.Should()
                .BeEquivalentTo(expectedUnauthorizedDecisionOrchestrationServiceException);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.consumerServiceMock.Verify(service =>
                service.RetrieveAllConsumersAsync(),
                    Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.consumerServiceMock.VerifyNoOtherCalls();
            this.decisionServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(RetrieveAllPendingAdoptionDecisionsForConsumerDependencyValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnRetrieveAllPendingAdoptionDecisionsAndLogItAsync(
            Xeption dependencyValidationException)
        {
            // given
            DateTimeOffset changesSinceDate = GetRandomDateTimeOffset();
            string decisionType = GetRandomString();

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ThrowsAsync(dependencyValidationException);

            var expectedDecisionOrchestrationDependencyValidationException =
                new DecisionOrchestrationDependencyValidationException(
                    message: "Decision orchestration dependency validation error occurred, " +
                        "please fix the errors and try again.",
                    innerException: dependencyValidationException.InnerException as Xeption);

            // when
            ValueTask<List<Decision>> retrieveAllPendingAdoptionDecisionsForConsumerTask =
                this.decisionOrchestrationService.RetrieveAllPendingAdoptionDecisionsForConsumer(
                    changesSinceDate, decisionType);

            DecisionOrchestrationDependencyValidationException
                actualDecisionOrchestrationDependencyValidationException =
                    await Assert.ThrowsAsync<DecisionOrchestrationDependencyValidationException>(() =>
                        retrieveAllPendingAdoptionDecisionsForConsumerTask.AsTask());

            // then
            actualDecisionOrchestrationDependencyValidationException
                .Should().BeEquivalentTo(expectedDecisionOrchestrationDependencyValidationException);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedDecisionOrchestrationDependencyValidationException))),
                        Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.consumerServiceMock.VerifyNoOtherCalls();
            this.decisionServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(RetrieveAllPendingAdoptionDecisionsForConsumerDependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnRetrieveAllPendingAdoptionDecisionsAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            DateTimeOffset changesSinceDate = GetRandomDateTimeOffset();
            string decisionType = GetRandomString();

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ThrowsAsync(dependencyException);

            var expectedDecisionOrchestrationDependencyException =
                new DecisionOrchestrationDependencyException(
                    message: "Decision orchestration dependency error occurred, " +
                        "please fix the errors and try again.",
                    innerException: dependencyException.InnerException as Xeption);

            // when
            ValueTask<List<Decision>> retrieveAllPendingAdoptionDecisionsForConsumerTask =
                this.decisionOrchestrationService.RetrieveAllPendingAdoptionDecisionsForConsumer(
                    changesSinceDate, decisionType);

            DecisionOrchestrationDependencyException
                actualDecisionOrchestrationDependencyException =
                    await Assert.ThrowsAsync<DecisionOrchestrationDependencyException>(
                        testCode: retrieveAllPendingAdoptionDecisionsForConsumerTask.AsTask);

            // then
            actualDecisionOrchestrationDependencyException
                .Should().BeEquivalentTo(expectedDecisionOrchestrationDependencyException);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedDecisionOrchestrationDependencyException))),
                        Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.consumerServiceMock.VerifyNoOtherCalls();
            this.decisionServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveAllPendingAdoptionDecisionsAndLogItAsync()
        {
            // given
            DateTimeOffset changesSinceDate = GetRandomDateTimeOffset();
            string decisionType = GetRandomString();
            var serviceException = new Exception();

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ThrowsAsync(serviceException);

            var failedServiceDecisionOrchestrationException =
                new FailedDecisionOrchestrationServiceException(
                    message: "Failed decision orchestration service error occurred, contact support.",
                    innerException: serviceException);

            var expectedDecisionOrchestrationServiceException =
                new DecisionOrchestrationServiceException(
                    message: "Decision orchestration service error occurred, contact support.",
                    innerException: failedServiceDecisionOrchestrationException.InnerException as Xeption);

            // when
            ValueTask<List<Decision>> retrieveAllPendingAdoptionDecisionsForConsumerTask =
                this.decisionOrchestrationService.RetrieveAllPendingAdoptionDecisionsForConsumer(
                    changesSinceDate, decisionType);

            DecisionOrchestrationServiceException
                actualDecisionOrchestrationServiceException =
                    await Assert.ThrowsAsync<DecisionOrchestrationServiceException>(
                        testCode: retrieveAllPendingAdoptionDecisionsForConsumerTask.AsTask);

            // then
            actualDecisionOrchestrationServiceException
                .Should().BeEquivalentTo(expectedDecisionOrchestrationServiceException);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedDecisionOrchestrationServiceException))),
                        Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.consumerServiceMock.VerifyNoOtherCalls();
            this.decisionServiceMock.VerifyNoOtherCalls();
        }
    }
}
