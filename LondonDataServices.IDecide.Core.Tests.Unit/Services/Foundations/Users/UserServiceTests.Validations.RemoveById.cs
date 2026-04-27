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
        public async Task ShouldThrowValidationExceptionOnRemoveByIdIfIdIsInvalidAndLogItAsync()
        {
            // given
            Guid invalidUserId = default;

            var invalidUserException =
                new InvalidUserException(
                    message: "Invalid user. Please correct the errors and try again.");

            invalidUserException.AddData(
                key: nameof(User.Id),
                values: "Id is required");

            var expectedUserValidationException =
                new UserValidationException(
                    message: "User validation errors occurred, please try again.",
                    innerException: invalidUserException);

            // when
            ValueTask<User> removeUserByIdTask = this.userService.RemoveUserByIdAsync(invalidUserId);

            UserValidationException actualUserValidationException =
                await Assert.ThrowsAsync<UserValidationException>(() => removeUserByIdTask.AsTask());

            // then
            actualUserValidationException.Should()
                .BeEquivalentTo(expectedUserValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedUserValidationException))),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectUserByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityAuditBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnRemoveByIdIfUserNotFoundAndLogItAsync()
        {
            // given
            Guid someUserId = Guid.NewGuid();
            User noUser = null;

            var notFoundUserException =
                new NotFoundUserException(
                    message: $"Couldn't find user with userId: {someUserId}.");

            var expectedUserValidationException =
                new UserValidationException(
                    message: "User validation errors occurred, please try again.",
                    innerException: notFoundUserException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectUserByIdAsync(someUserId))
                    .ReturnsAsync(noUser);

            // when
            ValueTask<User> removeUserByIdTask = this.userService.RemoveUserByIdAsync(someUserId);

            UserValidationException actualUserValidationException =
                await Assert.ThrowsAsync<UserValidationException>(() => removeUserByIdTask.AsTask());

            // then
            actualUserValidationException.Should()
                .BeEquivalentTo(expectedUserValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectUserByIdAsync(someUserId),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedUserValidationException))),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityAuditBrokerMock.VerifyNoOtherCalls();
        }
    }
}
