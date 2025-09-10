// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Core.Models.Foundations.ConsumerStatuses;
using LondonDataServices.IDecide.Core.Models.Foundations.ConsumerStatuses.Exceptions;
using Moq;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.ConsumerStatuses
{
    public partial class ConsumerStatusServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRemoveIfIdIsInvalidAndLogItAsync()
        {
            // given
            Guid invalidConsumerStatusId = Guid.Empty;

            var invalidConsumerStatusException =
                new InvalidConsumerStatusException(
                    message: "Invalid consumerStatus. Please correct the errors and try again.");

            invalidConsumerStatusException.AddData(
                key: nameof(ConsumerStatus.Id),
                values: "Id is required");

            var expectedConsumerStatusValidationException =
                new ConsumerStatusValidationException(
                    message: "ConsumerStatus validation errors occurred, please try again.",
                    innerException: invalidConsumerStatusException);

            // when
            ValueTask<ConsumerStatus> removeConsumerStatusByIdTask =
                this.consumerStatusService.RemoveConsumerStatusByIdAsync(invalidConsumerStatusId);

            ConsumerStatusValidationException actualConsumerStatusValidationException =
                await Assert.ThrowsAsync<ConsumerStatusValidationException>(
                    removeConsumerStatusByIdTask.AsTask);

            // then
            actualConsumerStatusValidationException.Should()
                .BeEquivalentTo(expectedConsumerStatusValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedConsumerStatusValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteConsumerStatusAsync(It.IsAny<ConsumerStatus>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowNotFoundExceptionOnRemoveByIdIfConsumerStatusIsNotFoundAndLogItAsync()
        {
            //given
            Guid someConsumerStatusId = Guid.NewGuid();
            ConsumerStatus noConsumerStatus = null;

            var notFoundConsumerStatusException = new NotFoundConsumerStatusException(
                $"Couldn't find consumerStatus with consumerStatusId: {someConsumerStatusId}.");

            var expectedConsumerStatusValidationException =
                new ConsumerStatusValidationException(
                    message: "ConsumerStatus validation errors occurred, please try again.",
                    innerException: notFoundConsumerStatusException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectConsumerStatusByIdAsync(It.IsAny<Guid>()))
                    .ReturnsAsync(noConsumerStatus);

            //when
            ValueTask<ConsumerStatus> retrieveConsumerStatusByIdTask =
                this.consumerStatusService.RemoveConsumerStatusByIdAsync(someConsumerStatusId);

            ConsumerStatusValidationException actualConsumerStatusValidationException =
                await Assert.ThrowsAsync<ConsumerStatusValidationException>(
                    retrieveConsumerStatusByIdTask.AsTask);

            //then
            actualConsumerStatusValidationException.Should().BeEquivalentTo(expectedConsumerStatusValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectConsumerStatusByIdAsync(It.IsAny<Guid>()),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedConsumerStatusValidationException))),
                        Times.Once());

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
