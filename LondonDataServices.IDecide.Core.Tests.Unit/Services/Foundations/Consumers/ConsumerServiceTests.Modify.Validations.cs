// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using LondonDataServices.IDecide.Core.Models.Foundations.Consumers;
using LondonDataServices.IDecide.Core.Models.Foundations.Consumers.Exceptions;
using LondonDataServices.IDecide.Core.Models.Securities;
using Moq;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.Consumers
{
    public partial class ConsumerServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfConsumerIsNullAndLogItAsync()
        {
            // given
            Consumer nullConsumer = null;
            var nullConsumerException = new NullConsumerException(message: "Consumer is null.");

            var expectedConsumerValidationException =
                new ConsumerValidationException(
                    message: "Consumer validation errors occurred, please try again.",
                    innerException: nullConsumerException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyModifyAuditValuesAsync(nullConsumer))
                    .ReturnsAsync(nullConsumer);

            // when
            ValueTask<Consumer> modifyConsumerTask =
                this.consumerService.ModifyConsumerAsync(nullConsumer);

            ConsumerValidationException actualConsumerValidationException =
                await Assert.ThrowsAsync<ConsumerValidationException>(
                    modifyConsumerTask.AsTask);

            // then
            actualConsumerValidationException.Should()
                .BeEquivalentTo(expectedConsumerValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyModifyAuditValuesAsync(nullConsumer),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedConsumerValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateConsumerAsync(It.IsAny<Consumer>()),
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
        public async Task ShouldThrowValidationExceptionOnModifyIfConsumerIsInvalidAndLogItAsync(string invalidText)
        {
            // given 
            string randomUserId = GetRandomString();
            User randomUser = CreateRandomUser(userId: randomUserId);

            var invalidConsumer = new Consumer
            {
                Name = invalidText
            };

            var invalidConsumerException =
                new InvalidConsumerException(
                    message: "Invalid consumer. Please correct the errors and try again.");

            invalidConsumerException.AddData(
                key: nameof(Consumer.Id),
                values: "Id is required");

            invalidConsumerException.AddData(
                key: nameof(Consumer.Name),
                values: "Text is required");

            invalidConsumerException.AddData(
                key: nameof(Consumer.CreatedDate),
                values: "Date is required");

            invalidConsumerException.AddData(
                key: nameof(Consumer.CreatedBy),
                values: "Text is required");

            invalidConsumerException.AddData(
                key: nameof(Consumer.UpdatedDate),
                values: "Date is required");

            invalidConsumerException.AddData(
                key: nameof(Consumer.UpdatedBy),
                values: "Text is required");

            var expectedConsumerValidationException =
                new ConsumerValidationException(
                    message: "Consumer validation errors occurred, please try again.",
                    innerException: invalidConsumerException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyModifyAuditValuesAsync(invalidConsumer))
                    .ReturnsAsync(invalidConsumer);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomUser);

            // when
            ValueTask<Consumer> modifyConsumerTask =
                this.consumerService.ModifyConsumerAsync(invalidConsumer);

            ConsumerValidationException actualConsumerValidationException =
                await Assert.ThrowsAsync<ConsumerValidationException>(
                    modifyConsumerTask.AsTask);

            //then
            actualConsumerValidationException.Should()
                .BeEquivalentTo(expectedConsumerValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyModifyAuditValuesAsync(invalidConsumer),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedConsumerValidationException))),
                        Times.Once());

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateConsumerAsync(It.IsAny<Consumer>()),
                    Times.Never);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfConsumerHasInvalidLengthProperty()
        {
            // given
            string randomUserId = GetRandomString();
            User randomUser = CreateRandomUser(userId: randomUserId);

            var invalidConsumer = CreateRandomModifyConsumer(GetRandomDateTimeOffset(), userId: randomUserId);
            invalidConsumer.Name = GetRandomStringWithLengthOf(256);

            var invalidConsumerException =
                new InvalidConsumerException(
                    message: "Invalid consumer. Please correct the errors and try again.");

            invalidConsumerException.AddData(
                key: nameof(Consumer.Name),
                values: $"Text exceed max length of {invalidConsumer.Name.Length - 1} characters");

            var expectedConsumerValidationException =
                new ConsumerValidationException(
                    message: "Consumer validation errors occurred, please try again.",
                    innerException: invalidConsumerException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyModifyAuditValuesAsync(invalidConsumer))
                    .ReturnsAsync(invalidConsumer);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomUser);

            // when
            ValueTask<Consumer> modifyConsumerTask =
                this.consumerService.ModifyConsumerAsync(invalidConsumer);

            ConsumerValidationException actualConsumerValidationException =
                await Assert.ThrowsAsync<ConsumerValidationException>(
                    modifyConsumerTask.AsTask);

            // then
            actualConsumerValidationException.Should()
                .BeEquivalentTo(expectedConsumerValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyModifyAuditValuesAsync(invalidConsumer),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once());

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedConsumerValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertConsumerAsync(It.IsAny<Consumer>()),
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

            Consumer randomConsumer =
                CreateRandomConsumer(dateTimeOffset: randomDateTimeOffset, userId: randomUserId);

            Consumer invalidConsumer = randomConsumer;

            var invalidConsumerException =
                new InvalidConsumerException(
                    message: "Invalid consumer. Please correct the errors and try again.");

            invalidConsumerException.AddData(
                key: nameof(Consumer.UpdatedDate),
                values: $"Date is the same as {nameof(Consumer.CreatedDate)}");

            var expectedConsumerValidationException =
                new ConsumerValidationException(
                    message: "Consumer validation errors occurred, please try again.",
                    innerException: invalidConsumerException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyModifyAuditValuesAsync(invalidConsumer))
                    .ReturnsAsync(invalidConsumer);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomUser);

            // when
            ValueTask<Consumer> modifyConsumerTask = this.consumerService.ModifyConsumerAsync(invalidConsumer);

            ConsumerValidationException actualConsumerValidationException =
                await Assert.ThrowsAsync<ConsumerValidationException>(
                    modifyConsumerTask.AsTask);

            // then
            actualConsumerValidationException.Should()
                .BeEquivalentTo(expectedConsumerValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyModifyAuditValuesAsync(invalidConsumer),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedConsumerValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectConsumerByIdAsync(invalidConsumer.Id),
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

            Consumer randomConsumer =
                CreateRandomConsumer(dateTimeOffset: randomDateTimeOffset, userId: randomUserId);

            randomConsumer.UpdatedDate = randomDateTimeOffset.AddMinutes(minutes);

            var invalidConsumerException =
                new InvalidConsumerException(
                    message: "Invalid consumer. Please correct the errors and try again.");

            invalidConsumerException.AddData(
                key: nameof(Consumer.UpdatedDate),
                values:
                    $"Date is not recent. Expected a value between {startDate} and {endDate} but found {invalidDate}");

            var expectedConsumerValidatonException =
                new ConsumerValidationException(
                    message: "Consumer validation errors occurred, please try again.",
                    innerException: invalidConsumerException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyModifyAuditValuesAsync(randomConsumer))
                    .ReturnsAsync(randomConsumer);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomUser);

            // when
            ValueTask<Consumer> modifyConsumerTask =
                this.consumerService.ModifyConsumerAsync(randomConsumer);

            ConsumerValidationException actualConsumerValidationException =
                await Assert.ThrowsAsync<ConsumerValidationException>(
                    modifyConsumerTask.AsTask);

            // then
            actualConsumerValidationException.Should()
                .BeEquivalentTo(expectedConsumerValidatonException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyModifyAuditValuesAsync(randomConsumer),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedConsumerValidatonException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectConsumerByIdAsync(It.IsAny<Guid>()),
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
        public async Task ShouldThrowValidationExceptionOnModifyIfConsumerDoesNotExistAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            string randomUserId = GetRandomString();
            User randomUser = CreateRandomUser(userId: randomUserId);

            Consumer randomConsumer = CreateRandomModifyConsumer(
                dateTimeOffset: randomDateTimeOffset, userId: randomUserId);

            Consumer nonExistConsumer = randomConsumer;
            Consumer nullConsumer = null;

            var notFoundConsumerException = new NotFoundConsumerException(
                message: $"Couldn't find consumer with consumerId: {nonExistConsumer.Id}.");

            var expectedConsumerValidationException =
                new ConsumerValidationException(
                    message: "Consumer validation errors occurred, please try again.",
                    innerException: notFoundConsumerException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyModifyAuditValuesAsync(nonExistConsumer))
                    .ReturnsAsync(nonExistConsumer);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectConsumerByIdAsync(nonExistConsumer.Id))
                    .ReturnsAsync(nullConsumer);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomUser);

            // when 
            ValueTask<Consumer> modifyConsumerTask =
                this.consumerService.ModifyConsumerAsync(nonExistConsumer);

            ConsumerValidationException actualConsumerValidationException =
                await Assert.ThrowsAsync<ConsumerValidationException>(
                    modifyConsumerTask.AsTask);

            // then
            actualConsumerValidationException.Should()
                .BeEquivalentTo(expectedConsumerValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyModifyAuditValuesAsync(nonExistConsumer),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectConsumerByIdAsync(nonExistConsumer.Id),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedConsumerValidationException))),
                        Times.Once);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
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
            User randomUser = CreateRandomUser(userId: randomUserId);

            Consumer randomConsumer = CreateRandomModifyConsumer(
                dateTimeOffset: randomDateTimeOffset, userId: randomUserId);

            Consumer invalidConsumer = randomConsumer.DeepClone();
            Consumer storageConsumer = invalidConsumer.DeepClone();
            storageConsumer.CreatedDate = storageConsumer.CreatedDate.AddMinutes(randomMinutes);
            storageConsumer.UpdatedDate = storageConsumer.UpdatedDate.AddMinutes(randomMinutes);

            var invalidConsumerException =
                new InvalidConsumerException(
                    message: "Invalid consumer. Please correct the errors and try again.");

            invalidConsumerException.AddData(
                key: nameof(Consumer.CreatedDate),
                values: $"Date is not the same as {nameof(Consumer.CreatedDate)}");

            var expectedConsumerValidationException =
                new ConsumerValidationException(
                    message: "Consumer validation errors occurred, please try again.",
                    innerException: invalidConsumerException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyModifyAuditValuesAsync(invalidConsumer))
                    .ReturnsAsync(invalidConsumer);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectConsumerByIdAsync(invalidConsumer.Id))
                    .ReturnsAsync(storageConsumer);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomUser);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.EnsureAddAuditValuesRemainsUnchangedOnModifyAsync(invalidConsumer, storageConsumer))
                    .ReturnsAsync(invalidConsumer);

            // when
            ValueTask<Consumer> modifyConsumerTask =
                this.consumerService.ModifyConsumerAsync(invalidConsumer);

            ConsumerValidationException actualConsumerValidationException =
                await Assert.ThrowsAsync<ConsumerValidationException>(
                    modifyConsumerTask.AsTask);

            // then
            actualConsumerValidationException.Should()
                .BeEquivalentTo(expectedConsumerValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyModifyAuditValuesAsync(invalidConsumer),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectConsumerByIdAsync(invalidConsumer.Id),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.EnsureAddAuditValuesRemainsUnchangedOnModifyAsync(invalidConsumer, storageConsumer),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedConsumerValidationException))),
                       Times.Once);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
