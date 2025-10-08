// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Core.Models.Foundations.Decisions;
using LondonDataServices.IDecide.Core.Models.Orchestrations.Consumers.Exceptions;
using Moq;
using Xeptions;
using Task = System.Threading.Tasks.Task;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Orchestrations.Consumers
{
    public partial class ConsumerOrchestrationServiceTests
    {
        [Theory]
        [MemberData(nameof(DependencyValidationExceptions))]
        public async Task ShouldThrowDependencyValidationOnAdoptPatientDecisionsAndLogItAsync(
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
                this.consumerOrchestrationService.AdoptPatientDecisions(inputDecisions);

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
            this.consumerServiceMock.VerifyNoOtherCalls();
            this.consumerAdoptionServiceMock.VerifyNoOtherCalls();
            this.patientServiceMock.VerifyNoOtherCalls();
            this.notificationServiceMock.VerifyNoOtherCalls();
        }
    }
}
