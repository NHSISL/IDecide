// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Core.Models.Foundations.Consumers;
using LondonDataServices.IDecide.Core.Models.Foundations.Consumers.Exceptions;
using Moq;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.Consumers
{
    public partial class ConsumerServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRemoveIfIdIsInvalidAndLogItAsync()
        {
            // given
            Guid invalidConsumerId = Guid.Empty;

            var invalidConsumerException =
                new InvalidConsumerException(
                    message: "Invalid consumer. Please correct the errors and try again.");

            invalidConsumerException.AddData(
                key: nameof(Consumer.Id),
                values: "Id is required");

            var expectedConsumerValidationException =
                new ConsumerValidationException(
                    message: "Consumer validation errors occurred, please try again.",
                    innerException: invalidConsumerException);

            // when
            ValueTask<Consumer> removeConsumerByIdTask =
                this.consumerService.RemoveConsumerByIdAsync(invalidConsumerId);

            ConsumerValidationException actualConsumerValidationException =
                await Assert.ThrowsAsync<ConsumerValidationException>(
                    removeConsumerByIdTask.AsTask);

            // then
            actualConsumerValidationException.Should()
                .BeEquivalentTo(expectedConsumerValidationException);

            this.loggingBrokerMock.Verify(broker =>
                    broker.LogErrorAsync(It.Is(SameExceptionAs(
                        expectedConsumerValidationException))),
                Times.Once);

            this.storageBrokerMock.Verify(broker =>
                    broker.DeleteConsumerAsync(It.IsAny<Consumer>()),
                Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
