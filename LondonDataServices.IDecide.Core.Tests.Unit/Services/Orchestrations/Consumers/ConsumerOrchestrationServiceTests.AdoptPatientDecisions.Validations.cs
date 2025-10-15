// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Core.Models.Foundations.Decisions;
using LondonDataServices.IDecide.Core.Models.Orchestrations.Consumers.Exceptions;
using Moq;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Orchestrations.Consumers
{
    public partial class ConsumerOrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldThrowInvalidDecisionsExceptionOnAdoptPatientDecisionsIfDecisionsIsEmptyAndLogItAsync()
        {
            List<Decision> emptyDecisions = new List<Decision>();

            var invalidDecisionsException =
                new InvalidDecisionsException(message: "Decisions required.");

            var expectedConsumerOrchestrationValidationException =
                new ConsumerOrchestrationValidationException(
                    message: "Consumer orchestration validation error occurred, please fix the errors and try again.",
                    innerException: invalidDecisionsException);

            // when
            ValueTask adoptPatientDecisionsTask =
                this.consumerOrchestrationService.AdoptPatientDecisionsAsync(emptyDecisions);

            ConsumerOrchestrationValidationException actualConsumerOrchestrationValidationException =
                await Assert.ThrowsAsync<ConsumerOrchestrationValidationException>(() =>
                    adoptPatientDecisionsTask.AsTask());

            // then
            actualConsumerOrchestrationValidationException.Should()
                .BeEquivalentTo(expectedConsumerOrchestrationValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedConsumerOrchestrationValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.consumerServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
            this.consumerAdoptionServiceMock.VerifyNoOtherCalls();
            this.patientServiceMock.VerifyNoOtherCalls();
            this.notificationServiceMock.VerifyNoOtherCalls();
        }
    }
}
