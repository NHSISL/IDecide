// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Core.Models.Foundations.ConsumerStatuses;
using LondonDataServices.IDecide.Core.Models.Foundations.ConsumerStatuses.Exceptions;
using LondonDataServices.IDecide.Core.Models.Securities;
using Moq;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.ConsumerStatuses
{
    public partial class ConsumerStatusServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfConsumerStatusIsNullAndLogItAsync()
        {
            // given
            ConsumerStatus nullConsumerStatus = null;
            var nullConsumerStatusException = new NullConsumerStatusException(message: "ConsumerStatus is null.");

            var expectedConsumerStatusValidationException =
                new ConsumerStatusValidationException(
                    message: "ConsumerStatus validation errors occurred, please try again.",
                    innerException: nullConsumerStatusException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyModifyAuditValuesAsync(nullConsumerStatus))
                    .ReturnsAsync(nullConsumerStatus);

            // when
            ValueTask<ConsumerStatus> modifyConsumerStatusTask =
                this.consumerStatusService.ModifyConsumerStatusAsync(nullConsumerStatus);

            ConsumerStatusValidationException actualConsumerStatusValidationException =
                await Assert.ThrowsAsync<ConsumerStatusValidationException>(
                    modifyConsumerStatusTask.AsTask);

            // then
            actualConsumerStatusValidationException.Should()
                .BeEquivalentTo(expectedConsumerStatusValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyModifyAuditValuesAsync(nullConsumerStatus),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedConsumerStatusValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateConsumerStatusAsync(It.IsAny<ConsumerStatus>()),
                    Times.Never);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnModifyIfConsumerStatusIsInvalidAndLogItAsync(
            string invalidText)
        {
            // given 
            string randomUserId = GetRandomString();
            User randomUser = CreateRandomUser(userId: randomUserId);

            var invalidConsumerStatus = new ConsumerStatus();

            var invalidConsumerStatusException =
                new InvalidConsumerStatusException(
                    message: "Invalid consumerStatus. Please correct the errors and try again.");

            invalidConsumerStatusException.AddData(
                key: nameof(ConsumerStatus.Id),
                values: "Id is required");

            invalidConsumerStatusException.AddData(
                key: nameof(ConsumerStatus.CreatedDate),
                values: "Date is required");

            invalidConsumerStatusException.AddData(
                key: nameof(ConsumerStatus.CreatedBy),
                values: "Text is required");

            invalidConsumerStatusException.AddData(
                key: nameof(ConsumerStatus.UpdatedDate),
                values:
                [
                    "Date is required",
                    $"Date is the same as {nameof(ConsumerStatus.CreatedDate)}"
                ]);

            invalidConsumerStatusException.AddData(
                key: nameof(ConsumerStatus.UpdatedBy),
                values:
                [
                    "Text is required",
                    $"Expected value to be '{randomUserId}' but found '{invalidConsumerStatus.UpdatedBy}'."
                ]);

            var expectedConsumerStatusValidationException =
                new ConsumerStatusValidationException(
                    message: "ConsumerStatus validation errors occurred, please try again.",
                    innerException: invalidConsumerStatusException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyModifyAuditValuesAsync(invalidConsumerStatus))
                    .ReturnsAsync(invalidConsumerStatus);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomUser);

            // when
            ValueTask<ConsumerStatus> modifyConsumerStatusTask =
                this.consumerStatusService.ModifyConsumerStatusAsync(invalidConsumerStatus);

            ConsumerStatusValidationException actualConsumerStatusValidationException =
                await Assert.ThrowsAsync<ConsumerStatusValidationException>(
                    modifyConsumerStatusTask.AsTask);

            //then
            actualConsumerStatusValidationException.Should()
                .BeEquivalentTo(expectedConsumerStatusValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyModifyAuditValuesAsync(invalidConsumerStatus),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedConsumerStatusValidationException))),
                        Times.Once());

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateConsumerStatusAsync(It.IsAny<ConsumerStatus>()),
                    Times.Never);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdatedDateIsSameAsCreatedDateAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            string randomUserId = GetRandomString();
            User randomUser = CreateRandomUser(userId: randomUserId);

            ConsumerStatus randomConsumerStatus =
                CreateRandomConsumerStatus(dateTimeOffset: randomDateTimeOffset, userId: randomUserId);

            ConsumerStatus invalidConsumerStatus = randomConsumerStatus;

            var invalidConsumerStatusException =
                new InvalidConsumerStatusException(
                    message: "Invalid consumerStatus. Please correct the errors and try again.");

            invalidConsumerStatusException.AddData(
                key: nameof(ConsumerStatus.UpdatedDate),
                values: $"Date is the same as {nameof(ConsumerStatus.CreatedDate)}");

            var expectedConsumerStatusValidationException =
                new ConsumerStatusValidationException(
                    message: "ConsumerStatus validation errors occurred, please try again.",
                    innerException: invalidConsumerStatusException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyModifyAuditValuesAsync(invalidConsumerStatus))
                    .ReturnsAsync(invalidConsumerStatus);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomUser);

            // when
            ValueTask<ConsumerStatus> modifyConsumerStatusTask =
                this.consumerStatusService.ModifyConsumerStatusAsync(invalidConsumerStatus);

            ConsumerStatusValidationException actualConsumerStatusValidationException =
                await Assert.ThrowsAsync<ConsumerStatusValidationException>(
                    modifyConsumerStatusTask.AsTask);

            // then
            actualConsumerStatusValidationException.Should()
                .BeEquivalentTo(expectedConsumerStatusValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyModifyAuditValuesAsync(invalidConsumerStatus),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedConsumerStatusValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectConsumerStatusByIdAsync(invalidConsumerStatus.Id),
                    Times.Never);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(MinutesBeforeOrAfter))]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdatedDateIsNotRecentAndLogItAsync(int minutes)
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            DateTimeOffset invalidDate = randomDateTimeOffset.AddMinutes(minutes);
            DateTimeOffset startDate = randomDateTimeOffset.AddSeconds(-90);
            DateTimeOffset endDate = randomDateTimeOffset.AddSeconds(0);
            string randomUserId = GetRandomString();
            User randomUser = CreateRandomUser(userId: randomUserId);

            ConsumerStatus randomConsumerStatus =
                CreateRandomConsumerStatus(dateTimeOffset: randomDateTimeOffset, userId: randomUserId);

            randomConsumerStatus.UpdatedDate = randomDateTimeOffset.AddMinutes(minutes);

            var invalidConsumerStatusException =
                new InvalidConsumerStatusException(
                    message: "Invalid consumerStatus. Please correct the errors and try again.");

            invalidConsumerStatusException.AddData(
                key: nameof(ConsumerStatus.UpdatedDate),
                values:
                    $"Date is not recent. Expected a value between {startDate} and {endDate} but found {invalidDate}");

            var expectedConsumerStatusValidatonException =
                new ConsumerStatusValidationException(
                    message: "ConsumerStatus validation errors occurred, please try again.",
                    innerException: invalidConsumerStatusException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyModifyAuditValuesAsync(randomConsumerStatus))
                    .ReturnsAsync(randomConsumerStatus);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomUser);

            // when
            ValueTask<ConsumerStatus> modifyConsumerStatusTask =
                this.consumerStatusService.ModifyConsumerStatusAsync(randomConsumerStatus);

            ConsumerStatusValidationException actualConsumerStatusValidationException =
                await Assert.ThrowsAsync<ConsumerStatusValidationException>(
                    modifyConsumerStatusTask.AsTask);

            // then
            actualConsumerStatusValidationException.Should()
                .BeEquivalentTo(expectedConsumerStatusValidatonException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyModifyAuditValuesAsync(randomConsumerStatus),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedConsumerStatusValidatonException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectConsumerStatusByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfConsumerStatusDoesNotExistAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            string randomUserId = GetRandomString();
            User randomUser = CreateRandomUser(userId: randomUserId);

            ConsumerStatus randomConsumerStatus = CreateRandomModifyConsumerStatus(
                dateTimeOffset: randomDateTimeOffset, userId: randomUserId);

            ConsumerStatus nonExistConsumerStatus = randomConsumerStatus;
            ConsumerStatus nullConsumerStatus = null;

            var notFoundConsumerStatusException = new NotFoundConsumerStatusException(
                message: $"Couldn't find consumerStatus with consumerStatusId: {nonExistConsumerStatus.Id}.");

            var expectedConsumerStatusValidationException =
                new ConsumerStatusValidationException(
                    message: "ConsumerStatus validation errors occurred, please try again.",
                    innerException: notFoundConsumerStatusException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyModifyAuditValuesAsync(nonExistConsumerStatus))
                    .ReturnsAsync(nonExistConsumerStatus);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectConsumerStatusByIdAsync(nonExistConsumerStatus.Id))
                    .ReturnsAsync(nullConsumerStatus);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomUser);

            // when 
            ValueTask<ConsumerStatus> modifyConsumerStatusTask =
                this.consumerStatusService.ModifyConsumerStatusAsync(nonExistConsumerStatus);

            ConsumerStatusValidationException actualConsumerStatusValidationException =
                await Assert.ThrowsAsync<ConsumerStatusValidationException>(
                    modifyConsumerStatusTask.AsTask);

            // then
            actualConsumerStatusValidationException.Should()
                .BeEquivalentTo(expectedConsumerStatusValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyModifyAuditValuesAsync(nonExistConsumerStatus),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectConsumerStatusByIdAsync(nonExistConsumerStatus.Id),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedConsumerStatusValidationException))),
                        Times.Once);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
