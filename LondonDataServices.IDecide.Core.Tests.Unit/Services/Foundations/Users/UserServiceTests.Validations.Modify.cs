// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Core.Models.Foundations.Users.Exceptions;
using Moq;
using User = LondonDataServices.IDecide.Core.Models.Foundations.Users.User;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.Users
{
    public partial class UserServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfUserIsNullAndLogItAsync()
        {
            // given
            User nullUser = null;

            var nullUserException =
                new NullUserException(message: "User is null.");

            var expectedUserValidationException =
                new UserValidationException(
                    message: "User validation errors occurred, please try again.",
                    innerException: nullUserException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyModifyAuditValuesAsync(nullUser))
                    .ReturnsAsync(nullUser);

            // when
            ValueTask<User> modifyUserTask = this.userService.ModifyUserAsync(nullUser);

            UserValidationException actualUserValidationException =
                await Assert.ThrowsAsync<UserValidationException>(() => modifyUserTask.AsTask());

            // then
            actualUserValidationException.Should()
                .BeEquivalentTo(expectedUserValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyModifyAuditValuesAsync(nullUser),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedUserValidationException))),
                    Times.Once);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnModifyIfUserIsInvalidAndLogItAsync(string invalidText)
        {
            // given
            string randomUserId = GetRandomString();

            var invalidUser = new User
            {
                NhsIdUserUid = invalidText,
                Name = invalidText,
                Sub = invalidText
            };

            var invalidUserException =
                new InvalidUserException(
                    message: "Invalid user. Please correct the errors and try again.");

            invalidUserException.AddData(
                key: nameof(User.Id),
                values: "Id is required");

            invalidUserException.AddData(
                key: nameof(User.NhsIdUserUid),
                values: "Text is required");

            invalidUserException.AddData(
                key: nameof(User.Name),
                values: "Text is required");

            invalidUserException.AddData(
                key: nameof(User.Sub),
                values: "Text is required");

            invalidUserException.AddData(
                key: nameof(User.CreatedDate),
                values: "Date is required");

            invalidUserException.AddData(
                key: nameof(User.CreatedBy),
                values: "Text is required");

            invalidUserException.AddData(
                key: nameof(User.UpdatedDate),
                values:
                [
                    "Date is required",
                    $"Date is the same as {nameof(User.CreatedDate)}"
                ]);

            invalidUserException.AddData(
                key: nameof(User.UpdatedBy),
                values:
                [
                    "Text is required",
                    $"Expected value to be '{randomUserId}' but found ''."
                ]);

            var expectedUserValidationException =
                new UserValidationException(
                    message: "User validation errors occurred, please try again.",
                    innerException: invalidUserException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyModifyAuditValuesAsync(It.IsAny<User>()))
                    .ReturnsAsync(invalidUser);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.GetCurrentUserIdAsync())
                    .ReturnsAsync(randomUserId);

            // when
            ValueTask<User> modifyUserTask = this.userService.ModifyUserAsync(invalidUser);

            UserValidationException actualUserValidationException =
                await Assert.ThrowsAsync<UserValidationException>(() => modifyUserTask.AsTask());

            // then
            actualUserValidationException.Should()
                .BeEquivalentTo(expectedUserValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyModifyAuditValuesAsync(It.IsAny<User>()),
                    Times.Once);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.GetCurrentUserIdAsync(),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedUserValidationException))),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateUserAsync(It.IsAny<User>()),
                    Times.Never);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfUserNotFoundAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            string randomUserId = GetRandomString();
            User randomUser = CreateRandomModifyUser(randomDateTimeOffset, randomUserId);
            User inputUser = randomUser;
            User auditAppliedUser = inputUser;
            User noUser = null;

            var notFoundUserException =
                new NotFoundUserException(
                    message: $"Couldn't find user with userId: {inputUser.Id}.");

            var expectedUserValidationException =
                new UserValidationException(
                    message: "User validation errors occurred, please try again.",
                    innerException: notFoundUserException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyModifyAuditValuesAsync(inputUser))
                    .ReturnsAsync(auditAppliedUser);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.GetCurrentUserIdAsync())
                    .ReturnsAsync(randomUserId);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectUserByIdAsync(inputUser.Id))
                    .ReturnsAsync(noUser);

            // when
            ValueTask<User> modifyUserTask = this.userService.ModifyUserAsync(inputUser);

            UserValidationException actualUserValidationException =
                await Assert.ThrowsAsync<UserValidationException>(() => modifyUserTask.AsTask());

            // then
            actualUserValidationException.Should()
                .BeEquivalentTo(expectedUserValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyModifyAuditValuesAsync(inputUser),
                    Times.Once);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.GetCurrentUserIdAsync(),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectUserByIdAsync(inputUser.Id),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedUserValidationException))),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateUserAsync(It.IsAny<User>()),
                    Times.Never);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
