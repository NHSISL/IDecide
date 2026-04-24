// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Core.Models.Foundations.Users.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;
using User = LondonDataServices.IDecide.Core.Models.Foundations.Users.User;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.Users
{
    public partial class UserServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRemoveByIdIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Guid someUserId = Guid.NewGuid();
            SqlException sqlException = GetSqlException();

            var failedUserStorageException =
                new FailedUserStorageException(
                    message: "Failed user storage error occurred, contact support.",
                    innerException: sqlException);

            var expectedUserDependencyException =
                new UserDependencyException(
                    message: "User dependency error occurred, contact support.",
                    innerException: failedUserStorageException);

            this.storageBrokerMock.Setup(broker =>
                    broker.SelectUserByIdAsync(someUserId))
                .ThrowsAsync(sqlException);

            // when
            ValueTask<User> removeUserByIdTask = this.userService.RemoveUserByIdAsync(someUserId);

            UserDependencyException actualUserDependencyException =
                await Assert.ThrowsAsync<UserDependencyException>(removeUserByIdTask.AsTask);

            // then
            actualUserDependencyException.Should()
                .BeEquivalentTo(expectedUserDependencyException);

            this.storageBrokerMock.Verify(broker =>
                    broker.SelectUserByIdAsync(someUserId),
                Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                    broker.LogCriticalAsync(It.Is(SameExceptionAs(expectedUserDependencyException))),
                Times.Once);

            this.storageBrokerMock.Verify(broker =>
                    broker.DeleteUserAsync(It.IsAny<User>()),
                Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityAuditBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnRemoveByIdIfDbUpdateConcurrencyOccursAndLogItAsync()
        {
            // given
            Guid someUserId = Guid.NewGuid();
            var dbUpdateConcurrencyException = new DbUpdateConcurrencyException();

            var lockedUserException =
                new LockedUserException(
                    message: "Locked user record exception, please try again later",
                    innerException: dbUpdateConcurrencyException);

            var expectedUserDependencyValidationException =
                new UserDependencyValidationException(
                    message: "User dependency validation occurred, please try again.",
                    innerException: lockedUserException);

            this.storageBrokerMock.Setup(broker =>
                    broker.SelectUserByIdAsync(someUserId))
                .ThrowsAsync(dbUpdateConcurrencyException);

            // when
            ValueTask<User> removeUserByIdTask = this.userService.RemoveUserByIdAsync(someUserId);

            UserDependencyValidationException actualUserDependencyValidationException =
                await Assert.ThrowsAsync<UserDependencyValidationException>(removeUserByIdTask.AsTask);

            // then
            actualUserDependencyValidationException.Should()
                .BeEquivalentTo(expectedUserDependencyValidationException);

            this.storageBrokerMock.Verify(broker =>
                    broker.SelectUserByIdAsync(someUserId),
                Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                    broker.LogErrorAsync(It.Is(SameExceptionAs(
                        expectedUserDependencyValidationException))),
                Times.Once);

            this.storageBrokerMock.Verify(broker =>
                    broker.DeleteUserAsync(It.IsAny<User>()),
                Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityAuditBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRemoveByIdIfDbUpdateExceptionOccursAndLogItAsync()
        {
            // given
            Guid someUserId = Guid.NewGuid();
            var dbUpdateException = new DbUpdateException();

            var failedUserStorageException =
                new FailedUserStorageException(
                    message: "Failed user storage error occurred, contact support.",
                    innerException: dbUpdateException);

            var expectedUserDependencyException =
                new UserDependencyException(
                    message: "User dependency error occurred, contact support.",
                    innerException: failedUserStorageException);

            this.storageBrokerMock.Setup(broker =>
                    broker.SelectUserByIdAsync(someUserId))
                .ThrowsAsync(dbUpdateException);

            // when
            ValueTask<User> removeUserByIdTask = this.userService.RemoveUserByIdAsync(someUserId);

            UserDependencyException actualUserDependencyException =
                await Assert.ThrowsAsync<UserDependencyException>(removeUserByIdTask.AsTask);

            // then
            actualUserDependencyException.Should()
                .BeEquivalentTo(expectedUserDependencyException);

            this.storageBrokerMock.Verify(broker =>
                    broker.SelectUserByIdAsync(someUserId),
                Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                    broker.LogErrorAsync(It.Is(SameExceptionAs(expectedUserDependencyException))),
                Times.Once);

            this.storageBrokerMock.Verify(broker =>
                    broker.DeleteUserAsync(It.IsAny<User>()),
                Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityAuditBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRemoveByIdIfServiceErrorOccursAndLogItAsync()
        {
            // given
            Guid someUserId = Guid.NewGuid();
            var serviceException = new Exception();

            var failedUserServiceException =
                new FailedUserServiceException(
                    message: "Failed user service occurred, please contact support",
                    innerException: serviceException);

            var expectedUserServiceException =
                new UserServiceException(
                    message: "User service error occurred, contact support.",
                    innerException: failedUserServiceException);

            this.storageBrokerMock.Setup(broker =>
                    broker.SelectUserByIdAsync(someUserId))
                .ThrowsAsync(serviceException);

            // when
            ValueTask<User> removeUserByIdTask = this.userService.RemoveUserByIdAsync(someUserId);

            UserServiceException actualUserServiceException =
                await Assert.ThrowsAsync<UserServiceException>(removeUserByIdTask.AsTask);

            // then
            actualUserServiceException.Should()
                .BeEquivalentTo(expectedUserServiceException);

            this.storageBrokerMock.Verify(broker =>
                    broker.SelectUserByIdAsync(someUserId),
                Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                    broker.LogErrorAsync(It.Is(SameExceptionAs(expectedUserServiceException))),
                Times.Once);

            this.storageBrokerMock.Verify(broker =>
                    broker.DeleteUserAsync(It.IsAny<User>()),
                Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityAuditBrokerMock.VerifyNoOtherCalls();
        }
    }
}
