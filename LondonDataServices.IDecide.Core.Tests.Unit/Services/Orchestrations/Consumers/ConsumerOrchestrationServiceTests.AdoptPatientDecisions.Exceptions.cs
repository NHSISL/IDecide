// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Core.Models.Foundations.Decisions;
using LondonDataServices.IDecide.Core.Models.Orchestrations.Consumers.Exceptions;
using Moq;
using Xeptions;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Orchestrations.Consumers
{
    public partial class ConsumerOrchestrationServiceTests
    {
        [Theory]
        [MemberData(nameof(DependencyValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnAdoptPatientDecisionsAndLogItAsync(
            Xeption dependencyValidationException)
        {
            List<Decision> randomDecisions = CreateRandomDecisions();
            List<Decision> inputDecisions = randomDecisions;

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ThrowsAsync(dependencyValidationException);

            var expectedConsumerOrchestrationDependencyValidationException =
                new ConsumerOrchestrationDependencyValidationException(
                    message: "Consumer orchestration dependency validation error occurred, " +
                        "please fix the errors and try again.",
                    innerException: dependencyValidationException);

            // when
            ValueTask adoptPatientDecisionsTask =
                this.consumerOrchestrationService.AdoptPatientDecisionsAsync(inputDecisions);

            ConsumerOrchestrationDependencyValidationException
                actualConsumerOrchestrationDependencyValidationException =
                    await Assert.ThrowsAsync<ConsumerOrchestrationDependencyValidationException>(() =>
                        adoptPatientDecisionsTask.AsTask());

            // then
            actualConsumerOrchestrationDependencyValidationException
                .Should().BeEquivalentTo(expectedConsumerOrchestrationDependencyValidationException);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
            this.consumerServiceMock.VerifyNoOtherCalls();
            this.consumerAdoptionServiceMock.VerifyNoOtherCalls();
            this.patientServiceMock.VerifyNoOtherCalls();
            this.notificationServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnAdoptPatientDecisionsAndLogItAsync(
            Xeption dependencyException)
        {
            List<Decision> randomDecisions = CreateRandomDecisions();
            List<Decision> inputDecisions = randomDecisions;

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ThrowsAsync(dependencyException);

            var expectedConsumerOrchestrationDependencyException =
                new ConsumerOrchestrationDependencyException(
                    message: "Consumer orchestration dependency error occurred, " +
                        "please fix the errors and try again.",
                    innerException: dependencyException);

            // when
            ValueTask adoptPatientDecisionsTask =
                this.consumerOrchestrationService.AdoptPatientDecisionsAsync(inputDecisions);

            ConsumerOrchestrationDependencyException
                actualConsumerOrchestrationDependencyException =
                    await Assert.ThrowsAsync<ConsumerOrchestrationDependencyException>(() =>
                        adoptPatientDecisionsTask.AsTask());

            // then
            actualConsumerOrchestrationDependencyException
                .Should().BeEquivalentTo(expectedConsumerOrchestrationDependencyException);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
            this.consumerServiceMock.VerifyNoOtherCalls();
            this.consumerAdoptionServiceMock.VerifyNoOtherCalls();
            this.patientServiceMock.VerifyNoOtherCalls();
            this.notificationServiceMock.VerifyNoOtherCalls();
        }


        [Fact]
        public async Task ShouldThrowServiceExceptionOnAdoptPatientDecisionsIfServiceErrorOccursAndLogItAsync()
        {
            // given
            List<Decision> randomDecisions = CreateRandomDecisions();
            List<Decision> inputDecisions = randomDecisions;
            var serviceException = new Exception();

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ThrowsAsync(serviceException);

            var failedConsumerOrchestrationServiceException =
                new FailedConsumerOrchestrationServiceException(
                    message: "Failed consumer orchestration service error occurred, contact support.",
                    innerException: serviceException);

            var expectedDecisionOrchestrationServiceException =
                new ConsumerOrchestrationServiceException(
                    message: "Consumer orchestration service error occurred, contact support.",
                    innerException: failedConsumerOrchestrationServiceException);

            // when
            ValueTask adoptPatientDecisionsTask =
                this.consumerOrchestrationService.AdoptPatientDecisionsAsync(inputDecisions);

            ConsumerOrchestrationServiceException
                actualConsumerOrchestrationServiceException =
                    await Assert.ThrowsAsync<ConsumerOrchestrationServiceException>(() =>
                        adoptPatientDecisionsTask.AsTask());

            // then
            actualConsumerOrchestrationServiceException
                .Should().BeEquivalentTo(expectedDecisionOrchestrationServiceException);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
            this.consumerServiceMock.VerifyNoOtherCalls();
            this.consumerAdoptionServiceMock.VerifyNoOtherCalls();
            this.patientServiceMock.VerifyNoOtherCalls();
            this.notificationServiceMock.VerifyNoOtherCalls();
        }
    }
}
