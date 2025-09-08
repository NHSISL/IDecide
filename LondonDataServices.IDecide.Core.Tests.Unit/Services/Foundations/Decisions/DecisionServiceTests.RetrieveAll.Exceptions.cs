// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Core.Models.Foundations.Decisions;
using LondonDataServices.IDecide.Core.Models.Foundations.Decisions.Exceptions;
using Microsoft.Data.SqlClient;
using Moq;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.Decisions
{
    public partial class DecisionServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveAllWhenSqlExceptionOccursAndLogIt()
        {
            // given
            SqlException sqlException = GetSqlException();

            var failedDecisionStorageException =
                new FailedDecisionStorageException(
                    message: "Failed decision storage error occurred, contact support.",
                    innerException: sqlException);

            var expectedDecisionDependencyException =
                new DecisionDependencyException(
                    message: "Decision dependency error occurred, contact support.",
                    innerException: failedDecisionStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllDecisionsAsync())
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<IQueryable<Decision>> retrieveAllDecisionsTask =
                this.decisionService.RetrieveAllDecisionsAsync();

            DecisionDependencyException actualDecisionDependencyException =
                await Assert.ThrowsAsync<DecisionDependencyException>(
                    retrieveAllDecisionsTask.AsTask);

            // then
            actualDecisionDependencyException.Should()
                .BeEquivalentTo(expectedDecisionDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllDecisionsAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedDecisionDependencyException))),
                        Times.Once);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveAllIfServiceErrorOccursAndLogItAsync()
        {
            // given
            string exceptionMessage = GetRandomString();
            var serviceException = new Exception(exceptionMessage);

            var failedDecisionServiceException =
                new FailedDecisionServiceException(
                    message: "Failed decision service occurred, please contact support",
                    innerException: serviceException);

            var expectedDecisionServiceException =
                new DecisionServiceException(
                    message: "Decision service error occurred, contact support.",
                    innerException: failedDecisionServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllDecisionsAsync())
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<IQueryable<Decision>> retrieveAllDecisionsTask =
                this.decisionService.RetrieveAllDecisionsAsync();

            DecisionServiceException actualDecisionServiceException =
                await Assert.ThrowsAsync<DecisionServiceException>(retrieveAllDecisionsTask.AsTask);

            // then
            actualDecisionServiceException.Should()
                .BeEquivalentTo(expectedDecisionServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllDecisionsAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedDecisionServiceException))),
                        Times.Once);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}