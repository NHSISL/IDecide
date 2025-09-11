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
        public async Task ShouldThrowValidationExceptionOnModifyIfConsumerAdoptionIsNullAndLogItAsync()
        {
            // given
            ConsumerAdoption nullConsumerAdoption = null;
            var nullConsumerAdoptionException = new NullConsumerAdoptionException(message: "ConsumerAdoption is null.");

            var expectedConsumerAdoptionValidationException =
                new ConsumerAdoptionValidationException(
                    message: "ConsumerAdoption validation errors occurred, please try again.",
                    innerException: nullConsumerAdoptionException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyModifyAuditValuesAsync(nullConsumerAdoption))
                    .ReturnsAsync(nullConsumerAdoption);

            // when
            ValueTask<ConsumerAdoption> modifyConsumerAdoptionTask =
                this.consumerAdoptionService.ModifyConsumerAdoptionAsync(nullConsumerAdoption);

            ConsumerAdoptionValidationException actualConsumerAdoptionValidationException =
                await Assert.ThrowsAsync<ConsumerAdoptionValidationException>(
                    modifyConsumerAdoptionTask.AsTask);

            // then
            actualConsumerAdoptionValidationException.Should()
                .BeEquivalentTo(expectedConsumerAdoptionValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyModifyAuditValuesAsync(nullConsumerAdoption),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedConsumerAdoptionValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateConsumerAdoptionAsync(It.IsAny<ConsumerAdoption>()),
                    Times.Never);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfConsumerAdoptionIsInvalidAndLogItAsync()
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
                values: "Text is required");

            invalidConsumerAdoptionException.AddData(
                key: nameof(ConsumerAdoption.UpdatedDate),
                values:
                [
                    "Date is required",
                    $"Date is the same as {nameof(ConsumerAdoption.CreatedDate)}"
                ]);

            invalidConsumerAdoptionException.AddData(
                key: nameof(ConsumerAdoption.UpdatedBy),
                values:
                [
                    "Text is required",
                    $"Expected value to be '{randomUserId}' but found '{invalidConsumerAdoption.UpdatedBy}'."
                ]);

            var expectedConsumerAdoptionValidationException =
                new ConsumerAdoptionValidationException(
                    message: "ConsumerAdoption validation errors occurred, please try again.",
                    innerException: invalidConsumerAdoptionException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyModifyAuditValuesAsync(invalidConsumerAdoption))
                    .ReturnsAsync(invalidConsumerAdoption);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.GetCurrentUserIdAsync())
                    .ReturnsAsync(randomUserId);

            // when
            ValueTask<ConsumerAdoption> modifyConsumerAdoptionTask =
                this.consumerAdoptionService.ModifyConsumerAdoptionAsync(invalidConsumerAdoption);

            ConsumerAdoptionValidationException actualConsumerAdoptionValidationException =
                await Assert.ThrowsAsync<ConsumerAdoptionValidationException>(
                    modifyConsumerAdoptionTask.AsTask);

            //then
            actualConsumerAdoptionValidationException.Should()
                .BeEquivalentTo(expectedConsumerAdoptionValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyModifyAuditValuesAsync(invalidConsumerAdoption),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.GetCurrentUserIdAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedConsumerAdoptionValidationException))),
                        Times.Once());

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateConsumerAdoptionAsync(It.IsAny<ConsumerAdoption>()),
                    Times.Never);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
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

            ConsumerAdoption randomConsumerAdoption =
                CreateRandomConsumerAdoption(dateTimeOffset: randomDateTimeOffset, userId: randomUserId);

            ConsumerAdoption invalidConsumerAdoption = randomConsumerAdoption;

            var invalidConsumerAdoptionException =
                new InvalidConsumerAdoptionException(
                    message: "Invalid consumerAdoption. Please correct the errors and try again.");

            invalidConsumerAdoptionException.AddData(
                key: nameof(ConsumerAdoption.UpdatedDate),
                values: $"Date is the same as {nameof(ConsumerAdoption.CreatedDate)}");

            var expectedConsumerAdoptionValidationException =
                new ConsumerAdoptionValidationException(
                    message: "ConsumerAdoption validation errors occurred, please try again.",
                    innerException: invalidConsumerAdoptionException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyModifyAuditValuesAsync(invalidConsumerAdoption))
                    .ReturnsAsync(invalidConsumerAdoption);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.GetCurrentUserIdAsync())
                    .ReturnsAsync(randomUserId);

            // when
            ValueTask<ConsumerAdoption> modifyConsumerAdoptionTask =
                this.consumerAdoptionService.ModifyConsumerAdoptionAsync(invalidConsumerAdoption);

            ConsumerAdoptionValidationException actualConsumerAdoptionValidationException =
                await Assert.ThrowsAsync<ConsumerAdoptionValidationException>(
                    modifyConsumerAdoptionTask.AsTask);

            // then
            actualConsumerAdoptionValidationException.Should()
                .BeEquivalentTo(expectedConsumerAdoptionValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyModifyAuditValuesAsync(invalidConsumerAdoption),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.GetCurrentUserIdAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedConsumerAdoptionValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectConsumerAdoptionByIdAsync(invalidConsumerAdoption.Id),
                    Times.Never);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
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

            ConsumerAdoption randomConsumerAdoption =
                CreateRandomConsumerAdoption(dateTimeOffset: randomDateTimeOffset, userId: randomUserId);

            randomConsumerAdoption.UpdatedDate = randomDateTimeOffset.AddMinutes(minutes);

            var invalidConsumerAdoptionException =
                new InvalidConsumerAdoptionException(
                    message: "Invalid consumerAdoption. Please correct the errors and try again.");

            invalidConsumerAdoptionException.AddData(
                key: nameof(ConsumerAdoption.UpdatedDate),
                values:
                    $"Date is not recent. Expected a value between {startDate} and {endDate} but found {invalidDate}");

            var expectedConsumerAdoptionValidatonException =
                new ConsumerAdoptionValidationException(
                    message: "ConsumerAdoption validation errors occurred, please try again.",
                    innerException: invalidConsumerAdoptionException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyModifyAuditValuesAsync(randomConsumerAdoption))
                    .ReturnsAsync(randomConsumerAdoption);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                .ReturnsAsync(randomDateTimeOffset);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.GetCurrentUserIdAsync())
                    .ReturnsAsync(randomUserId);

            // when
            ValueTask<ConsumerAdoption> modifyConsumerAdoptionTask =
                this.consumerAdoptionService.ModifyConsumerAdoptionAsync(randomConsumerAdoption);

            ConsumerAdoptionValidationException actualConsumerAdoptionValidationException =
                await Assert.ThrowsAsync<ConsumerAdoptionValidationException>(
                    modifyConsumerAdoptionTask.AsTask);

            // then
            actualConsumerAdoptionValidationException.Should()
                .BeEquivalentTo(expectedConsumerAdoptionValidatonException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyModifyAuditValuesAsync(randomConsumerAdoption),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedConsumerAdoptionValidatonException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectConsumerAdoptionByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.GetCurrentUserIdAsync(),
                    Times.Once);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfConsumerAdoptionDoesNotExistAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            string randomUserId = GetRandomString();

            ConsumerAdoption randomConsumerAdoption = CreateRandomModifyConsumerAdoption(
                dateTimeOffset: randomDateTimeOffset, userId: randomUserId);

            ConsumerAdoption nonExistConsumerAdoption = randomConsumerAdoption;
            ConsumerAdoption nullConsumerAdoption = null;

            var notFoundConsumerAdoptionException = new NotFoundConsumerAdoptionException(
                message: $"Couldn't find consumerAdoption with consumerAdoptionId: {nonExistConsumerAdoption.Id}.");

            var expectedConsumerAdoptionValidationException =
                new ConsumerAdoptionValidationException(
                    message: "ConsumerAdoption validation errors occurred, please try again.",
                    innerException: notFoundConsumerAdoptionException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyModifyAuditValuesAsync(nonExistConsumerAdoption))
                    .ReturnsAsync(nonExistConsumerAdoption);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectConsumerAdoptionByIdAsync(nonExistConsumerAdoption.Id))
                    .ReturnsAsync(nullConsumerAdoption);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.GetCurrentUserIdAsync())
                    .ReturnsAsync(randomUserId);

            // when 
            ValueTask<ConsumerAdoption> modifyConsumerAdoptionTask =
                this.consumerAdoptionService.ModifyConsumerAdoptionAsync(nonExistConsumerAdoption);

            ConsumerAdoptionValidationException actualConsumerAdoptionValidationException =
                await Assert.ThrowsAsync<ConsumerAdoptionValidationException>(
                    modifyConsumerAdoptionTask.AsTask);

            // then
            actualConsumerAdoptionValidationException.Should()
                .BeEquivalentTo(expectedConsumerAdoptionValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyModifyAuditValuesAsync(nonExistConsumerAdoption),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectConsumerAdoptionByIdAsync(nonExistConsumerAdoption.Id),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.GetCurrentUserIdAsync(),
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
        public async Task ShouldThrowValidationExceptionOnModifyIfStorageCreatedDateNotSameAsCreatedDateAndLogItAsync()
        {
            // given
            int randomNumber = GetRandomNegativeNumber();
            int randomMinutes = randomNumber;
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            string randomUserId = GetRandomString();

            ConsumerAdoption randomConsumerAdoption = CreateRandomModifyConsumerAdoption(
                dateTimeOffset: randomDateTimeOffset, userId: randomUserId);

            ConsumerAdoption invalidConsumerAdoption = randomConsumerAdoption.DeepClone();
            ConsumerAdoption storageConsumerAdoption = invalidConsumerAdoption.DeepClone();
            storageConsumerAdoption.CreatedDate = storageConsumerAdoption.CreatedDate.AddMinutes(randomMinutes);
            storageConsumerAdoption.UpdatedDate = storageConsumerAdoption.UpdatedDate.AddMinutes(randomMinutes);

            var invalidConsumerAdoptionException =
                new InvalidConsumerAdoptionException(
                    message: "Invalid consumerAdoption. Please correct the errors and try again.");

            invalidConsumerAdoptionException.AddData(
                key: nameof(ConsumerAdoption.CreatedDate),
                values: $"Date is not the same as {nameof(ConsumerAdoption.CreatedDate)}");

            var expectedConsumerAdoptionValidationException =
                new ConsumerAdoptionValidationException(
                    message: "ConsumerAdoption validation errors occurred, please try again.",
                    innerException: invalidConsumerAdoptionException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyModifyAuditValuesAsync(invalidConsumerAdoption))
                    .ReturnsAsync(invalidConsumerAdoption);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectConsumerAdoptionByIdAsync(invalidConsumerAdoption.Id))
                    .ReturnsAsync(storageConsumerAdoption);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.GetCurrentUserIdAsync())
                    .ReturnsAsync(randomUserId);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.EnsureAddAuditValuesRemainsUnchangedOnModifyAsync(invalidConsumerAdoption, storageConsumerAdoption))
                    .ReturnsAsync(invalidConsumerAdoption);

            // when
            ValueTask<ConsumerAdoption> modifyConsumerAdoptionTask =
                this.consumerAdoptionService.ModifyConsumerAdoptionAsync(invalidConsumerAdoption);

            ConsumerAdoptionValidationException actualConsumerAdoptionValidationException =
                await Assert.ThrowsAsync<ConsumerAdoptionValidationException>(
                    modifyConsumerAdoptionTask.AsTask);

            // then
            actualConsumerAdoptionValidationException.Should()
                .BeEquivalentTo(expectedConsumerAdoptionValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyModifyAuditValuesAsync(invalidConsumerAdoption),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectConsumerAdoptionByIdAsync(invalidConsumerAdoption.Id),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.GetCurrentUserIdAsync(),
                    Times.Once);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.EnsureAddAuditValuesRemainsUnchangedOnModifyAsync(invalidConsumerAdoption, storageConsumerAdoption),
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
        public async Task ShouldThrowValidationExceptionOnModifyIfCreatedUserDoesntMatchStorageAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            string randomUserId = GetRandomString();

            ConsumerAdoption randomConsumerAdoption =
                CreateRandomModifyConsumerAdoption(dateTimeOffset: randomDateTimeOffset, userId: randomUserId);

            ConsumerAdoption invalidConsumerAdoption = randomConsumerAdoption.DeepClone();
            ConsumerAdoption storageConsumerAdoption = invalidConsumerAdoption.DeepClone();
            invalidConsumerAdoption.CreatedBy = Guid.NewGuid().ToString();
            storageConsumerAdoption.UpdatedDate = storageConsumerAdoption.CreatedDate;

            var invalidConsumerAdoptionException =
                new InvalidConsumerAdoptionException(
                    message: "Invalid consumerAdoption. Please correct the errors and try again.");

            invalidConsumerAdoptionException.AddData(
                key: nameof(ConsumerAdoption.CreatedBy),
                values: $"Text is not the same as {nameof(ConsumerAdoption.CreatedBy)}");

            var expectedConsumerAdoptionValidationException =
                new ConsumerAdoptionValidationException(
                    message: "ConsumerAdoption validation errors occurred, please try again.",
                    innerException: invalidConsumerAdoptionException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyModifyAuditValuesAsync(invalidConsumerAdoption))
                    .ReturnsAsync(invalidConsumerAdoption);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectConsumerAdoptionByIdAsync(invalidConsumerAdoption.Id))
                    .ReturnsAsync(storageConsumerAdoption);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.GetCurrentUserIdAsync())
                    .ReturnsAsync(randomUserId);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.EnsureAddAuditValuesRemainsUnchangedOnModifyAsync(invalidConsumerAdoption, storageConsumerAdoption))
                    .ReturnsAsync(invalidConsumerAdoption);

            // when
            ValueTask<ConsumerAdoption> modifyConsumerAdoptionTask =
                this.consumerAdoptionService.ModifyConsumerAdoptionAsync(invalidConsumerAdoption);

            ConsumerAdoptionValidationException actualConsumerAdoptionValidationException =
                await Assert.ThrowsAsync<ConsumerAdoptionValidationException>(
                    modifyConsumerAdoptionTask.AsTask);

            // then
            actualConsumerAdoptionValidationException.Should().BeEquivalentTo(expectedConsumerAdoptionValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyModifyAuditValuesAsync(invalidConsumerAdoption),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectConsumerAdoptionByIdAsync(invalidConsumerAdoption.Id),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.GetCurrentUserIdAsync(),
                    Times.Once);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.EnsureAddAuditValuesRemainsUnchangedOnModifyAsync(invalidConsumerAdoption, storageConsumerAdoption),
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
        public async Task ShouldThrowValidationExceptionOnModifyIfStorageUpdatedDateSameAsUpdatedDateAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            string randomUserId = GetRandomString();

            ConsumerAdoption randomConsumerAdoption =
                CreateRandomModifyConsumerAdoption(dateTimeOffset: randomDateTimeOffset, userId: randomUserId);

            ConsumerAdoption invalidConsumerAdoption = randomConsumerAdoption;
            ConsumerAdoption storageConsumerAdoption = randomConsumerAdoption.DeepClone();

            var invalidConsumerAdoptionException =
                new InvalidConsumerAdoptionException(
                    message: "Invalid consumerAdoption. Please correct the errors and try again.");

            invalidConsumerAdoptionException.AddData(
                key: nameof(ConsumerAdoption.UpdatedDate),
                values: $"Date is the same as {nameof(ConsumerAdoption.UpdatedDate)}");

            var expectedConsumerAdoptionValidationException =
                new ConsumerAdoptionValidationException(
                    message: "ConsumerAdoption validation errors occurred, please try again.",
                    innerException: invalidConsumerAdoptionException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyModifyAuditValuesAsync(invalidConsumerAdoption))
                    .ReturnsAsync(invalidConsumerAdoption);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectConsumerAdoptionByIdAsync(invalidConsumerAdoption.Id))
                    .ReturnsAsync(storageConsumerAdoption);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.GetCurrentUserIdAsync())
                    .ReturnsAsync(randomUserId);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.EnsureAddAuditValuesRemainsUnchangedOnModifyAsync(invalidConsumerAdoption, storageConsumerAdoption))
                    .ReturnsAsync(invalidConsumerAdoption);

            // when
            ValueTask<ConsumerAdoption> modifyConsumerAdoptionTask =
                this.consumerAdoptionService.ModifyConsumerAdoptionAsync(invalidConsumerAdoption);

            // then
            await Assert.ThrowsAsync<ConsumerAdoptionValidationException>(
                modifyConsumerAdoptionTask.AsTask);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyModifyAuditValuesAsync(invalidConsumerAdoption),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.GetCurrentUserIdAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedConsumerAdoptionValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectConsumerAdoptionByIdAsync(invalidConsumerAdoption.Id),
                    Times.Once);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.EnsureAddAuditValuesRemainsUnchangedOnModifyAsync(invalidConsumerAdoption, storageConsumerAdoption),
                    Times.Once);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
