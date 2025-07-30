using System;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Moq;
using StandardlyTestProject.Api.Models.Foundations.DecisionTypes.Exceptions;
using Xunit;

namespace StandardlyTestProject.Api.Tests.Unit.Services.Foundations.DecisionTypes
{
    public partial class DecisionTypeServiceTests
    {
        [Fact]
        public void ShouldThrowCriticalDependencyExceptionOnRetrieveAllWhenSqlExceptionOccursAndLogIt()
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
                broker.SelectAllDecisionTypes())
                    .Throws(sqlException);

            // when
            Action retrieveAllDecisionTypesAction = () =>
                this.decisionTypeService.RetrieveAllDecisionTypes();

            DecisionTypeDependencyException actualDecisionTypeDependencyException =
                Assert.Throws<DecisionTypeDependencyException>(retrieveAllDecisionTypesAction);

            // then
            actualDecisionTypeDependencyException.Should()
                .BeEquivalentTo(expectedDecisionTypeDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllDecisionTypes(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedDecisionTypeDependencyException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldThrowServiceExceptionOnRetrieveAllIfServiceErrorOccursAndLogItAsync()
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
                broker.SelectAllDecisionTypes())
                    .Throws(serviceException);

            // when
            Action retrieveAllDecisionTypesAction = () =>
                this.decisionTypeService.RetrieveAllDecisionTypes();

            DecisionTypeServiceException actualDecisionTypeServiceException =
                Assert.Throws<DecisionTypeServiceException>(retrieveAllDecisionTypesAction);

            // then
            actualDecisionTypeServiceException.Should()
                .BeEquivalentTo(expectedDecisionTypeServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllDecisionTypes(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedDecisionTypeServiceException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}