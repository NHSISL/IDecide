// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using LondonDataServices.IDecide.Core.Models.Foundations.DecisionTypes;
using LondonDataServices.IDecide.Core.Models.Foundations.DecisionTypes.Exceptions;
using LondonDataServices.IDecide.Core.Models.Securities;
using Moq;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.DecisionTypes
{
    public partial class DecisionTypeServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyDecisionTypeAsync()
        {
            // given
            DecisionType nullDecisionType = null;
            var nullDecisionTypeException = new NullDecisionTypeException(message: "DecisionType is null.");

            var expectedDecisionTypeValidationException =
                new DecisionTypeValidationException(
                    message: "DecisionType validation error occurred, please fix errors and try again.",
                    innerException: nullDecisionTypeException);

            // when
            ValueTask<DecisionType> modifyDecisionTypeTask = 
                this.decisionTypeService.ModifyDecisionTypeAsync(nullDecisionType);

            DecisionTypeValidationException actualDecisionTypeValidationException =
                await Assert.ThrowsAsync<DecisionTypeValidationException>(testCode: modifyDecisionTypeTask.AsTask);

            // then
            actualDecisionTypeValidationException.Should().BeEquivalentTo(expectedDecisionTypeValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedDecisionTypeValidationException))), 
                    Times.Once());

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Never);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateDecisionTypeAsync(It.IsAny<DecisionType>()),
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
        public async Task ShouldThrowValidationExceptionOnModifyIfDecisionTypeIsInvalidAndLogItAsync(string invalidText)
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
                values:"Date is required");

            invalidDecisionTypeException.AddData(
                key: nameof(DecisionType.CreatedBy),
                values: "Text is invalid");

            invalidDecisionTypeException.AddData(
                key: nameof(DecisionType.UpdatedDate),
                values:
                [
                    "Date is required",
                    "Date is the same as CreatedDate",
                    $"Date is not recent. Expected a value between {startDate} and {endDate} but found " +
                    $"{invalidDecisionType.UpdatedDate}"
                ]);

            invalidDecisionTypeException.AddData(
                key: nameof(DecisionType.UpdatedBy),
                values:
                [
                    "Text is invalid",
                    $"Expected value to be '{randomUser.UserId}' but found " +
                        $"'{invalidDecisionType.CreatedBy}'."
                ]);

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
            ValueTask<DecisionType> modifyDecisionTypeTask =
                this.decisionTypeService.ModifyDecisionTypeAsync(invalidDecisionType);

            DecisionTypeValidationException actualDecisionTypeValidationException =
                await Assert.ThrowsAsync<DecisionTypeValidationException>(testCode: modifyDecisionTypeTask.AsTask);

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
                broker.UpdateDecisionTypeAsync(It.IsAny<DecisionType>()),
                    Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfDecisionTypeHasInvalidLengthProperty()
        {
            // given
            User randomUser = CreateRandomUser(GetRandomStringWithLengthOf(256));
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            DecisionType invalidDecisionType = CreateRandomModifyDecisionType(randomDateTimeOffset, randomUser.UserId);
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
            ValueTask<DecisionType> modifyDecisionTypeTask =
                this.decisionTypeService.ModifyDecisionTypeAsync(invalidDecisionType);

            DecisionTypeValidationException actualDecisionTypeValidationException =
                await Assert.ThrowsAsync<DecisionTypeValidationException>(
                    testCode: modifyDecisionTypeTask.AsTask);

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
                broker.UpdateDecisionTypeAsync(It.IsAny<DecisionType>()),
                    Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdatedDateIsSameAsCreatedDateAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            User randomUser = CreateRandomUser();

            DecisionType randomDecisionType = CreateRandomDecisionType(
                dateTimeOffset: randomDateTimeOffset,
                userId: randomUser.UserId);

            DecisionType invalidDecisionType = randomDecisionType;

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomUser);

            var invalidDecisionTypeException =
                new InvalidDecisionTypeException(
                    message: "Invalid decision type. Please correct the errors and try again.");

            invalidDecisionTypeException.AddData(
                key: nameof(DecisionType.UpdatedDate),
                values: $"Date is the same as {nameof(DecisionType.CreatedDate)}");

            var expectedDecisionTypeValidationException =
                new DecisionTypeValidationException(
                    message: "DecisionType validation error occurred, please fix errors and try again.",
                    innerException: invalidDecisionTypeException);

            // when
            ValueTask<DecisionType> modifyDecisionTypeTask =
                this.decisionTypeService.ModifyDecisionTypeAsync(invalidDecisionType);

            DecisionTypeValidationException actualDecisionTypeValidationException =
                await Assert.ThrowsAsync<DecisionTypeValidationException>(
                    modifyDecisionTypeTask.AsTask);

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
                broker.SelectDecisionTypeByIdAsync(invalidDecisionType.Id),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(-91)]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdatedDateIsNotRecentAndLogItAsync(
            int invalidSeconds)
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            DateTimeOffset now = randomDateTimeOffset;
            DateTimeOffset startDate = now.AddSeconds(-90);
            DateTimeOffset endDate = now.AddSeconds(0);
            User randomUser = CreateRandomUser();

            DecisionType randomDecisionType = CreateRandomDecisionType(
                dateTimeOffset: randomDateTimeOffset,
                userId: randomUser.UserId);

            DecisionType invalidDecisionType = randomDecisionType;
            invalidDecisionType.UpdatedDate = randomDateTimeOffset.AddSeconds(invalidSeconds);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomUser);

            var invalidDecisionTypeException = new InvalidDecisionTypeException(
                message: "Invalid decision type. Please correct the errors and try again.");

            invalidDecisionTypeException.AddData(
                key: nameof(DecisionType.UpdatedDate),
                values:
                [
                    $"Date is not recent." +
                    $" Expected a value between {startDate} and {endDate} but found {randomDecisionType.UpdatedDate}"
                ]);

            var expectedDecisionTypeValidationException = new DecisionTypeValidationException(
                message: "DecisionType validation error occurred, please fix errors and try again.",
                innerException: invalidDecisionTypeException);

            // when
            ValueTask<DecisionType> modifyDecisionTypeTask =
                this.decisionTypeService.ModifyDecisionTypeAsync(invalidDecisionType);

            DecisionTypeValidationException actualDecisionTypeVaildationException =
                await Assert.ThrowsAsync<DecisionTypeValidationException>(
                    testCode: modifyDecisionTypeTask.AsTask);

            // then
            actualDecisionTypeVaildationException.Should().BeEquivalentTo(expectedDecisionTypeValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(
                   SameExceptionAs(expectedDecisionTypeValidationException))),
                       Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfDecisionTypeDoesNotExistAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            User randomUser = CreateRandomUser();

            DecisionType randomDecisionType = CreateRandomModifyDecisionType(
                dateTimeOffset: randomDateTimeOffset,
                userId: randomUser.UserId);

            DecisionType nonExistDecisionType = randomDecisionType;
            DecisionType nullDecisionType = null;

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomUser);

            var notFoundDecisionTypeException =
            new NotFoundDecisionTypeException(
                    $"Couldn't find decision type with decisionTypeId: {nonExistDecisionType.Id}.");

            var expectedDecisionTypeValidationException =
                new DecisionTypeValidationException(
                    message: "DecisionType validation error occurred, please fix errors and try again.",
                    innerException: notFoundDecisionTypeException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectDecisionTypeByIdAsync(nonExistDecisionType.Id))
                .ReturnsAsync(nullDecisionType);

            // when 
            ValueTask<DecisionType> modifyDecisionTypeTask =
                this.decisionTypeService.ModifyDecisionTypeAsync(nonExistDecisionType);

            DecisionTypeValidationException actualDecisionTypeValidationException =
                await Assert.ThrowsAsync<DecisionTypeValidationException>(
                    modifyDecisionTypeTask.AsTask);

            // then
            actualDecisionTypeValidationException.Should()
                .BeEquivalentTo(expectedDecisionTypeValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectDecisionTypeByIdAsync(nonExistDecisionType.Id),
                    Times.Once);

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

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfStorageCreatedDateNotSameAsCreatedDateAndLogItAsync()
        {
            // given
            int randomNumber = GetRandomNegativeNumber();
            int randomMinutes = randomNumber;
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            User randomUser = CreateRandomUser();

            DecisionType randomDecisionType = CreateRandomModifyDecisionType(
                dateTimeOffset: randomDateTimeOffset,
                userId: randomUser.UserId);

            DecisionType invalidDecisionType = randomDecisionType.DeepClone();
            DecisionType storageDecisionType = invalidDecisionType.DeepClone();
            storageDecisionType.CreatedDate = storageDecisionType.CreatedDate.AddMinutes(randomMinutes);
            storageDecisionType.UpdatedDate = storageDecisionType.UpdatedDate.AddMinutes(randomMinutes);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomUser);

            var invalidDecisionTypeException =
                new InvalidDecisionTypeException(
                    message: "Invalid decision type. Please correct the errors and try again.");

            invalidDecisionTypeException.AddData(
                key: nameof(DecisionType.CreatedDate),
                values: $"Date is not the same as {nameof(DecisionType.CreatedDate)}");

            var expectedDecisionTypeValidationException =
                new DecisionTypeValidationException(
                    message: "DecisionType validation error occurred, please fix errors and try again.",
                    innerException: invalidDecisionTypeException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectDecisionTypeByIdAsync(invalidDecisionType.Id))
                .ReturnsAsync(storageDecisionType);

            // when
            ValueTask<DecisionType> modifyDecisionTypeTask =
                this.decisionTypeService.ModifyDecisionTypeAsync(invalidDecisionType);

            DecisionTypeValidationException actualDecisionTypeValidationException =
                await Assert.ThrowsAsync<DecisionTypeValidationException>(
                    modifyDecisionTypeTask.AsTask);

            // then
            actualDecisionTypeValidationException.Should()
                .BeEquivalentTo(expectedDecisionTypeValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectDecisionTypeByIdAsync(invalidDecisionType.Id),
                    Times.Once);

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

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfCreatedUserDontMacthStorageAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            User randomUser = CreateRandomUser();

            DecisionType randomDecisionType = CreateRandomModifyDecisionType(
                dateTimeOffset: randomDateTimeOffset,
                userId: randomUser.UserId);

            DecisionType invalidDecisionType = randomDecisionType.DeepClone();
            DecisionType storageDecisionType = invalidDecisionType.DeepClone();
            invalidDecisionType.CreatedBy = Guid.NewGuid().ToString();
            storageDecisionType.UpdatedDate = storageDecisionType.CreatedDate;

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomUser);

            var invalidDecisionTypeException =
                new InvalidDecisionTypeException(
                    message: "Invalid decision type. Please correct the errors and try again.");

            invalidDecisionTypeException.AddData(
                key: nameof(DecisionType.CreatedBy),
                values: $"Text is not the same as {nameof(DecisionType.CreatedBy)}");

            var expectedDecisionTypeValidationException =
                new DecisionTypeValidationException(
                    message: "DecisionType validation error occurred, please fix errors and try again.",
                    innerException: invalidDecisionTypeException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectDecisionTypeByIdAsync(invalidDecisionType.Id))
                .ReturnsAsync(storageDecisionType);

            // when
            ValueTask<DecisionType> modifyDecisionTypeTask =
                this.decisionTypeService.ModifyDecisionTypeAsync(invalidDecisionType);

            DecisionTypeValidationException actualDecisionTypeValidationException =
                await Assert.ThrowsAsync<DecisionTypeValidationException>(
                    modifyDecisionTypeTask.AsTask);

            // then
            actualDecisionTypeValidationException.Should().BeEquivalentTo(expectedDecisionTypeValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectDecisionTypeByIdAsync(invalidDecisionType.Id),
                    Times.Once);

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

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfStorageUpdatedDateSameAsUpdatedDateAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            User randomUser = CreateRandomUser();

            DecisionType randomDecisionType = CreateRandomModifyDecisionType(
                dateTimeOffset: randomDateTimeOffset,
                userId: randomUser.UserId);

            DecisionType invalidDecisionType = randomDecisionType;
            DecisionType storageDecisionType = randomDecisionType.DeepClone();

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomUser);

            var invalidDecisionTypeException =
                new InvalidDecisionTypeException(
                    message: "Invalid decision type. Please correct the errors and try again.");

            invalidDecisionTypeException.AddData(
                key: nameof(DecisionType.UpdatedDate),
                values: $"Date is the same as {nameof(DecisionType.UpdatedDate)}");

            var expectedDecisionTypeValidationException =
                new DecisionTypeValidationException(
                    message: "DecisionType validation error occurred, please fix errors and try again.",
                    innerException: invalidDecisionTypeException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectDecisionTypeByIdAsync(invalidDecisionType.Id))
                .ReturnsAsync(storageDecisionType);

            // when
            ValueTask<DecisionType> modifyDecisionTypeTask =
                this.decisionTypeService.ModifyDecisionTypeAsync(invalidDecisionType);

            // then
            await Assert.ThrowsAsync<DecisionTypeValidationException>(
                modifyDecisionTypeTask.AsTask);

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
                broker.SelectDecisionTypeByIdAsync(invalidDecisionType.Id),
                    Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}