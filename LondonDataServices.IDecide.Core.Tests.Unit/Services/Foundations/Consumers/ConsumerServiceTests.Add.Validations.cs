// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Core.Models.Foundations.Consumers;
using LondonDataServices.IDecide.Core.Models.Foundations.Consumers.Exceptions;
using LondonDataServices.IDecide.Core.Models.Securities;
using Moq;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.Consumers
{
    public partial class ConsumerServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfConsumerIsNullAndLogItAsync()
        {
            // given
            Consumer nullConsumer = null;

            var nullConsumerException =
                new NullConsumerException(message: "Consumer is null.");

            var expectedConsumerValidationException =
                new ConsumerValidationException(
                    message: "Consumer validation errors occurred, please try again.",
                    innerException: nullConsumerException);

            this.securityAuditBrokerMock.Setup(broker =>
                    broker.ApplyAddAuditValuesAsync(nullConsumer))
                .ReturnsAsync(nullConsumer);

            // when
            ValueTask<Consumer> addConsumerTask =
                this.consumerService.AddConsumerAsync(nullConsumer);

            ConsumerValidationException actualConsumerValidationException =
                await Assert.ThrowsAsync<ConsumerValidationException>(() =>
                    addConsumerTask.AsTask());

            // then
            actualConsumerValidationException.Should()
                .BeEquivalentTo(expectedConsumerValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                    broker.ApplyAddAuditValuesAsync(nullConsumer),
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

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnAddIfConsumerIsInvalidAndLogItAsync(string invalidText)
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
                values:
                    [
                        "Text is required",
                        $"Expected value to be '{randomUserId}' but found '{invalidConsumer.CreatedBy}'."
                    ]);

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
                broker.ApplyAddAuditValuesAsync(invalidConsumer))
                    .ReturnsAsync(invalidConsumer);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomUser);

            // when
            ValueTask<Consumer> addConsumerTask =
                this.consumerService.AddConsumerAsync(invalidConsumer);

            ConsumerValidationException actualConsumerValidationException =
                await Assert.ThrowsAsync<ConsumerValidationException>(() =>
                    addConsumerTask.AsTask());

            // then
            actualConsumerValidationException.Should()
                .BeEquivalentTo(expectedConsumerValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyAddAuditValuesAsync(invalidConsumer),
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
        public async Task ShouldThrowValidationExceptionOnAddIfConsumerHasInvalidLengthProperty()
        {
            // given
            string randomUserId = GetRandomString();
            User randomUser = CreateRandomUser(userId: randomUserId);

            var invalidConsumer = CreateRandomConsumer(GetRandomDateTimeOffset(), userId: randomUserId);
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
                broker.ApplyAddAuditValuesAsync(invalidConsumer))
                    .ReturnsAsync(invalidConsumer);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomUser);

            // when
            ValueTask<Consumer> addConsumerTask =
                this.consumerService.AddConsumerAsync(invalidConsumer);

            ConsumerValidationException actualConsumerValidationException =
                await Assert.ThrowsAsync<ConsumerValidationException>(
                    addConsumerTask.AsTask);

            // then
            actualConsumerValidationException.Should()
                .BeEquivalentTo(expectedConsumerValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyAddAuditValuesAsync(invalidConsumer),
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
    }
}
