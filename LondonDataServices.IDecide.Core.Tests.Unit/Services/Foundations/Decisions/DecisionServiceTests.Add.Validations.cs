// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using LondonDataServices.IDecide.Core.Models.Foundations.Decisions;
using LondonDataServices.IDecide.Core.Models.Foundations.Decisions.Exceptions;
using LondonDataServices.IDecide.Core.Models.Securities;
using Moq;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.Decisions
{
    public partial class DecisionServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfDecisionIsNullAndLogItAsync()
        {
            // given
            Decision nullDecision = null;

            var nullDecisionException =
                new NullDecisionException(message: "Decision is null.");

            var expectedDecisionValidationException =
                new DecisionValidationException(
                    message: "Decision validation errors occurred, please try again.",
                    innerException: nullDecisionException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyAddAuditValuesAsync(nullDecision))
                    .ReturnsAsync(nullDecision);

            // when
            ValueTask<Decision> addDecisionTask =
                this.decisionService.AddDecisionAsync(nullDecision);

            DecisionValidationException actualDecisionValidationException =
                await Assert.ThrowsAsync<DecisionValidationException>(() =>
                    addDecisionTask.AsTask());

            // then
            actualDecisionValidationException.Should()
                .BeEquivalentTo(expectedDecisionValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyAddAuditValuesAsync(nullDecision),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedDecisionValidationException))),
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
        public async Task ShouldThrowValidationExceptionOnAddIfDecisionIsInvalidAndLogItAsync(string invalidText)
        {
            // given
            string randomUserId = GetRandomString();
            User randomUser = CreateRandomUser(userId: randomUserId);

            var invalidDecision = new Decision
            {
                // TODO: Add more properties for validation checks as needed
                // Name = invalidText
            };

            var invalidDecisionException =
                new InvalidDecisionException(
                    message: "Invalid decision. Please correct the errors and try again.");

            invalidDecisionException.AddData(
                key: nameof(Decision.Id),
                values: "Id is required");

            // TODO: Add more validation checks as needed
            // invalidDecisionException.AddData(
            //     key: nameof(Decision.Name),
            //     values: "Text is required");

            invalidDecisionException.AddData(
                key: nameof(Decision.CreatedDate),
                values: "Date is required");

            invalidDecisionException.AddData(
                key: nameof(Decision.CreatedBy),
                values:
                    [
                        "Text is required",
                        $"Expected value to be '{randomUserId}' but found '{invalidDecision.CreatedBy}'."
                    ]);

            invalidDecisionException.AddData(
                key: nameof(Decision.UpdatedDate),
                values: "Date is required");

            invalidDecisionException.AddData(
                key: nameof(Decision.UpdatedBy),
                values: "Text is required");

            var expectedDecisionValidationException =
                new DecisionValidationException(
                    message: "Decision validation errors occurred, please try again.",
                    innerException: invalidDecisionException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyAddAuditValuesAsync(invalidDecision))
                    .ReturnsAsync(invalidDecision);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomUser);

            // when
            ValueTask<Decision> addDecisionTask =
                this.decisionService.AddDecisionAsync(invalidDecision);

            DecisionValidationException actualDecisionValidationException =
                await Assert.ThrowsAsync<DecisionValidationException>(() =>
                    addDecisionTask.AsTask());

            // then
            actualDecisionValidationException.Should()
                .BeEquivalentTo(expectedDecisionValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyAddAuditValuesAsync(invalidDecision),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once());

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedDecisionValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertDecisionAsync(It.IsAny<Decision>()),
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
            Decision randomDecision = CreateRandomDecision(randomDateTimeOffset, userId: randomUserId);
            Decision invalidDecision = randomDecision;

            invalidDecision.UpdatedDate =
                invalidDecision.CreatedDate.AddDays(randomNumber);

            var invalidDecisionException =
                new InvalidDecisionException(
                    message: "Invalid decision. Please correct the errors and try again.");

            invalidDecisionException.AddData(
                key: nameof(Decision.UpdatedDate),
                values: $"Date is not the same as {nameof(Decision.CreatedDate)}");

            var expectedDecisionValidationException =
                new DecisionValidationException(
                    message: "Decision validation errors occurred, please try again.",
                    innerException: invalidDecisionException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyAddAuditValuesAsync(invalidDecision))
                    .ReturnsAsync(invalidDecision);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomUser);

            // when
            ValueTask<Decision> addDecisionTask =
                this.decisionService.AddDecisionAsync(invalidDecision);

            DecisionValidationException actualDecisionValidationException =
                await Assert.ThrowsAsync<DecisionValidationException>(() =>
                    addDecisionTask.AsTask());

            // then
            actualDecisionValidationException.Should()
                .BeEquivalentTo(expectedDecisionValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyAddAuditValuesAsync(invalidDecision),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once());

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedDecisionValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertDecisionAsync(It.IsAny<Decision>()),
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
            Decision randomDecision = CreateRandomDecision(randomDateTimeOffset, userId: randomUserId);
            Decision invalidDecision = randomDecision.DeepClone();
            invalidDecision.UpdatedBy = Guid.NewGuid().ToString();

            var invalidDecisionException =
                new InvalidDecisionException(
                    message: "Invalid decision. Please correct the errors and try again.");

            invalidDecisionException.AddData(
                key: nameof(Decision.UpdatedBy),
                values: $"Text is not the same as {nameof(Decision.CreatedBy)}");

            var expectedDecisionValidationException =
                new DecisionValidationException(
                    message: "Decision validation errors occurred, please try again.",
                    innerException: invalidDecisionException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyAddAuditValuesAsync(invalidDecision))
                    .ReturnsAsync(invalidDecision);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomUser);

            // when
            ValueTask<Decision> addDecisionTask =
                this.decisionService.AddDecisionAsync(invalidDecision);

            DecisionValidationException actualDecisionValidationException =
                await Assert.ThrowsAsync<DecisionValidationException>(() =>
                    addDecisionTask.AsTask());

            // then
            actualDecisionValidationException.Should()
                .BeEquivalentTo(expectedDecisionValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyAddAuditValuesAsync(invalidDecision),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once());

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedDecisionValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertDecisionAsync(It.IsAny<Decision>()),
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
            Decision randomDecision = CreateRandomDecision(invalidDateTime, userId: randomUserId);
            Decision invalidDecision = randomDecision;

            var invalidDecisionException =
                new InvalidDecisionException(
                    message: "Invalid decision. Please correct the errors and try again.");

            invalidDecisionException.AddData(
                key: nameof(Decision.CreatedDate),
                values:
                    $"Date is not recent. Expected a value between {startDate} and {endDate} but found {invalidDate}");

            var expectedDecisionValidationException =
                new DecisionValidationException(
                    message: "Decision validation errors occurred, please try again.",
                    innerException: invalidDecisionException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyAddAuditValuesAsync(invalidDecision))
                    .ReturnsAsync(invalidDecision);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomUser);

            // when
            ValueTask<Decision> addDecisionTask =
                this.decisionService.AddDecisionAsync(invalidDecision);

            DecisionValidationException actualDecisionValidationException =
                await Assert.ThrowsAsync<DecisionValidationException>(() =>
                    addDecisionTask.AsTask());

            // then
            actualDecisionValidationException.Should()
                .BeEquivalentTo(expectedDecisionValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyAddAuditValuesAsync(invalidDecision),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once());

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedDecisionValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertDecisionAsync(It.IsAny<Decision>()),
                    Times.Never);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}