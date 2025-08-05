// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Core.Models.Foundations.DecisionTypes;
using Microsoft.Data.SqlClient;
using Moq;
using StandardlyTestProject.Api.Models.Foundations.DecisionTypes.Exceptions;

namespace StandardlyTestProject.Api.Tests.Unit.Services.Foundations.DecisionTypes
{
    public partial class DecisionTypeServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveByIdIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Guid someId = Guid.NewGuid();
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
                broker.SelectDecisionTypeByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<DecisionType> retrieveDecisionTypeByIdTask =
                this.decisionTypeService.RetrieveDecisionTypeByIdAsync(someId);

            DecisionTypeDependencyException actualDecisionTypeDependencyException =
                await Assert.ThrowsAsync<DecisionTypeDependencyException>(
                    retrieveDecisionTypeByIdTask.AsTask);

            // then
            actualDecisionTypeDependencyException.Should()
                .BeEquivalentTo(expectedDecisionTypeDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectDecisionTypeByIdAsync(It.IsAny<Guid>()),
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
        public async Task ShouldThrowServiceExceptionOnRetrieveByIdIfServiceErrorOccursAndLogItAsync()
        {
            // given
            Guid someId = Guid.NewGuid();
            var serviceException = new Exception();

            var failedDecisionTypeServiceException =
                new FailedDecisionTypeServiceException(
                    message: "Failed decisionType service occurred, please contact support",
                    innerException: serviceException);

            var expectedDecisionTypeServiceException =
                new DecisionTypeServiceException(
                    message: "DecisionType service error occurred, contact support.",
                    innerException: failedDecisionTypeServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectDecisionTypeByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<DecisionType> retrieveDecisionTypeByIdTask =
                this.decisionTypeService.RetrieveDecisionTypeByIdAsync(someId);

            DecisionTypeServiceException actualDecisionTypeServiceException =
                await Assert.ThrowsAsync<DecisionTypeServiceException>(
                    retrieveDecisionTypeByIdTask.AsTask);

            // then
            actualDecisionTypeServiceException.Should()
                .BeEquivalentTo(expectedDecisionTypeServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectDecisionTypeByIdAsync(It.IsAny<Guid>()),
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