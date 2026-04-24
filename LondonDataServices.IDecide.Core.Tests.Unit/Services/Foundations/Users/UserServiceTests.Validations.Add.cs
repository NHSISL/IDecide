// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using LondonDataServices.IDecide.Core.Models.Foundations.Users.Exceptions;
using Moq;
using User = LondonDataServices.IDecide.Core.Models.Foundations.Users.User;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.Users
{
    public partial class UserServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfUserIsNullAndLogItAsync()
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
                    broker.ApplyAddAuditValuesAsync(nullUser))
                .ReturnsAsync(nullUser);

            // when
            ValueTask<User> addUserTask = this.userService.AddUserAsync(nullUser);

            UserValidationException actualUserValidationException =
                await Assert.ThrowsAsync<UserValidationException>(() => addUserTask.AsTask());

            // then
            actualUserValidationException.Should()
                .BeEquivalentTo(expectedUserValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                    broker.ApplyAddAuditValuesAsync(nullUser),
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
        public async Task ShouldThrowValidationExceptionOnAddIfUserIsInvalidAndLogItAsync(string invalidText)
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
                values:
                [
                    "Text is required",
                    $"Expected value to be '{randomUserId}' but found ''."
                ]);

            invalidUserException.AddData(
                key: nameof(User.UpdatedDate),
                values: "Date is required");

            invalidUserException.AddData(
                key: nameof(User.UpdatedBy),
                values:
                [
                    "Text is required"
                ]);

            var expectedUserValidationException =
                new UserValidationException(
                    message: "User validation errors occurred, please try again.",
                    innerException: invalidUserException);

            this.securityAuditBrokerMock.Setup(broker =>
                    broker.ApplyAddAuditValuesAsync(It.IsAny<User>()))
                .ReturnsAsync(invalidUser);

            this.securityAuditBrokerMock.Setup(broker =>
                    broker.GetCurrentUserIdAsync())
                .ReturnsAsync(randomUserId);

            // when
            ValueTask<User> addUserTask = this.userService.AddUserAsync(invalidUser);

            UserValidationException actualUserValidationException =
                await Assert.ThrowsAsync<UserValidationException>(() => addUserTask.AsTask());

            // then
            actualUserValidationException.Should()
                .BeEquivalentTo(expectedUserValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                    broker.ApplyAddAuditValuesAsync(It.IsAny<User>()),
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
                    broker.InsertUserAsync(It.IsAny<User>()),
                Times.Never);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(MinutesBeforeOrAfter))]
        public async Task ShouldThrowValidationExceptionOnAddIfCreatedDateIsNotRecentAndLogItAsync(int minutes)
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            string randomUserId = GetRandomString();

            User randomUser = CreateRandomUser(randomDateTimeOffset, randomUserId);
            User invalidUser = randomUser;
            invalidUser.CreatedDate = randomDateTimeOffset.AddMinutes(minutes);
            invalidUser.UpdatedDate = invalidUser.CreatedDate;

            var invalidUserException =
                new InvalidUserException(
                    message: "Invalid user. Please correct the errors and try again.");

            invalidUserException.AddData(
                key: nameof(User.CreatedDate),
                values: $"Date is not recent. Expected a value between " +
                    $"{randomDateTimeOffset.AddSeconds(-90)} and {randomDateTimeOffset} " +
                    $"but found {invalidUser.CreatedDate}");

            var expectedUserValidationException =
                new UserValidationException(
                    message: "User validation errors occurred, please try again.",
                    innerException: invalidUserException);

            this.securityAuditBrokerMock.Setup(broker =>
                    broker.ApplyAddAuditValuesAsync(It.IsAny<User>()))
                .ReturnsAsync(invalidUser);

            this.securityAuditBrokerMock.Setup(broker =>
                    broker.GetCurrentUserIdAsync())
                .ReturnsAsync(randomUserId);

            this.dateTimeBrokerMock.Setup(broker =>
                    broker.GetCurrentDateTimeOffsetAsync())
                .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<User> addUserTask = this.userService.AddUserAsync(invalidUser);

            UserValidationException actualUserValidationException =
                await Assert.ThrowsAsync<UserValidationException>(() => addUserTask.AsTask());

            // then
            actualUserValidationException.Should()
                .BeEquivalentTo(expectedUserValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                    broker.ApplyAddAuditValuesAsync(It.IsAny<User>()),
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
                    broker.InsertUserAsync(It.IsAny<User>()),
                Times.Never);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
