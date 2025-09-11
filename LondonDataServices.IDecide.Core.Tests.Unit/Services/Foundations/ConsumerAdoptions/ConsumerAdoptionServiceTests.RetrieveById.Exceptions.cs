// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
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
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveByIdIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Guid someId = Guid.NewGuid();
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
                broker.SelectConsumerAdoptionByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<ConsumerAdoption> retrieveConsumerAdoptionByIdTask =
                this.consumerAdoptionService.RetrieveConsumerAdoptionByIdAsync(someId);

            ConsumerAdoptionDependencyException actualConsumerAdoptionDependencyException =
                await Assert.ThrowsAsync<ConsumerAdoptionDependencyException>(
                    retrieveConsumerAdoptionByIdTask.AsTask);

            // then
            actualConsumerAdoptionDependencyException.Should()
                .BeEquivalentTo(expectedConsumerAdoptionDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectConsumerAdoptionByIdAsync(It.IsAny<Guid>()),
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
        public async Task ShouldThrowServiceExceptionOnRetrieveByIdIfServiceErrorOccursAndLogItAsync()
        {
            // given
            Guid someId = Guid.NewGuid();
            var serviceException = new Exception();

            var failedConsumerAdoptionServiceException =
                new FailedConsumerAdoptionServiceException(
                    message: "Failed consumerAdoption service occurred, please contact support",
                    innerException: serviceException);

            var expectedConsumerAdoptionServiceException =
                new ConsumerAdoptionServiceException(
                    message: "ConsumerAdoption service error occurred, contact support.",
                    innerException: failedConsumerAdoptionServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectConsumerAdoptionByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<ConsumerAdoption> retrieveConsumerAdoptionByIdTask =
                this.consumerAdoptionService.RetrieveConsumerAdoptionByIdAsync(someId);

            ConsumerAdoptionServiceException actualConsumerAdoptionServiceException =
                await Assert.ThrowsAsync<ConsumerAdoptionServiceException>(
                    retrieveConsumerAdoptionByIdTask.AsTask);

            // then
            actualConsumerAdoptionServiceException.Should()
                .BeEquivalentTo(expectedConsumerAdoptionServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectConsumerAdoptionByIdAsync(It.IsAny<Guid>()),
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
