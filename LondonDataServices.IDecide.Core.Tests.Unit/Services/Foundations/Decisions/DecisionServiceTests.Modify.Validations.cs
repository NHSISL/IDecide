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
        public async Task ShouldThrowValidationExceptionOnModifyIfDecisionIsNullAndLogItAsync()
        {
            // given
            Decision nullDecision = null;
            var nullDecisionException = new NullDecisionException(message: "Decision is null.");

            var expectedDecisionValidationException =
                new DecisionValidationException(
                    message: "Decision validation errors occurred, please try again.",
                    innerException: nullDecisionException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyModifyAuditValueAsync(nullDecision))
                    .ReturnsAsync(nullDecision);

            // when
            ValueTask<Decision> modifyDecisionTask =
                this.decisionService.ModifyDecisionAsync(nullDecision);

            DecisionValidationException actualDecisionValidationException =
                await Assert.ThrowsAsync<DecisionValidationException>(
                    modifyDecisionTask.AsTask);

            // then
            actualDecisionValidationException.Should()
                .BeEquivalentTo(expectedDecisionValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyModifyAuditValueAsync(nullDecision),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedDecisionValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateDecisionAsync(It.IsAny<Decision>()),
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
        public async Task ShouldThrowValidationExceptionOnModifyIfDecisionIsInvalidAndLogItAsync(string invalidText)
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
                values: "Text is required");

            invalidDecisionException.AddData(
                key: nameof(Decision.UpdatedDate),
                values:
                new[] {
                    "Date is required",
                    $"Date is the same as {nameof(Decision.CreatedDate)}"
                });

            invalidDecisionException.AddData(
                key: nameof(Decision.UpdatedBy),
                values:
                    [
                        "Text is required",
                        $"Expected value to be '{randomUserId}' but found '{invalidDecision.UpdatedBy}'."
                    ]);

            var expectedDecisionValidationException =
                new DecisionValidationException(
                    message: "Decision validation errors occurred, please try again.",
                    innerException: invalidDecisionException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyModifyAuditValueAsync(invalidDecision))
                    .ReturnsAsync(invalidDecision);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomUser);

            // when
            ValueTask<Decision> modifyDecisionTask =
                this.decisionService.ModifyDecisionAsync(invalidDecision);

            DecisionValidationException actualDecisionValidationException =
                await Assert.ThrowsAsync<DecisionValidationException>(
                    modifyDecisionTask.AsTask);

            //then
            actualDecisionValidationException.Should()
                .BeEquivalentTo(expectedDecisionValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyModifyAuditValueAsync(invalidDecision),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedDecisionValidationException))),
                        Times.Once());

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateDecisionAsync(It.IsAny<Decision>()),
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

            Decision randomDecision =
                CreateRandomDecision(dateTimeOffset: randomDateTimeOffset, userId: randomUserId);

            Decision invalidDecision = randomDecision;

            var invalidDecisionException =
                new InvalidDecisionException(
                    message: "Invalid decision. Please correct the errors and try again.");

            invalidDecisionException.AddData(
                key: nameof(Decision.UpdatedDate),
                values: $"Date is the same as {nameof(Decision.CreatedDate)}");

            var expectedDecisionValidationException =
                new DecisionValidationException(
                    message: "Decision validation errors occurred, please try again.",
                    innerException: invalidDecisionException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyModifyAuditValueAsync(invalidDecision))
                    .ReturnsAsync(invalidDecision);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomUser);

            // when
            ValueTask<Decision> modifyDecisionTask =
                this.decisionService.ModifyDecisionAsync(invalidDecision);

            DecisionValidationException actualDecisionValidationException =
                await Assert.ThrowsAsync<DecisionValidationException>(
                    modifyDecisionTask.AsTask);

            // then
            actualDecisionValidationException.Should()
                .BeEquivalentTo(expectedDecisionValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyModifyAuditValueAsync(invalidDecision),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedDecisionValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectDecisionByIdAsync(invalidDecision.Id),
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

            Decision randomDecision =
                CreateRandomDecision(dateTimeOffset: randomDateTimeOffset, userId: randomUserId);

            randomDecision.UpdatedDate = randomDateTimeOffset.AddMinutes(minutes);

            var invalidDecisionException =
                new InvalidDecisionException(
                    message: "Invalid decision. Please correct the errors and try again.");

            invalidDecisionException.AddData(
                key: nameof(Decision.UpdatedDate),
                values:
                    $"Date is not recent. Expected a value between {startDate} and {endDate} but found {invalidDate}");

            var expectedDecisionValidatonException =
                new DecisionValidationException(
                    message: "Decision validation errors occurred, please try again.",
                    innerException: invalidDecisionException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyModifyAuditValueAsync(randomDecision))
                    .ReturnsAsync(randomDecision);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomUser);

            // when
            ValueTask<Decision> modifyDecisionTask =
                this.decisionService.ModifyDecisionAsync(randomDecision);

            DecisionValidationException actualDecisionValidationException =
                await Assert.ThrowsAsync<DecisionValidationException>(
                    modifyDecisionTask.AsTask);

            // then
            actualDecisionValidationException.Should()
                .BeEquivalentTo(expectedDecisionValidatonException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyModifyAuditValueAsync(randomDecision),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedDecisionValidatonException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectDecisionByIdAsync(It.IsAny<Guid>()),
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
        public async Task ShouldThrowValidationExceptionOnModifyIfDecisionDoesNotExistAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            string randomUserId = GetRandomString();
            User randomUser = CreateRandomUser(userId: randomUserId);

            Decision randomDecision = CreateRandomModifyDecision(
                dateTimeOffset: randomDateTimeOffset, userId: randomUserId);

            Decision nonExistDecision = randomDecision;
            Decision nullDecision = null;

            var notFoundDecisionException = new NotFoundDecisionException(
                message: $"Couldn't find decision type with decisionId: {nonExistDecision.Id}.");

            var expectedDecisionValidationException =
                new DecisionValidationException(
                    message: "Decision validation errors occurred, please try again.",
                    innerException: notFoundDecisionException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyModifyAuditValueAsync(nonExistDecision))
                    .ReturnsAsync(nonExistDecision);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectDecisionByIdAsync(nonExistDecision.Id))
                    .ReturnsAsync(nullDecision);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomUser);

            // when 
            ValueTask<Decision> modifyDecisionTask =
                this.decisionService.ModifyDecisionAsync(nonExistDecision);

            DecisionValidationException actualDecisionValidationException =
                await Assert.ThrowsAsync<DecisionValidationException>(
                    modifyDecisionTask.AsTask);

            // then
            actualDecisionValidationException.Should()
                .BeEquivalentTo(expectedDecisionValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyModifyAuditValueAsync(nonExistDecision),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectDecisionByIdAsync(nonExistDecision.Id),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
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

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfStorageCreatedDateNotSameAsCreatedDateAndLogItAsync()
        {
            // given
            int randomNumber = GetRandomNegativeNumber();
            int randomMinutes = randomNumber;
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            string randomUserId = GetRandomString();
            User randomUser = CreateRandomUser(userId: randomUserId);

            Decision randomDecision = CreateRandomModifyDecision(
                dateTimeOffset: randomDateTimeOffset, userId: randomUserId);

            Decision invalidDecision = randomDecision.DeepClone();
            Decision storageDecision = invalidDecision.DeepClone();
            storageDecision.CreatedDate = storageDecision.CreatedDate.AddMinutes(randomMinutes);
            storageDecision.UpdatedDate = storageDecision.UpdatedDate.AddMinutes(randomMinutes);

            var invalidDecisionException =
                new InvalidDecisionException(
                    message: "Invalid decision. Please correct the errors and try again.");

            invalidDecisionException.AddData(
                key: nameof(Decision.CreatedDate),
                values: $"Date is not the same as {nameof(Decision.CreatedDate)}");

            var expectedDecisionValidationException =
                new DecisionValidationException(
                    message: "Decision validation errors occurred, please try again.",
                    innerException: invalidDecisionException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyModifyAuditValueAsync(invalidDecision))
                    .ReturnsAsync(invalidDecision);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectDecisionByIdAsync(invalidDecision.Id))
                    .ReturnsAsync(storageDecision);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomUser);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.EnsureAddAuditValuesRemainsUnchangedOnModifyAsync(invalidDecision, storageDecision))
                    .ReturnsAsync(invalidDecision);

            // when
            ValueTask<Decision> modifyDecisionTask =
                this.decisionService.ModifyDecisionAsync(invalidDecision);

            DecisionValidationException actualDecisionValidationException =
                await Assert.ThrowsAsync<DecisionValidationException>(
                    modifyDecisionTask.AsTask);

            // then
            actualDecisionValidationException.Should()
                .BeEquivalentTo(expectedDecisionValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyModifyAuditValueAsync(invalidDecision),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectDecisionByIdAsync(invalidDecision.Id),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.EnsureAddAuditValuesRemainsUnchangedOnModifyAsync(invalidDecision, storageDecision),
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

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfCreatedUserDontMacthStorageAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            string randomUserId = GetRandomString();
            User randomUser = CreateRandomUser(userId: randomUserId);

            Decision randomDecision =
                CreateRandomModifyDecision(dateTimeOffset: randomDateTimeOffset, userId: randomUserId);

            Decision invalidDecision = randomDecision.DeepClone();
            Decision storageDecision = invalidDecision.DeepClone();
            invalidDecision.CreatedBy = Guid.NewGuid().ToString();
            storageDecision.UpdatedDate = storageDecision.CreatedDate;

            var invalidDecisionException =
                new InvalidDecisionException(
                    message: "Invalid decision. Please correct the errors and try again.");

            invalidDecisionException.AddData(
                key: nameof(Decision.CreatedBy),
                values: $"Text is not the same as {nameof(Decision.CreatedBy)}");

            var expectedDecisionValidationException =
                new DecisionValidationException(
                    message: "Decision validation errors occurred, please try again.",
                    innerException: invalidDecisionException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyModifyAuditValueAsync(invalidDecision))
                    .ReturnsAsync(invalidDecision);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectDecisionByIdAsync(invalidDecision.Id))
                    .ReturnsAsync(storageDecision);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomUser);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.EnsureAddAuditValuesRemainsUnchangedOnModifyAsync(invalidDecision, storageDecision))
                    .ReturnsAsync(invalidDecision);

            // when
            ValueTask<Decision> modifyDecisionTask =
                this.decisionService.ModifyDecisionAsync(invalidDecision);

            DecisionValidationException actualDecisionValidationException =
                await Assert.ThrowsAsync<DecisionValidationException>(
                    modifyDecisionTask.AsTask);

            // then
            actualDecisionValidationException.Should().BeEquivalentTo(expectedDecisionValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyModifyAuditValueAsync(invalidDecision),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectDecisionByIdAsync(invalidDecision.Id),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.EnsureAddAuditValuesRemainsUnchangedOnModifyAsync(invalidDecision, storageDecision),
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

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfStorageUpdatedDateSameAsUpdatedDateAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            string randomUserId = GetRandomString();
            User randomUser = CreateRandomUser(userId: randomUserId);

            Decision randomDecision =
                CreateRandomModifyDecision(dateTimeOffset: randomDateTimeOffset, userId: randomUserId);

            Decision invalidDecision = randomDecision;
            Decision storageDecision = randomDecision.DeepClone();

            var invalidDecisionException =
                new InvalidDecisionException(
                    message: "Invalid decision. Please correct the errors and try again.");

            invalidDecisionException.AddData(
                key: nameof(Decision.UpdatedDate),
                values: $"Date is the same as {nameof(Decision.UpdatedDate)}");

            var expectedDecisionValidationException =
                new DecisionValidationException(
                    message: "Decision validation errors occurred, please try again.",
                    innerException: invalidDecisionException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyModifyAuditValueAsync(invalidDecision))
                    .ReturnsAsync(invalidDecision);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectDecisionByIdAsync(invalidDecision.Id))
                    .ReturnsAsync(storageDecision);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomUser);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.EnsureAddAuditValuesRemainsUnchangedOnModifyAsync(invalidDecision, storageDecision))
                    .ReturnsAsync(invalidDecision);

            // when
            ValueTask<Decision> modifyDecisionTask =
                this.decisionService.ModifyDecisionAsync(invalidDecision);

            // then
            await Assert.ThrowsAsync<DecisionValidationException>(
                modifyDecisionTask.AsTask);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyModifyAuditValueAsync(invalidDecision),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedDecisionValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectDecisionByIdAsync(invalidDecision.Id),
                    Times.Once);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.EnsureAddAuditValuesRemainsUnchangedOnModifyAsync(invalidDecision, storageDecision),
                    Times.Once);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}