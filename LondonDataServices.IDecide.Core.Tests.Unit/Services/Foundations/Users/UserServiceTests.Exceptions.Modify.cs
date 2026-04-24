// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
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
        public async Task ShouldThrowCriticalDependencyExceptionOnModifyIfSqlErrorOccursAndLogItAsync()
        {
            // given
            User someUser = CreateRandomUser();
            SqlException sqlException = GetSqlException();

            var failedUserStorageException =
                new FailedUserStorageException(
                    message: "Failed user storage error occurred, contact support.",
                    innerException: sqlException);

            var expectedUserDependencyException =
                new UserDependencyException(
                    message: "User dependency error occurred, contact support.",
                    innerException: failedUserStorageException);

            this.securityAuditBrokerMock.Setup(broker =>
                    broker.ApplyModifyAuditValuesAsync(It.IsAny<User>()))
                .ThrowsAsync(sqlException);

            // when
            ValueTask<User> modifyUserTask = this.userService.ModifyUserAsync(someUser);

            UserDependencyException actualUserDependencyException =
                await Assert.ThrowsAsync<UserDependencyException>(modifyUserTask.AsTask);

            // then
            actualUserDependencyException.Should()
                .BeEquivalentTo(expectedUserDependencyException);

            this.securityAuditBrokerMock.Verify(broker =>
                    broker.ApplyModifyAuditValuesAsync(It.IsAny<User>()),
                Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                    broker.LogCriticalAsync(It.Is(SameExceptionAs(expectedUserDependencyException))),
                Times.Once);

            this.storageBrokerMock.Verify(broker =>
                    broker.UpdateUserAsync(It.IsAny<User>()),
                Times.Never);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnModifyIfDbUpdateConcurrencyOccursAndLogItAsync()
        {
            // given
            User someUser = CreateRandomUser();
            var dbUpdateConcurrencyException = new DbUpdateConcurrencyException();

            var lockedUserException =
                new LockedUserException(
                    message: "Locked user record exception, please try again later",
                    innerException: dbUpdateConcurrencyException);

            var expectedUserDependencyValidationException =
                new UserDependencyValidationException(
                    message: "User dependency validation occurred, please try again.",
                    innerException: lockedUserException);

            this.securityAuditBrokerMock.Setup(broker =>
                    broker.ApplyModifyAuditValuesAsync(It.IsAny<User>()))
                .ThrowsAsync(dbUpdateConcurrencyException);

            // when
            ValueTask<User> modifyUserTask = this.userService.ModifyUserAsync(someUser);

            UserDependencyValidationException actualUserDependencyValidationException =
                await Assert.ThrowsAsync<UserDependencyValidationException>(modifyUserTask.AsTask);

            // then
            actualUserDependencyValidationException.Should()
                .BeEquivalentTo(expectedUserDependencyValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                    broker.ApplyModifyAuditValuesAsync(It.IsAny<User>()),
                Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                    broker.LogErrorAsync(It.Is(SameExceptionAs(
                        expectedUserDependencyValidationException))),
                Times.Once);

            this.storageBrokerMock.Verify(broker =>
                    broker.UpdateUserAsync(It.IsAny<User>()),
                Times.Never);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnModifyIfDbUpdateExceptionOccursAndLogItAsync()
        {
            // given
            User someUser = CreateRandomUser();
            var dbUpdateException = new DbUpdateException();

            var failedUserStorageException =
                new FailedUserStorageException(
                    message: "Failed user storage error occurred, contact support.",
                    innerException: dbUpdateException);

            var expectedUserDependencyException =
                new UserDependencyException(
                    message: "User dependency error occurred, contact support.",
                    innerException: failedUserStorageException);

            this.securityAuditBrokerMock.Setup(broker =>
                    broker.ApplyModifyAuditValuesAsync(It.IsAny<User>()))
                .ThrowsAsync(dbUpdateException);

            // when
            ValueTask<User> modifyUserTask = this.userService.ModifyUserAsync(someUser);

            UserDependencyException actualUserDependencyException =
                await Assert.ThrowsAsync<UserDependencyException>(modifyUserTask.AsTask);

            // then
            actualUserDependencyException.Should()
                .BeEquivalentTo(expectedUserDependencyException);

            this.securityAuditBrokerMock.Verify(broker =>
                    broker.ApplyModifyAuditValuesAsync(It.IsAny<User>()),
                Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                    broker.LogErrorAsync(It.Is(SameExceptionAs(expectedUserDependencyException))),
                Times.Once);

            this.storageBrokerMock.Verify(broker =>
                    broker.UpdateUserAsync(It.IsAny<User>()),
                Times.Never);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnModifyIfServiceErrorOccursAndLogItAsync()
        {
            // given
            User someUser = CreateRandomUser();
            var serviceException = new Exception();

            var failedUserServiceException =
                new FailedUserServiceException(
                    message: "Failed user service occurred, please contact support",
                    innerException: serviceException);

            var expectedUserServiceException =
                new UserServiceException(
                    message: "User service error occurred, contact support.",
                    innerException: failedUserServiceException);

            this.securityAuditBrokerMock.Setup(broker =>
                    broker.ApplyModifyAuditValuesAsync(It.IsAny<User>()))
                .ThrowsAsync(serviceException);

            // when
            ValueTask<User> modifyUserTask = this.userService.ModifyUserAsync(someUser);

            UserServiceException actualUserServiceException =
                await Assert.ThrowsAsync<UserServiceException>(modifyUserTask.AsTask);

            // then
            actualUserServiceException.Should()
                .BeEquivalentTo(expectedUserServiceException);

            this.securityAuditBrokerMock.Verify(broker =>
                    broker.ApplyModifyAuditValuesAsync(It.IsAny<User>()),
                Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                    broker.LogErrorAsync(It.Is(SameExceptionAs(expectedUserServiceException))),
                Times.Once);

            this.storageBrokerMock.Verify(broker =>
                    broker.UpdateUserAsync(It.IsAny<User>()),
                Times.Never);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
