// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Core.Models.Foundations.DecisionType;
using LondonDataServices.IDecide.Core.Models.Foundations.DecisionType.Exceptions;
using Microsoft.Data.SqlClient;
using Moq;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.DecisionType
{
    public partial class DecisionTypeServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveAllWhenSqlExceptionOccursAndLogIt()
        {
            // given
            SqlException sqlException = GetSqlException();

            var failedDecisionTypeStorageException =
                new FailedDecisionTypeStorageException(
                    message: "Failed decisionType storage error occurred, contact support.",
                    innerException: sqlException);

            var expectedDecisionTypeDependencyException =
                new DecisionTypeDependencyException(
                    message: "DecisionType dependency error occurred, contact support.",
                    innerException: failedDecisionTypeStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllDecisionTypeAsync())
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<IQueryable<DecisionType>> retrieveAllDecisionTypeTask =
                this.decisionTypeService.RetrieveAllDecisionTypeAsync();

            DecisionTypeDependencyException actualDecisionTypeDependencyException =
                await Assert.ThrowsAsync<DecisionTypeDependencyException>(
                    retrieveAllDecisionTypeTask.AsTask);

            // then
            actualDecisionTypeDependencyException.Should()
                .BeEquivalentTo(expectedDecisionTypeDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllDecisionTypeAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedDecisionTypeDependencyException))),
                        Times.Once);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveAllIfServiceErrorOccursAndLogItAsync()
        {
            // given
            string exceptionMessage = GetRandomString();
            var serviceException = new Exception(exceptionMessage);

            var failedDecisionTypeServiceException =
                new FailedDecisionTypeServiceException(
                    message: "Failed decisionType service occurred, please contact support",
                    innerException: serviceException);

            var expectedDecisionTypeServiceException =
                new DecisionTypeServiceException(
                    message: "DecisionType service error occurred, contact support.",
                    innerException: failedDecisionTypeServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllDecisionTypeAsync())
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<IQueryable<DecisionType>> retrieveAllDecisionTypeTask =
                this.decisionTypeService.RetrieveAllDecisionTypeAsync();

            DecisionTypeServiceException actualDecisionTypeServiceException =
                await Assert.ThrowsAsync<DecisionTypeServiceException>(retrieveAllDecisionTypeTask.AsTask);

            // then
            actualDecisionTypeServiceException.Should()
                .BeEquivalentTo(expectedDecisionTypeServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllDecisionTypeAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedDecisionTypeServiceException))),
                        Times.Once);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
        }
    }
}