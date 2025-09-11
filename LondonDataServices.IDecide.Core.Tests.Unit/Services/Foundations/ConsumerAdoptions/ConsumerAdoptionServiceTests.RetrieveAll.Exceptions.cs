// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Core.Models.Foundations.ConsumerAdoptions;
using LondonDataServices.IDecide.Core.Models.Foundations.ConsumerAdoptions.Exceptions;
using Microsoft.Data.SqlClient;
using Moq;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.ConsumerAdoptions
{
    public partial class ConsumerAdoptionServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveAllWhenSqlExceptionOccursAndLogIt()
        {
            // given
            SqlException sqlException = GetSqlException();

            var failedConsumerAdoptionStorageException =
                new FailedConsumerAdoptionStorageException(
                    message: "Failed consumerAdoption storage error occurred, contact support.",
                    innerException: sqlException);

            var expectedConsumerAdoptionDependencyException =
                new ConsumerAdoptionDependencyException(
                    message: "ConsumerAdoption dependency error occurred, contact support.",
                    innerException: failedConsumerAdoptionStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllConsumerAdoptionsAsync())
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<IQueryable<ConsumerAdoption>> retrieveAllConsumerAdoptionsTask =
                this.consumerAdoptionService.RetrieveAllConsumerAdoptionsAsync();

            ConsumerAdoptionDependencyException actualConsumerAdoptionDependencyException =
                await Assert.ThrowsAsync<ConsumerAdoptionDependencyException>(
                    retrieveAllConsumerAdoptionsTask.AsTask);

            // then
            actualConsumerAdoptionDependencyException.Should()
                .BeEquivalentTo(expectedConsumerAdoptionDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllConsumerAdoptionsAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedConsumerAdoptionDependencyException))),
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

            var failedConsumerAdoptionServiceException =
                new FailedConsumerAdoptionServiceException(
                    message: "Failed consumerAdoption service occurred, please contact support",
                    innerException: serviceException);

            var expectedConsumerAdoptionServiceException =
                new ConsumerAdoptionServiceException(
                    message: "ConsumerAdoption service error occurred, contact support.",
                    innerException: failedConsumerAdoptionServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllConsumerAdoptionsAsync())
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<IQueryable<ConsumerAdoption>> retrieveAllConsumerAdoptionsTask =
                this.consumerAdoptionService.RetrieveAllConsumerAdoptionsAsync();

            ConsumerAdoptionServiceException actualConsumerAdoptionServiceException =
                await Assert.ThrowsAsync<ConsumerAdoptionServiceException>(retrieveAllConsumerAdoptionsTask.AsTask);

            // then
            actualConsumerAdoptionServiceException.Should()
                .BeEquivalentTo(expectedConsumerAdoptionServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllConsumerAdoptionsAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedConsumerAdoptionServiceException))),
                        Times.Once);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
