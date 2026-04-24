// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Core.Models.Foundations.Users.Exceptions;
using Microsoft.Data.SqlClient;
using Moq;
using User = LondonDataServices.IDecide.Core.Models.Foundations.Users.User;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.Users
{
    public partial class UserServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveAllIfSqlErrorOccursAndLogItAsync()
        {
            // given
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
                broker.SelectAllUsersAsync())
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<IQueryable<User>> retrieveAllUsersTask = this.userService.RetrieveAllUsersAsync();

            UserDependencyException actualUserDependencyException =
                await Assert.ThrowsAsync<UserDependencyException>(retrieveAllUsersTask.AsTask);

            // then
            actualUserDependencyException.Should()
                .BeEquivalentTo(expectedUserDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllUsersAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(expectedUserDependencyException))),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityAuditBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveAllIfServiceErrorOccursAndLogItAsync()
        {
            // given
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
                broker.SelectAllUsersAsync())
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<IQueryable<User>> retrieveAllUsersTask = this.userService.RetrieveAllUsersAsync();

            UserServiceException actualUserServiceException =
                await Assert.ThrowsAsync<UserServiceException>(retrieveAllUsersTask.AsTask);

            // then
            actualUserServiceException.Should()
                .BeEquivalentTo(expectedUserServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllUsersAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedUserServiceException))),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityAuditBrokerMock.VerifyNoOtherCalls();
        }
    }
}
