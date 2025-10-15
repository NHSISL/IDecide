// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Core.Models.Orchestrations.Consumers.Exceptions;
using Moq;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Orchestrations.Consumers
{
    public partial class ConsumerOrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldThrowInvalidDecisionIdsExceptionOnRecordConsumerAdoptionIfIdsAreEmptyAndLogItAsync()
        {
            List<Guid> emptyDecisionIds = new List<Guid>();

            var invalidDecisionIdsException =
                new InvalidDecisionIdsException(message: "Decision Ids required.");

            var expectedConsumerOrchestrationValidationException =
                new ConsumerOrchestrationValidationException(
                    message: "Consumer orchestration validation error occurred, please fix the errors and try again.",
                    innerException: invalidDecisionIdsException);

            // when
            ValueTask recordConsumerAdoptionTask =
                this.consumerOrchestrationService.RecordConsumerAdoptionAsync(emptyDecisionIds);

            ConsumerOrchestrationValidationException actualConsumerOrchestrationValidationException =
                await Assert.ThrowsAsync<ConsumerOrchestrationValidationException>(() =>
                    recordConsumerAdoptionTask.AsTask());

            // then
            actualConsumerOrchestrationValidationException.Should()
                .BeEquivalentTo(expectedConsumerOrchestrationValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedConsumerOrchestrationValidationException))),
                        Times.Once);

            this.securityBrokerMock.VerifyNoOtherCalls();
            this.consumerServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
            this.consumerAdoptionServiceMock.VerifyNoOtherCalls();
        }
    }
}
