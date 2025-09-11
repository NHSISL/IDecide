// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using LondonDataServices.IDecide.Core.Models.Foundations.ConsumerAdoptions;
using LondonDataServices.IDecide.Core.Models.Foundations.ConsumerAdoptions.Exceptions;
using Moq;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.ConsumerAdoptions
{
    public partial class ConsumerAdoptionServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfConsumerAdoptionIsNullAndLogItAsync()
        {
            // given
            ConsumerAdoption nullConsumerAdoption = null;

            var nullConsumerAdoptionException =
                new NullConsumerAdoptionException(message: "ConsumerAdoption is null.");

            var expectedConsumerAdoptionValidationException =
                new ConsumerAdoptionValidationException(
                    message: "ConsumerAdoption validation errors occurred, please try again.",
                    innerException: nullConsumerAdoptionException);

            this.securityAuditBrokerMock.Setup(broker =>
                    broker.ApplyAddAuditValuesAsync(nullConsumerAdoption))
                .ReturnsAsync(nullConsumerAdoption);

            // when
            ValueTask<ConsumerAdoption> addConsumerAdoptionTask =
                this.consumerAdoptionService.AddConsumerAdoptionAsync(nullConsumerAdoption);

            ConsumerAdoptionValidationException actualConsumerAdoptionValidationException =
                await Assert.ThrowsAsync<ConsumerAdoptionValidationException>(() =>
                    addConsumerAdoptionTask.AsTask());

            // then
            actualConsumerAdoptionValidationException.Should()
                .BeEquivalentTo(expectedConsumerAdoptionValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                    broker.ApplyAddAuditValuesAsync(nullConsumerAdoption),
                Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                    broker.LogErrorAsync(It.Is(SameExceptionAs(
                        expectedConsumerAdoptionValidationException))),
                Times.Once);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfConsumerAdoptionIsInvalidAndLogItAsync()
        {
            // given
            string randomUserId = GetRandomString();

            var invalidConsumerAdoption = new ConsumerAdoption();

            var invalidConsumerAdoptionException =
                new InvalidConsumerAdoptionException(
                    message: "Invalid consumerAdoption. Please correct the errors and try again.");

            invalidConsumerAdoptionException.AddData(
                key: nameof(ConsumerAdoption.Id),
                values: "Id is required");

            invalidConsumerAdoptionException.AddData(
               key: nameof(ConsumerAdoption.AdoptionDate),
               values: "Date is required");

            invalidConsumerAdoptionException.AddData(
                key: nameof(ConsumerAdoption.CreatedDate),
                values: "Date is required");

            invalidConsumerAdoptionException.AddData(
                key: nameof(ConsumerAdoption.CreatedBy),
                values:
                    [
                        "Text is required",
                        $"Expected value to be '{randomUserId}' but found '{invalidConsumerAdoption.CreatedBy}'."
                    ]);

            invalidConsumerAdoptionException.AddData(
                key: nameof(ConsumerAdoption.UpdatedDate),
                values: "Date is required");

            invalidConsumerAdoptionException.AddData(
                key: nameof(ConsumerAdoption.UpdatedBy),
                values: "Text is required");

            var expectedConsumerAdoptionValidationException =
                new ConsumerAdoptionValidationException(
                    message: "ConsumerAdoption validation errors occurred, please try again.",
                    innerException: invalidConsumerAdoptionException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyAddAuditValuesAsync(invalidConsumerAdoption))
                    .ReturnsAsync(invalidConsumerAdoption);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.GetCurrentUserIdAsync())
                    .ReturnsAsync(randomUserId);

            // when
            ValueTask<ConsumerAdoption> addConsumerAdoptionTask =
                this.consumerAdoptionService.AddConsumerAdoptionAsync(invalidConsumerAdoption);

            ConsumerAdoptionValidationException actualConsumerAdoptionValidationException =
                await Assert.ThrowsAsync<ConsumerAdoptionValidationException>(() =>
                    addConsumerAdoptionTask.AsTask());

            // then
            actualConsumerAdoptionValidationException.Should()
                .BeEquivalentTo(expectedConsumerAdoptionValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyAddAuditValuesAsync(invalidConsumerAdoption),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once());

            this.securityAuditBrokerMock.Verify(broker =>
                broker.GetCurrentUserIdAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedConsumerAdoptionValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertConsumerAdoptionAsync(It.IsAny<ConsumerAdoption>()),
                    Times.Never);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
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

            ConsumerAdoption randomConsumerAdoption = CreateRandomConsumerAdoption(
                randomDateTimeOffset, userId: randomUserId);

            ConsumerAdoption invalidConsumerAdoption = randomConsumerAdoption;

            invalidConsumerAdoption.UpdatedDate =
                invalidConsumerAdoption.CreatedDate.AddDays(randomNumber);

            var invalidConsumerAdoptionException =
                new InvalidConsumerAdoptionException(
                    message: "Invalid consumerAdoption. Please correct the errors and try again.");

            invalidConsumerAdoptionException.AddData(
                key: nameof(ConsumerAdoption.UpdatedDate),
                values: $"Date is not the same as {nameof(ConsumerAdoption.CreatedDate)}");

            var expectedConsumerAdoptionValidationException =
                new ConsumerAdoptionValidationException(
                    message: "ConsumerAdoption validation errors occurred, please try again.",
                    innerException: invalidConsumerAdoptionException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyAddAuditValuesAsync(invalidConsumerAdoption))
                    .ReturnsAsync(invalidConsumerAdoption);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.GetCurrentUserIdAsync())
                    .ReturnsAsync(randomUserId);

            // when
            ValueTask<ConsumerAdoption> addConsumerAdoptionTask =
                this.consumerAdoptionService.AddConsumerAdoptionAsync(invalidConsumerAdoption);

            ConsumerAdoptionValidationException actualConsumerAdoptionValidationException =
                await Assert.ThrowsAsync<ConsumerAdoptionValidationException>(() =>
                    addConsumerAdoptionTask.AsTask());

            // then
            actualConsumerAdoptionValidationException.Should()
                .BeEquivalentTo(expectedConsumerAdoptionValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyAddAuditValuesAsync(invalidConsumerAdoption),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once());

            this.securityAuditBrokerMock.Verify(broker =>
                broker.GetCurrentUserIdAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedConsumerAdoptionValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertConsumerAdoptionAsync(It.IsAny<ConsumerAdoption>()),
                    Times.Never);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
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

            ConsumerAdoption randomConsumerAdoption = CreateRandomConsumerAdoption(
                randomDateTimeOffset, userId: randomUserId);

            ConsumerAdoption invalidConsumerAdoption = randomConsumerAdoption.DeepClone();
            invalidConsumerAdoption.UpdatedBy = Guid.NewGuid().ToString();

            var invalidConsumerAdoptionException =
                new InvalidConsumerAdoptionException(
                    message: "Invalid consumerAdoption. Please correct the errors and try again.");

            invalidConsumerAdoptionException.AddData(
                key: nameof(ConsumerAdoption.UpdatedBy),
                values: $"Text is not the same as {nameof(ConsumerAdoption.CreatedBy)}");

            var expectedConsumerAdoptionValidationException =
                new ConsumerAdoptionValidationException(
                    message: "ConsumerAdoption validation errors occurred, please try again.",
                    innerException: invalidConsumerAdoptionException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyAddAuditValuesAsync(invalidConsumerAdoption))
                    .ReturnsAsync(invalidConsumerAdoption);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.GetCurrentUserIdAsync())
                    .ReturnsAsync(randomUserId);

            // when
            ValueTask<ConsumerAdoption> addConsumerAdoptionTask =
                this.consumerAdoptionService.AddConsumerAdoptionAsync(invalidConsumerAdoption);

            ConsumerAdoptionValidationException actualConsumerAdoptionValidationException =
                await Assert.ThrowsAsync<ConsumerAdoptionValidationException>(() =>
                    addConsumerAdoptionTask.AsTask());

            // then
            actualConsumerAdoptionValidationException.Should()
                .BeEquivalentTo(expectedConsumerAdoptionValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyAddAuditValuesAsync(invalidConsumerAdoption),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once());

            this.securityAuditBrokerMock.Verify(broker =>
                broker.GetCurrentUserIdAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedConsumerAdoptionValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertConsumerAdoptionAsync(It.IsAny<ConsumerAdoption>()),
                    Times.Never);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
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
            ConsumerAdoption randomConsumerAdoption = CreateRandomConsumerAdoption(invalidDateTime, userId: randomUserId);
            ConsumerAdoption invalidConsumerAdoption = randomConsumerAdoption;

            var invalidConsumerAdoptionException =
                new InvalidConsumerAdoptionException(
                    message: "Invalid consumerAdoption. Please correct the errors and try again.");

            invalidConsumerAdoptionException.AddData(
                key: nameof(ConsumerAdoption.CreatedDate),
                values:
                    $"Date is not recent. Expected a value between {startDate} and {endDate} but found {invalidDate}");

            var expectedConsumerAdoptionValidationException =
                new ConsumerAdoptionValidationException(
                    message: "ConsumerAdoption validation errors occurred, please try again.",
                    innerException: invalidConsumerAdoptionException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyAddAuditValuesAsync(invalidConsumerAdoption))
                    .ReturnsAsync(invalidConsumerAdoption);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.GetCurrentUserIdAsync())
                    .ReturnsAsync(randomUserId);

            // when
            ValueTask<ConsumerAdoption> addConsumerAdoptionTask =
                this.consumerAdoptionService.AddConsumerAdoptionAsync(invalidConsumerAdoption);

            ConsumerAdoptionValidationException actualConsumerAdoptionValidationException =
                await Assert.ThrowsAsync<ConsumerAdoptionValidationException>(() =>
                    addConsumerAdoptionTask.AsTask());

            // then
            actualConsumerAdoptionValidationException.Should()
                .BeEquivalentTo(expectedConsumerAdoptionValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyAddAuditValuesAsync(invalidConsumerAdoption),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once());

            this.securityAuditBrokerMock.Verify(broker =>
                broker.GetCurrentUserIdAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedConsumerAdoptionValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertConsumerAdoptionAsync(It.IsAny<ConsumerAdoption>()),
                    Times.Never);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
