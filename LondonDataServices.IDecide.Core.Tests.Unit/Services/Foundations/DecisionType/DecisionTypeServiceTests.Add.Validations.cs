// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using LondonDataServices.IDecide.Core.Models.Foundations.DecisionType;
using LondonDataServices.IDecide.Core.Models.Foundations.DecisionType.Exceptions;
using LondonDataServices.IDecide.Core.Models.Securities;
using Moq;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.DecisionType
{
    public partial class DecisionTypeServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfDecisionTypeIsNullAndLogItAsync()
        {
            // given
            DecisionType nullDecisionType = null;

            var nullDecisionTypeException =
                new NullDecisionTypeException(message: "DecisionType is null.");

            var expectedDecisionTypeValidationException =
                new DecisionTypeValidationException(
                    message: "DecisionType validation errors occurred, please try again.",
                    innerException: nullDecisionTypeException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyAddAuditValuesAsync(nullDecisionType))
                    .ReturnsAsync(nullDecisionType);

            // when
            ValueTask<DecisionType> addDecisionTypeTask =
                this.decisionTypeService.AddDecisionTypeAsync(nullDecisionType);

            DecisionTypeValidationException actualDecisionTypeValidationException =
                await Assert.ThrowsAsync<DecisionTypeValidationException>(() =>
                    addDecisionTypeTask.AsTask());

            // then
            actualDecisionTypeValidationException.Should()
                .BeEquivalentTo(expectedDecisionTypeValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyAddAuditValuesAsync(nullDecisionType),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedDecisionTypeValidationException))),
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
        public async Task ShouldThrowValidationExceptionOnAddIfDecisionTypeIsInvalidAndLogItAsync(string invalidText)
        {
            // given
            string randomUserId = GetRandomString();
            User randomUser = CreateRandomUser(userId: randomUserId);

            var invalidDecisionType = new DecisionType
            {
                // TODO: Add more properties for validation checks as needed
                // Name = invalidText
            };

            var invalidDecisionTypeException =
                new InvalidDecisionTypeException(
                    message: "Invalid decisionType. Please correct the errors and try again.");

            invalidDecisionTypeException.AddData(
                key: nameof(DecisionType.Id),
                values: "Id is required");

            // TODO: Add more validation checks as needed
            // invalidDecisionTypeException.AddData(
            //     key: nameof(DecisionType.Name),
            //     values: "Text is required");

            invalidDecisionTypeException.AddData(
                key: nameof(DecisionType.CreatedDate),
                values: "Date is required");

            invalidDecisionTypeException.AddData(
                key: nameof(DecisionType.CreatedBy),
                values:
                    [
                        "Text is required",
                        $"Expected value to be '{randomUserId}' but found '{invalidDecisionType.CreatedBy}'."
                    ]);

            invalidDecisionTypeException.AddData(
                key: nameof(DecisionType.UpdatedDate),
                values: "Date is required");

            invalidDecisionTypeException.AddData(
                key: nameof(DecisionType.UpdatedBy),
                values: "Text is required");

            var expectedDecisionTypeValidationException =
                new DecisionTypeValidationException(
                    message: "DecisionType validation errors occurred, please try again.",
                    innerException: invalidDecisionTypeException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyAddAuditValuesAsync(invalidDecisionType))
                    .ReturnsAsync(invalidDecisionType);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomUser);

            // when
            ValueTask<DecisionType> addDecisionTypeTask =
                this.decisionTypeService.AddDecisionTypeAsync(invalidDecisionType);

            DecisionTypeValidationException actualDecisionTypeValidationException =
                await Assert.ThrowsAsync<DecisionTypeValidationException>(() =>
                    addDecisionTypeTask.AsTask());

            // then
            actualDecisionTypeValidationException.Should()
                .BeEquivalentTo(expectedDecisionTypeValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyAddAuditValuesAsync(invalidDecisionType),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once());

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
            DecisionType randomDecisionType = CreateRandomDecisionType(randomDateTimeOffset, userId: randomUserId);
            DecisionType invalidDecisionType = randomDecisionType;

            invalidDecisionType.UpdatedDate =
                invalidDecisionType.CreatedDate.AddDays(randomNumber);

            var invalidDecisionTypeException =
                new InvalidDecisionTypeException(
                    message: "Invalid decisionType. Please correct the errors and try again.");

            invalidDecisionTypeException.AddData(
                key: nameof(DecisionType.UpdatedDate),
                values: $"Date is not the same as {nameof(DecisionType.CreatedDate)}");

            var expectedDecisionTypeValidationException =
                new DecisionTypeValidationException(
                    message: "DecisionType validation errors occurred, please try again.",
                    innerException: invalidDecisionTypeException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyAddAuditValuesAsync(invalidDecisionType))
                    .ReturnsAsync(invalidDecisionType);

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
                await Assert.ThrowsAsync<DecisionTypeValidationException>(() =>
                    addDecisionTypeTask.AsTask());

            // then
            actualDecisionTypeValidationException.Should()
                .BeEquivalentTo(expectedDecisionTypeValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyAddAuditValuesAsync(invalidDecisionType),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once());

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
            DecisionType randomDecisionType = CreateRandomDecisionType(randomDateTimeOffset, userId: randomUserId);
            DecisionType invalidDecisionType = randomDecisionType.DeepClone();
            invalidDecisionType.UpdatedBy = Guid.NewGuid().ToString();

            var invalidDecisionTypeException =
                new InvalidDecisionTypeException(
                    message: "Invalid decisionType. Please correct the errors and try again.");

            invalidDecisionTypeException.AddData(
                key: nameof(DecisionType.UpdatedBy),
                values: $"Text is not the same as {nameof(DecisionType.CreatedBy)}");

            var expectedDecisionTypeValidationException =
                new DecisionTypeValidationException(
                    message: "DecisionType validation errors occurred, please try again.",
                    innerException: invalidDecisionTypeException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyAddAuditValuesAsync(invalidDecisionType))
                    .ReturnsAsync(invalidDecisionType);

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
                await Assert.ThrowsAsync<DecisionTypeValidationException>(() =>
                    addDecisionTypeTask.AsTask());

            // then
            actualDecisionTypeValidationException.Should()
                .BeEquivalentTo(expectedDecisionTypeValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyAddAuditValuesAsync(invalidDecisionType),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once());

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
            DecisionType randomDecisionType = CreateRandomDecisionType(invalidDateTime, userId: randomUserId);
            DecisionType invalidDecisionType = randomDecisionType;

            var invalidDecisionTypeException =
                new InvalidDecisionTypeException(
                    message: "Invalid decisionType. Please correct the errors and try again.");

            invalidDecisionTypeException.AddData(
                key: nameof(DecisionType.CreatedDate),
                values:
                    $"Date is not recent. Expected a value between {startDate} and {endDate} but found {invalidDate}");

            var expectedDecisionTypeValidationException =
                new DecisionTypeValidationException(
                    message: "DecisionType validation errors occurred, please try again.",
                    innerException: invalidDecisionTypeException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyAddAuditValuesAsync(invalidDecisionType))
                    .ReturnsAsync(invalidDecisionType);

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
                await Assert.ThrowsAsync<DecisionTypeValidationException>(() =>
                    addDecisionTypeTask.AsTask());

            // then
            actualDecisionTypeValidationException.Should()
                .BeEquivalentTo(expectedDecisionTypeValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyAddAuditValuesAsync(invalidDecisionType),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once());

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

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}