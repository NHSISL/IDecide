// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Core.Models.Foundations.DecisionTypes.Exceptions;
using LondonDataServices.IDecide.Core.Models.Foundations.DecisionTypes;
using Moq;
using LondonDataServices.IDecide.Core.Models.Securities;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.DecisionTypes
{
    public partial class DecisionTypeServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddDecisionTypeAsync()
        {
            // given
            DecisionType nullDecisionType = null;
            var nullDecisionTypeException = new NullDecisionTypeException(message: "DecisionType is null.");

            var expectedDecisionTypeValidationException =
                new DecisionTypeValidationException(
                    message: "DecisionType validation error occurred, please fix errors and try again.",
                    innerException: nullDecisionTypeException);

            // when
            ValueTask<DecisionType> addDecisionTypeTask = this.decisionTypeService.AddDecisionTypeAsync(nullDecisionType);

            DecisionTypeValidationException actualDecisionTypeValidationException =
                await Assert.ThrowsAsync<DecisionTypeValidationException>(testCode: addDecisionTypeTask.AsTask);

            // then
            actualDecisionTypeValidationException.Should().BeEquivalentTo(expectedDecisionTypeValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedDecisionTypeValidationException))), Times.Once());

            this.storageBrokerMock.Verify(broker =>
                broker.InsertDecisionTypeAsync(It.IsAny<DecisionType>()),
                    Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnAddIfDecisionTypeIsInvalidAndLogItAsync(string invalidText)
        {
            // given
            User randomUser = CreateRandomUser();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            DateTimeOffset startDate = randomDateTimeOffset.AddSeconds(-90);
            DateTimeOffset endDate = randomDateTimeOffset.AddSeconds(0);

            var invalidDecisionType = new DecisionType
            {
                Name = invalidText,
                CreatedBy = invalidText,
                UpdatedBy = invalidText,
            };

            var invalidDecisionTypeException =
                new InvalidDecisionTypeException(
                    message: "Invalid decision type. Please correct the errors and try again.");

            invalidDecisionTypeException.AddData(
                key: nameof(DecisionType.Id),
                values: "Id is invalid");

            invalidDecisionTypeException.AddData(
                key: nameof(DecisionType.Name),
                values: "Text is invalid");

            invalidDecisionTypeException.AddData(
                key: nameof(DecisionType.CreatedDate),
                values:
                [
                    "Date is required",
                    $"Date is not recent. Expected a value between " +
                        $"{startDate} and {endDate} but found {invalidDecisionType.CreatedDate}"
                ]);

            invalidDecisionTypeException.AddData(
                key: nameof(DecisionType.CreatedBy),
                values:
                [
                    "Text is invalid",
                    $"Expected value to be '{randomUser.UserId}' but found " +
                        $"'{invalidDecisionType.CreatedBy}'."
                ]);

            invalidDecisionTypeException.AddData(
                key: nameof(DecisionType.UpdatedDate),
                values: "Date is required");

            invalidDecisionTypeException.AddData(
                key: nameof(DecisionType.UpdatedBy),
                values: "Text is invalid");

            var expectedDecisionTypeValidationException =
                new DecisionTypeValidationException(
                    message: "DecisionType validation error occurred, please fix errors and try again.",
                    innerException: invalidDecisionTypeException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomUser);


            // when
            ValueTask<DecisionType> addDecisionTypeTask =
                this.decisionTypeService.AddDecisionTypeAsync(invalidDecisionType);

            DecisionTypeValidationException actualDecisionTypeValidationException =
                await Assert.ThrowsAsync<DecisionTypeValidationException>(testCode: addDecisionTypeTask.AsTask);

            // then
            actualDecisionTypeValidationException.Should()
                .BeEquivalentTo(expectedDecisionTypeValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedDecisionTypeValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertDecisionTypeAsync(It.IsAny<DecisionType>()),
                    Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfDecisionTypeHasInvalidLengthProperty()
        {
            // given
            User randomUser = CreateRandomUser(GetRandomStringWithLengthOf(256));
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            DecisionType invalidDecisionType = CreateRandomDecisionType(randomDateTimeOffset, randomUser.UserId);
            invalidDecisionType.Name = GetRandomStringWithLengthOf(256);
            invalidDecisionType.CreatedBy = randomUser.UserId;
            invalidDecisionType.UpdatedBy = randomUser.UserId;

            var invalidDecisionTypeException = new InvalidDecisionTypeException(
                message: "Invalid decision type. Please correct the errors and try again.");

            invalidDecisionTypeException.AddData(
                key: nameof(DecisionType.Name),
                values: $"Text exceed max length of {invalidDecisionType.Name.Length - 1} characters");

            invalidDecisionTypeException.AddData(
                key: nameof(DecisionType.CreatedBy),
                values: $"Text exceed max length of {invalidDecisionType.CreatedBy.Length - 1} characters");

            invalidDecisionTypeException.AddData(
                key: nameof(DecisionType.UpdatedBy),
                values: $"Text exceed max length of {invalidDecisionType.UpdatedBy.Length - 1} characters");

            var expectedDecisionTypeValidationException =
                new DecisionTypeValidationException(
                    message: "DecisionType validation error occurred, please fix errors and try again.",
                    innerException: invalidDecisionTypeException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomUser);

            // when
            ValueTask<DecisionType> addDecisionTypeTask =
                this.decisionTypeService.AddDecisionTypeAsync(invalidDecisionType);

            DecisionTypeValidationException actualDecisionTypeValidationException =
                await Assert.ThrowsAsync<DecisionTypeValidationException>(
                    testCode: addDecisionTypeTask.AsTask);

            // then
            actualDecisionTypeValidationException.Should()
                .BeEquivalentTo(expectedDecisionTypeValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedDecisionTypeValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertDecisionTypeAsync(It.IsAny<DecisionType>()),
                    Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfCreateAndUpdateDatesIsNotSameAndLogItAsync()
        {
            // given
            int randomNumber = GetRandomNumber();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            User randomEntraUser = CreateRandomUser();

            DecisionType randomDecisionType = CreateRandomDecisionType(
                dateTimeOffset: randomDateTimeOffset,
                userId: randomEntraUser.UserId);

            DecisionType invalidDecisionType = randomDecisionType;

            invalidDecisionType.UpdatedDate =
                invalidDecisionType.CreatedDate.AddDays(randomNumber);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomEntraUser);

            var invalidDecisionTypeException =
                new InvalidDecisionTypeException(
                    message: "Invalid decision type. Please correct the errors and try again.");

            invalidDecisionTypeException.AddData(
                key: nameof(DecisionType.UpdatedDate),
                values: $"Date is not the same as {nameof(DecisionType.CreatedDate)}");

            var expectedDecisionTypeValidationException =
                new DecisionTypeValidationException(
                    message: "DecisionType validation error occurred, please fix errors and try again.",
                    innerException: invalidDecisionTypeException);

            // when
            ValueTask<DecisionType> addDecisionTypeTask =
                this.decisionTypeService.AddDecisionTypeAsync(invalidDecisionType);

            DecisionTypeValidationException actualDecisionTypeValidationException =
                await Assert.ThrowsAsync<DecisionTypeValidationException>(() =>
                    addDecisionTypeTask.AsTask());

            // then
            actualDecisionTypeValidationException.Should()
                .BeEquivalentTo(expectedDecisionTypeValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once());

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedDecisionTypeValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertDecisionTypeAsync(It.IsAny<DecisionType>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfCreateAndUpdateUsersIsNotSameAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            User randomEntraUser = CreateRandomUser();

            DecisionType randomDecisionType = CreateRandomDecisionType(
                dateTimeOffset: randomDateTimeOffset,
                userId: randomEntraUser.UserId);

            DecisionType invalidDecisionType = randomDecisionType;
            invalidDecisionType.UpdatedBy = Guid.NewGuid().ToString();

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomEntraUser);

            var invalidDecisionTypeException =
                new InvalidDecisionTypeException(
                    message: "Invalid decision type. Please correct the errors and try again.");

            invalidDecisionTypeException.AddData(
                key: nameof(DecisionType.UpdatedBy),
                values: $"Text is not the same as {nameof(DecisionType.CreatedBy)}");

            var expectedDecisionTypeValidationException =
                new DecisionTypeValidationException(
                    message: "DecisionType validation error occurred, please fix errors and try again.",
                    innerException: invalidDecisionTypeException);

            // when
            ValueTask<DecisionType> addDecisionTypeTask =
                this.decisionTypeService.AddDecisionTypeAsync(invalidDecisionType);

            DecisionTypeValidationException actualDecisionTypeValidationException =
                await Assert.ThrowsAsync<DecisionTypeValidationException>(() =>
                    addDecisionTypeTask.AsTask());

            // then
            actualDecisionTypeValidationException.Should()
                .BeEquivalentTo(expectedDecisionTypeValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once());

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedDecisionTypeValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertDecisionTypeAsync(It.IsAny<DecisionType>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(-91)]
        public async Task ShouldThrowValidationExceptionOnAddIfCreatedDateIsNotRecentAndLogItAsync(
            int invalidSeconds)
        {
            // given
            DateTimeOffset randomDateTime =
                GetRandomDateTimeOffset();

            DateTimeOffset now = randomDateTime;
            DateTimeOffset startDate = now.AddSeconds(-90);
            DateTimeOffset endDate = now.AddSeconds(0);
            User randomUser = CreateRandomUser();
            DecisionType randomDecisionType = CreateRandomDecisionType(randomDateTime, randomUser.UserId);
            DecisionType invalidDecisionType = randomDecisionType;

            DateTimeOffset invalidDate =
                now.AddSeconds(invalidSeconds);

            invalidDecisionType.CreatedDate = invalidDate;
            invalidDecisionType.UpdatedDate = invalidDate;

            var invalidDecisionTypeException = new InvalidDecisionTypeException(
                message: "Invalid decision type. Please correct the errors and try again.");

            invalidDecisionTypeException.AddData(
            key: nameof(DecisionType.CreatedDate),
                values:
                    $"Date is not recent. Expected a value between " +
                    $"{startDate} and {endDate} but found {invalidDate}");

            var expectedDecisionTypeValidationException =
                new DecisionTypeValidationException(
                    message: "DecisionType validation error occurred, please fix errors and try again.",
                    innerException: invalidDecisionTypeException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(now);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomUser);

            // when
            ValueTask<DecisionType> addDecisionTypeTask =
                this.decisionTypeService.AddDecisionTypeAsync(invalidDecisionType);

            DecisionTypeValidationException actualDecisionTypeValidationException =
                await Assert.ThrowsAsync<DecisionTypeValidationException>(
                    testCode: addDecisionTypeTask.AsTask);

            // then
            actualDecisionTypeValidationException.Should().BeEquivalentTo(
                expectedDecisionTypeValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedDecisionTypeValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertDecisionTypeAsync(It.IsAny<DecisionType>()),
                    Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
