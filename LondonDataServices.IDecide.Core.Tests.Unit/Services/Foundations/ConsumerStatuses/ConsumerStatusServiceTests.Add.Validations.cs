// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using LondonDataServices.IDecide.Core.Models.Foundations.ConsumerStatuses;
using LondonDataServices.IDecide.Core.Models.Foundations.ConsumerStatuses.Exceptions;
using LondonDataServices.IDecide.Core.Models.Securities;
using Moq;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.ConsumerStatuses
{
    public partial class ConsumerStatusServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfConsumerStatusIsNullAndLogItAsync()
        {
            // given
            ConsumerStatus nullConsumerStatus = null;

            var nullConsumerStatusException =
                new NullConsumerStatusException(message: "ConsumerStatus is null.");

            var expectedConsumerStatusValidationException =
                new ConsumerStatusValidationException(
                    message: "ConsumerStatus validation errors occurred, please try again.",
                    innerException: nullConsumerStatusException);

            this.securityAuditBrokerMock.Setup(broker =>
                    broker.ApplyAddAuditValuesAsync(nullConsumerStatus))
                .ReturnsAsync(nullConsumerStatus);

            // when
            ValueTask<ConsumerStatus> addConsumerStatusTask =
                this.consumerStatusService.AddConsumerStatusAsync(nullConsumerStatus);

            ConsumerStatusValidationException actualConsumerStatusValidationException =
                await Assert.ThrowsAsync<ConsumerStatusValidationException>(() =>
                    addConsumerStatusTask.AsTask());

            // then
            actualConsumerStatusValidationException.Should()
                .BeEquivalentTo(expectedConsumerStatusValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                    broker.ApplyAddAuditValuesAsync(nullConsumerStatus),
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

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnAddIfConsumerStatusIsInvalidAndLogItAsync(string invalidText)
        {
            // given
            string randomUserId = GetRandomString();
            User randomUser = CreateRandomUser(userId: randomUserId);

            var invalidConsumerStatus = new ConsumerStatus();

            var invalidConsumerStatusException =
                new InvalidConsumerStatusException(
                    message: "Invalid consumer status. Please correct the errors and try again.");

            invalidConsumerStatusException.AddData(
                key: nameof(ConsumerStatus.Id),
                values: "Id is required");

            invalidConsumerStatusException.AddData(
                key: nameof(ConsumerStatus.CreatedDate),
                values: "Date is required");

            invalidConsumerStatusException.AddData(
                key: nameof(ConsumerStatus.CreatedBy),
                values:
                    [
                        "Text is required",
                        $"Expected value to be '{randomUserId}' but found '{invalidConsumerStatus.CreatedBy}'."
                    ]);

            invalidConsumerStatusException.AddData(
                key: nameof(ConsumerStatus.UpdatedDate),
                values: "Date is required");

            invalidConsumerStatusException.AddData(
                key: nameof(ConsumerStatus.UpdatedBy),
                values: "Text is required");

            var expectedConsumerStatusValidationException =
                new ConsumerStatusValidationException(
                    message: "ConsumerStatus validation errors occurred, please try again.",
                    innerException: invalidConsumerStatusException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyAddAuditValuesAsync(invalidConsumerStatus))
                    .ReturnsAsync(invalidConsumerStatus);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomUser);

            // when
            ValueTask<ConsumerStatus> addConsumerStatusTask =
                this.consumerStatusService.AddConsumerStatusAsync(invalidConsumerStatus);

            ConsumerStatusValidationException actualConsumerStatusValidationException =
                await Assert.ThrowsAsync<ConsumerStatusValidationException>(() =>
                    addConsumerStatusTask.AsTask());

            // then
            actualConsumerStatusValidationException.Should()
                .BeEquivalentTo(expectedConsumerStatusValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyAddAuditValuesAsync(invalidConsumerStatus),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once());

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedConsumerStatusValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertConsumerStatusAsync(It.IsAny<ConsumerStatus>()),
                    Times.Never);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfCreateAndUpdateDatesIsNotSameAndLogItAsync()
        {
            // given
            int randomNumber = GetRandomNumber();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            string randomUserId = GetRandomString();
            User randomUser = CreateRandomUser(userId: randomUserId);
            ConsumerStatus randomConsumerStatus = CreateRandomConsumerStatus(randomDateTimeOffset, userId: randomUserId);
            ConsumerStatus invalidConsumerStatus = randomConsumerStatus;

            invalidConsumerStatus.UpdatedDate =
                invalidConsumerStatus.CreatedDate.AddDays(randomNumber);

            var invalidConsumerStatusException =
                new InvalidConsumerStatusException(
                    message: "Invalid consumer status. Please correct the errors and try again.");

            invalidConsumerStatusException.AddData(
                key: nameof(ConsumerStatus.UpdatedDate),
                values: $"Date is not the same as {nameof(ConsumerStatus.CreatedDate)}");

            var expectedConsumerStatusValidationException =
                new ConsumerStatusValidationException(
                    message: "ConsumerStatus validation errors occurred, please try again.",
                    innerException: invalidConsumerStatusException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyAddAuditValuesAsync(invalidConsumerStatus))
                    .ReturnsAsync(invalidConsumerStatus);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomUser);

            // when
            ValueTask<ConsumerStatus> addConsumerStatusTask =
                this.consumerStatusService.AddConsumerStatusAsync(invalidConsumerStatus);

            ConsumerStatusValidationException actualConsumerStatusValidationException =
                await Assert.ThrowsAsync<ConsumerStatusValidationException>(() =>
                    addConsumerStatusTask.AsTask());

            // then
            actualConsumerStatusValidationException.Should()
                .BeEquivalentTo(expectedConsumerStatusValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyAddAuditValuesAsync(invalidConsumerStatus),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once());

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedConsumerStatusValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertConsumerStatusAsync(It.IsAny<ConsumerStatus>()),
                    Times.Never);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfCreateAndUpdateUsersIsNotSameAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            string randomUserId = GetRandomString();
            User randomUser = CreateRandomUser(userId: randomUserId);
            ConsumerStatus randomConsumerStatus = CreateRandomConsumerStatus(randomDateTimeOffset, userId: randomUserId);
            ConsumerStatus invalidConsumerStatus = randomConsumerStatus.DeepClone();
            invalidConsumerStatus.UpdatedBy = Guid.NewGuid().ToString();

            var invalidConsumerStatusException =
                new InvalidConsumerStatusException(
                    message: "Invalid consumer status. Please correct the errors and try again.");

            invalidConsumerStatusException.AddData(
                key: nameof(ConsumerStatus.UpdatedBy),
                values: $"Text is not the same as {nameof(ConsumerStatus.CreatedBy)}");

            var expectedConsumerStatusValidationException =
                new ConsumerStatusValidationException(
                    message: "ConsumerStatus validation errors occurred, please try again.",
                    innerException: invalidConsumerStatusException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyAddAuditValuesAsync(invalidConsumerStatus))
                    .ReturnsAsync(invalidConsumerStatus);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomUser);

            // when
            ValueTask<ConsumerStatus> addConsumerStatusTask =
                this.consumerStatusService.AddConsumerStatusAsync(invalidConsumerStatus);

            ConsumerStatusValidationException actualConsumerStatusValidationException =
                await Assert.ThrowsAsync<ConsumerStatusValidationException>(() =>
                    addConsumerStatusTask.AsTask());

            // then
            actualConsumerStatusValidationException.Should()
                .BeEquivalentTo(expectedConsumerStatusValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyAddAuditValuesAsync(invalidConsumerStatus),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once());

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedConsumerStatusValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertConsumerStatusAsync(It.IsAny<ConsumerStatus>()),
                    Times.Never);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(MinutesBeforeOrAfter))]
        public async Task ShouldThrowValidationExceptionOnAddIfCreatedDateIsNotRecentAndLogItAsync(
            int minutesBeforeOrAfter)
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();

            DateTimeOffset invalidDateTime =
                randomDateTimeOffset.AddMinutes(minutesBeforeOrAfter);

            DateTimeOffset invalidDate = randomDateTimeOffset.AddMinutes(minutesBeforeOrAfter);
            DateTimeOffset startDate = randomDateTimeOffset.AddSeconds(-90);
            DateTimeOffset endDate = randomDateTimeOffset.AddSeconds(0);
            string randomUserId = GetRandomString();
            User randomUser = CreateRandomUser(userId: randomUserId);
            ConsumerStatus randomConsumerStatus = CreateRandomConsumerStatus(invalidDateTime, userId: randomUserId);
            ConsumerStatus invalidConsumerStatus = randomConsumerStatus;

            var invalidConsumerStatusException =
                new InvalidConsumerStatusException(
                    message: "Invalid consumer status. Please correct the errors and try again.");

            invalidConsumerStatusException.AddData(
                key: nameof(ConsumerStatus.CreatedDate),
                values:
                    $"Date is not recent. Expected a value between {startDate} and {endDate} but found {invalidDate}");

            var expectedConsumerStatusValidationException =
                new ConsumerStatusValidationException(
                    message: "ConsumerStatus validation errors occurred, please try again.",
                    innerException: invalidConsumerStatusException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyAddAuditValuesAsync(invalidConsumerStatus))
                    .ReturnsAsync(invalidConsumerStatus);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomUser);

            // when
            ValueTask<ConsumerStatus> addConsumerStatusTask =
                this.consumerStatusService.AddConsumerStatusAsync(invalidConsumerStatus);

            ConsumerStatusValidationException actualConsumerStatusValidationException =
                await Assert.ThrowsAsync<ConsumerStatusValidationException>(() =>
                    addConsumerStatusTask.AsTask());

            // then
            actualConsumerStatusValidationException.Should()
                .BeEquivalentTo(expectedConsumerStatusValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyAddAuditValuesAsync(invalidConsumerStatus),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once());

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedConsumerStatusValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertConsumerStatusAsync(It.IsAny<ConsumerStatus>()),
                    Times.Never);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
