// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
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
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveByIdIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Guid someId = Guid.NewGuid();
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
                broker.SelectDecisionByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<Decision> retrieveDecisionByIdTask =
                this.decisionService.RetrieveDecisionByIdAsync(someId);

            DecisionDependencyException actualDecisionDependencyException =
                await Assert.ThrowsAsync<DecisionDependencyException>(
                    retrieveDecisionByIdTask.AsTask);

            // then
            actualDecisionDependencyException.Should()
                .BeEquivalentTo(expectedDecisionDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectDecisionByIdAsync(It.IsAny<Guid>()),
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
        public async Task ShouldThrowServiceExceptionOnRetrieveByIdIfServiceErrorOccursAndLogItAsync()
        {
            // given
            Guid someId = Guid.NewGuid();
            var serviceException = new Exception();

            var failedDecisionServiceException =
                new FailedDecisionServiceException(
                    message: "Failed decision service occurred, please contact support",
                    innerException: serviceException);

            var expectedDecisionServiceException =
                new DecisionServiceException(
                    message: "Decision service error occurred, contact support.",
                    innerException: failedDecisionServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectDecisionByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<Decision> retrieveDecisionByIdTask =
                this.decisionService.RetrieveDecisionByIdAsync(someId);

            DecisionServiceException actualDecisionServiceException =
                await Assert.ThrowsAsync<DecisionServiceException>(
                    retrieveDecisionByIdTask.AsTask);

            // then
            actualDecisionServiceException.Should()
                .BeEquivalentTo(expectedDecisionServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectDecisionByIdAsync(It.IsAny<Guid>()),
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