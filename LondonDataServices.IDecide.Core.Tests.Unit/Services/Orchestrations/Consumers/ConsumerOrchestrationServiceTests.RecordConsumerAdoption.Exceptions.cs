// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Core.Models.Orchestrations.Consumers.Exceptions;
using Moq;
using Xeptions;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Orchestrations.Consumers
{
    public partial class ConsumerOrchestrationServiceTests
    {
        [Theory]
        [MemberData(nameof(DependencyValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnRecordConsumerAdoptionAndLogItAsync(
            Xeption dependencyValidationException)
        {
            List<Guid> randomDecisionIds = CreateRandomDecisionIds();
            List<Guid> inputDecisionIds = randomDecisionIds;

            this.securityBrokerMock.Setup(broker =>
                broker.IsCurrentUserAuthenticatedAsync())
                    .ThrowsAsync(dependencyValidationException);

            var expectedConsumerOrchestrationDependencyValidationException =
                new ConsumerOrchestrationDependencyValidationException(
                    message: "Consumer orchestration dependency validation error occurred, " +
                             "please fix the errors and try again.",
                    innerException: dependencyValidationException);

            // when
            ValueTask adoptPatientDecisionsTask =
                this.consumerOrchestrationService.RecordConsumerAdoptionAsync(inputDecisionIds);

            ConsumerOrchestrationDependencyValidationException
                actualConsumerOrchestrationDependencyValidationException =
                    await Assert.ThrowsAsync<ConsumerOrchestrationDependencyValidationException>(() =>
                        adoptPatientDecisionsTask.AsTask());

            // then
            actualConsumerOrchestrationDependencyValidationException
                .Should().BeEquivalentTo(expectedConsumerOrchestrationDependencyValidationException);

            this.securityBrokerMock.Verify(broker =>
                broker.IsCurrentUserAuthenticatedAsync(),
                    Times.Once);

            this.securityBrokerMock.VerifyNoOtherCalls();
            this.consumerServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
            this.consumerAdoptionServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnRecordConsumerAdoptionAndLogItAsync(
            Xeption dependencyException)
        {
            List<Guid> randomDecisionIds = CreateRandomDecisionIds();
            List<Guid> inputDecisionIds = randomDecisionIds;

            this.securityBrokerMock.Setup(broker =>
                broker.IsCurrentUserAuthenticatedAsync())
                    .ThrowsAsync(dependencyException);

            var expectedConsumerOrchestrationDependencyException =
                new ConsumerOrchestrationDependencyException(
                    message: "Consumer orchestration dependency error occurred, " +
                        "please fix the errors and try again.",
                    innerException: dependencyException);

            // when
            ValueTask adoptPatientDecisionsTask =
                this.consumerOrchestrationService.RecordConsumerAdoptionAsync(inputDecisionIds);

            ConsumerOrchestrationDependencyException
                actualConsumerOrchestrationDependencyException =
                    await Assert.ThrowsAsync<ConsumerOrchestrationDependencyException>(() =>
                        adoptPatientDecisionsTask.AsTask());

            // then
            actualConsumerOrchestrationDependencyException
                .Should().BeEquivalentTo(expectedConsumerOrchestrationDependencyException);

            this.securityBrokerMock.Verify(broker =>
                broker.IsCurrentUserAuthenticatedAsync(),
                    Times.Once);

            this.securityBrokerMock.VerifyNoOtherCalls();
            this.consumerServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
            this.consumerAdoptionServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRecordConsumerAdoptionIfServiceErrorOccursAndLogItAsync()
        {
            // given
            List<Guid> randomDecisionIds = CreateRandomDecisionIds();
            List<Guid> inputDecisionIds = randomDecisionIds;
            var serviceException = new Exception();

            this.securityBrokerMock.Setup(broker =>
                broker.IsCurrentUserAuthenticatedAsync())
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
                this.consumerOrchestrationService.RecordConsumerAdoptionAsync(inputDecisionIds);

            ConsumerOrchestrationServiceException
                actualConsumerOrchestrationServiceException =
                    await Assert.ThrowsAsync<ConsumerOrchestrationServiceException>(() =>
                        adoptPatientDecisionsTask.AsTask());

            // then
            actualConsumerOrchestrationServiceException
                .Should().BeEquivalentTo(expectedDecisionOrchestrationServiceException);

            this.securityBrokerMock.Verify(broker =>
                broker.IsCurrentUserAuthenticatedAsync(),
                    Times.Once);

            this.securityBrokerMock.VerifyNoOtherCalls();
            this.consumerServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
            this.consumerAdoptionServiceMock.VerifyNoOtherCalls();
        }
    }
}
