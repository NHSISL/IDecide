// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using LondonDataServices.IDecide.Core.Models.Foundations.DecisionTypes;
using Moq;
using StandardlyTestProject.Api.Models.Foundations.DecisionTypes.Exceptions;

namespace StandardlyTestProject.Api.Tests.Unit.Services.Foundations.DecisionTypes
{
    public partial class DecisionTypeServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfDecisionTypeIsNullAndLogItAsync()
        {
            // given
            DecisionType nullDecisionType = null;
            var nullDecisionTypeException = new NullDecisionTypeException(message: "DecisionType is null.");

            var expectedDecisionTypeValidationException =
                new DecisionTypeValidationException(
                    message: "DecisionType validation errors occurred, please try again.",
                    innerException: nullDecisionTypeException);

            // when
            ValueTask<DecisionType> modifyDecisionTypeTask =
                this.decisionTypeService.ModifyDecisionTypeAsync(nullDecisionType);

            DecisionTypeValidationException actualDecisionTypeValidationException =
                await Assert.ThrowsAsync<DecisionTypeValidationException>(
                    modifyDecisionTypeTask.AsTask);

            // then
            actualDecisionTypeValidationException.Should()
                .BeEquivalentTo(expectedDecisionTypeValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedDecisionTypeValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateDecisionTypeAsync(It.IsAny<DecisionType>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnModifyIfDecisionTypeIsInvalidAndLogItAsync(string invalidText)
        {
            // given 
            var invalidDecisionType = new DecisionType
            {
                // TODO:  Add default values for your properties i.e. Name = invalidText
            };

            var invalidDecisionTypeException =
                new InvalidDecisionTypeException(
                    message: "Invalid decisionType. Please correct the errors and try again.");

            invalidDecisionTypeException.AddData(
                key: nameof(DecisionType.Id),
                values: "Id is required");

            //invalidDecisionTypeException.AddData(
            //    key: nameof(DecisionType.Name),
            //    values: "Text is required");

            // TODO: Add or remove data here to suit the validation needs for the DecisionType model

            invalidDecisionTypeException.AddData(
                key: nameof(DecisionType.CreatedDate),
                values: "Date is required");

            invalidDecisionTypeException.AddData(
                key: nameof(DecisionType.CreatedBy),
                values: "Text is required");

            invalidDecisionTypeException.AddData(
                key: nameof(DecisionType.UpdatedDate),
                values:
                new[] {
                    "Date is required",
                    $"Date is the same as {nameof(DecisionType.CreatedDate)}"
                });

            invalidDecisionTypeException.AddData(
                key: nameof(DecisionType.UpdatedBy),
                values: "Text is required");

            var expectedDecisionTypeValidationException =
                new DecisionTypeValidationException(
                    message: "DecisionType validation errors occurred, please try again.",
                    innerException: invalidDecisionTypeException);

            // when
            ValueTask<DecisionType> modifyDecisionTypeTask =
                this.decisionTypeService.ModifyDecisionTypeAsync(invalidDecisionType);

            DecisionTypeValidationException actualDecisionTypeValidationException =
                await Assert.ThrowsAsync<DecisionTypeValidationException>(
                    modifyDecisionTypeTask.AsTask);

            //then
            actualDecisionTypeValidationException.Should()
                .BeEquivalentTo(expectedDecisionTypeValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedDecisionTypeValidationException))),
                        Times.Once());

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateDecisionTypeAsync(It.IsAny<DecisionType>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdatedDateIsSameAsCreatedDateAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            DecisionType randomDecisionType = CreateRandomDecisionType(randomDateTimeOffset);
            DecisionType invalidDecisionType = randomDecisionType;

            var invalidDecisionTypeException =
                new InvalidDecisionTypeException(
                    message: "Invalid decisionType. Please correct the errors and try again.");

            invalidDecisionTypeException.AddData(
                key: nameof(DecisionType.UpdatedDate),
                values: $"Date is the same as {nameof(DecisionType.CreatedDate)}");

            var expectedDecisionTypeValidationException =
                new DecisionTypeValidationException(
                    message: "DecisionType validation errors occurred, please try again.",
                    innerException: invalidDecisionTypeException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

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

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedDecisionTypeValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectDecisionTypeByIdAsync(invalidDecisionType.Id),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(MinutesBeforeOrAfter))]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdatedDateIsNotRecentAndLogItAsync(int minutes)
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            DecisionType randomDecisionType = CreateRandomDecisionType(randomDateTimeOffset);
            randomDecisionType.UpdatedDate = randomDateTimeOffset.AddMinutes(minutes);

            var invalidDecisionTypeException =
                new InvalidDecisionTypeException(
                    message: "Invalid decisionType. Please correct the errors and try again.");

            invalidDecisionTypeException.AddData(
                key: nameof(DecisionType.UpdatedDate),
                values: "Date is not recent");

            var expectedDecisionTypeValidatonException =
                new DecisionTypeValidationException(
                    message: "DecisionType validation errors occurred, please try again.",
                    innerException: invalidDecisionTypeException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<DecisionType> modifyDecisionTypeTask =
                this.decisionTypeService.ModifyDecisionTypeAsync(randomDecisionType);

            DecisionTypeValidationException actualDecisionTypeValidationException =
                await Assert.ThrowsAsync<DecisionTypeValidationException>(
                    modifyDecisionTypeTask.AsTask);

            // then
            actualDecisionTypeValidationException.Should()
                .BeEquivalentTo(expectedDecisionTypeValidatonException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedDecisionTypeValidatonException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectDecisionTypeByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfDecisionTypeDoesNotExistAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            DecisionType randomDecisionType = CreateRandomModifyDecisionType(randomDateTimeOffset);
            DecisionType nonExistDecisionType = randomDecisionType;
            DecisionType nullDecisionType = null;

            var notFoundDecisionTypeException = new NotFoundDecisionTypeException(
                message: $"Couldn't find decision type with decisionTypeId: {nonExistDecisionType.Id}.");

            var expectedDecisionTypeValidationException =
                new DecisionTypeValidationException(
                    message: "DecisionType validation errors occurred, please try again.",
                    innerException: notFoundDecisionTypeException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectDecisionTypeByIdAsync(nonExistDecisionType.Id))
                .ReturnsAsync(nullDecisionType);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                .ReturnsAsync(randomDateTimeOffset);

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

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedDecisionTypeValidationException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfStorageCreatedDateNotSameAsCreatedDateAndLogItAsync()
        {
            // given
            int randomNumber = GetRandomNegativeNumber();
            int randomMinutes = randomNumber;
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            DecisionType randomDecisionType = CreateRandomModifyDecisionType(randomDateTimeOffset);
            DecisionType invalidDecisionType = randomDecisionType.DeepClone();
            DecisionType storageDecisionType = invalidDecisionType.DeepClone();
            storageDecisionType.CreatedDate = storageDecisionType.CreatedDate.AddMinutes(randomMinutes);
            storageDecisionType.UpdatedDate = storageDecisionType.UpdatedDate.AddMinutes(randomMinutes);

            var invalidDecisionTypeException =
                new InvalidDecisionTypeException(
                    message: "Invalid decisionType. Please correct the errors and try again.");

            invalidDecisionTypeException.AddData(
                key: nameof(DecisionType.CreatedDate),
                values: $"Date is not the same as {nameof(DecisionType.CreatedDate)}");

            var expectedDecisionTypeValidationException =
                new DecisionTypeValidationException(
                    message: "DecisionType validation errors occurred, please try again.",
                    innerException: invalidDecisionTypeException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectDecisionTypeByIdAsync(invalidDecisionType.Id))
                .ReturnsAsync(storageDecisionType);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                .ReturnsAsync(randomDateTimeOffset);

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

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedDecisionTypeValidationException))),
                       Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfCreatedUserDontMacthStorageAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            DecisionType randomDecisionType = CreateRandomModifyDecisionType(randomDateTimeOffset);
            DecisionType invalidDecisionType = randomDecisionType.DeepClone();
            DecisionType storageDecisionType = invalidDecisionType.DeepClone();
            invalidDecisionType.CreatedBy = Guid.NewGuid().ToString();
            storageDecisionType.UpdatedDate = storageDecisionType.CreatedDate;

            var invalidDecisionTypeException =
                new InvalidDecisionTypeException(
                    message: "Invalid decisionType. Please correct the errors and try again.");

            invalidDecisionTypeException.AddData(
                key: nameof(DecisionType.CreatedBy),
                values: $"Text is not the same as {nameof(DecisionType.CreatedBy)}");

            var expectedDecisionTypeValidationException =
                new DecisionTypeValidationException(
                    message: "DecisionType validation errors occurred, please try again.",
                    innerException: invalidDecisionTypeException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectDecisionTypeByIdAsync(invalidDecisionType.Id))
                .ReturnsAsync(storageDecisionType);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                .ReturnsAsync(randomDateTimeOffset);

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

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedDecisionTypeValidationException))),
                       Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfStorageUpdatedDateSameAsUpdatedDateAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            DecisionType randomDecisionType = CreateRandomModifyDecisionType(randomDateTimeOffset);
            DecisionType invalidDecisionType = randomDecisionType;
            DecisionType storageDecisionType = randomDecisionType.DeepClone();

            var invalidDecisionTypeException =
                new InvalidDecisionTypeException(
                    message: "Invalid decisionType. Please correct the errors and try again.");

            invalidDecisionTypeException.AddData(
                key: nameof(DecisionType.UpdatedDate),
                values: $"Date is the same as {nameof(DecisionType.UpdatedDate)}");

            var expectedDecisionTypeValidationException =
                new DecisionTypeValidationException(
                    message: "DecisionType validation errors occurred, please try again.",
                    innerException: invalidDecisionTypeException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectDecisionTypeByIdAsync(invalidDecisionType.Id))
                .ReturnsAsync(storageDecisionType);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<DecisionType> modifyDecisionTypeTask =
                this.decisionTypeService.ModifyDecisionTypeAsync(invalidDecisionType);

            // then
            await Assert.ThrowsAsync<DecisionTypeValidationException>(
                modifyDecisionTypeTask.AsTask);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedDecisionTypeValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectDecisionTypeByIdAsync(invalidDecisionType.Id),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}