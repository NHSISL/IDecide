// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using FluentAssertions;
using LondonDataServices.IDecide.Core.Models.Foundations.ConsumerAdoptions;
using LondonDataServices.IDecide.Core.Models.Foundations.ConsumerAdoptions.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.ConsumerAdoptions
{
    public partial class ConsumerAdoptionServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnAddIfSqlErrorOccursAndLogItAsync()
        {
            // given
            ConsumerAdoption someConsumerAdoption = CreateRandomConsumerAdoption();
            SqlException sqlException = GetSqlException();

            var failedConsumerAdoptionStorageException =
                new FailedConsumerAdoptionStorageException(
                    message: "Failed consumerAdoption storage error occurred, contact support.",
                    innerException: sqlException);

            var expectedConsumerAdoptionDependencyException =
                new ConsumerAdoptionDependencyException(
                    message: "ConsumerAdoption dependency error occurred, contact support.",
                    innerException: failedConsumerAdoptionStorageException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyAddAuditValuesAsync(It.IsAny<ConsumerAdoption>()))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<ConsumerAdoption> addConsumerAdoptionTask =
                this.consumerAdoptionService.AddConsumerAdoptionAsync(someConsumerAdoption);

            ConsumerAdoptionDependencyException actualConsumerAdoptionDependencyException =
                await Assert.ThrowsAsync<ConsumerAdoptionDependencyException>(
                    addConsumerAdoptionTask.AsTask);

            // then
            actualConsumerAdoptionDependencyException.Should()
                .BeEquivalentTo(expectedConsumerAdoptionDependencyException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyAddAuditValuesAsync(It.IsAny<ConsumerAdoption>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedConsumerAdoptionDependencyException))),
                        Times.Once);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.GetCurrentUserIdAsync(),
                    Times.Never);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Never());

            this.storageBrokerMock.Verify(broker =>
                broker.InsertConsumerAdoptionAsync(It.IsAny<ConsumerAdoption>()),
                    Times.Never);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfConsumerAdoptionAlreadyExistsAndLogItAsync()
        {
            // given
            ConsumerAdoption randomConsumerAdoption = CreateRandomConsumerAdoption();
            ConsumerAdoption alreadyExistsConsumerAdoption = randomConsumerAdoption;
            string randomMessage = GetRandomString();

            var duplicateKeyException =
                new DuplicateKeyException(randomMessage);

            var alreadyExistsConsumerAdoptionException =
                new AlreadyExistsConsumerAdoptionException(
                    message: "ConsumerAdoption with the same Id already exists.",
                    innerException: duplicateKeyException);

            var expectedConsumerAdoptionDependencyValidationException =
                new ConsumerAdoptionDependencyValidationException(
                    message: "ConsumerAdoption dependency validation occurred, please try again.",
                    innerException: alreadyExistsConsumerAdoptionException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyAddAuditValuesAsync(It.IsAny<ConsumerAdoption>()))
                    .ThrowsAsync(duplicateKeyException);

            // when
            ValueTask<ConsumerAdoption> addConsumerAdoptionTask =
                this.consumerAdoptionService.AddConsumerAdoptionAsync(alreadyExistsConsumerAdoption);

            // then
            ConsumerAdoptionDependencyValidationException actualConsumerAdoptionDependencyValidationException =
                await Assert.ThrowsAsync<ConsumerAdoptionDependencyValidationException>(
                    addConsumerAdoptionTask.AsTask);

            actualConsumerAdoptionDependencyValidationException.Should()
                .BeEquivalentTo(expectedConsumerAdoptionDependencyValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyAddAuditValuesAsync(It.IsAny<ConsumerAdoption>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedConsumerAdoptionDependencyValidationException))),
                        Times.Once);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.GetCurrentUserIdAsync(),
                    Times.Never);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Never());

            this.storageBrokerMock.Verify(broker =>
                broker.InsertConsumerAdoptionAsync(It.IsAny<ConsumerAdoption>()),
                    Times.Never);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfReferenceErrorOccursAndLogItAsync()
        {
            // given
            ConsumerAdoption someConsumerAdoption = CreateRandomConsumerAdoption();
            string randomMessage = GetRandomString();
            string exceptionMessage = randomMessage;

            var foreignKeyConstraintConflictException =
                new ForeignKeyConstraintConflictException(exceptionMessage);

            var invalidConsumerAdoptionReferenceException =
                new InvalidConsumerAdoptionReferenceException(
                    message: "Invalid consumerAdoption reference error occurred.",
                    innerException: foreignKeyConstraintConflictException);

            var expectedConsumerAdoptionValidationException =
                new ConsumerAdoptionDependencyValidationException(
                    message: "ConsumerAdoption dependency validation occurred, please try again.",
                    innerException: invalidConsumerAdoptionReferenceException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyAddAuditValuesAsync(It.IsAny<ConsumerAdoption>()))
                    .ThrowsAsync(foreignKeyConstraintConflictException);

            // when
            ValueTask<ConsumerAdoption> addConsumerAdoptionTask =
                this.consumerAdoptionService.AddConsumerAdoptionAsync(someConsumerAdoption);

            // then
            ConsumerAdoptionDependencyValidationException actualConsumerAdoptionDependencyValidationException =
                await Assert.ThrowsAsync<ConsumerAdoptionDependencyValidationException>(
                    addConsumerAdoptionTask.AsTask);

            actualConsumerAdoptionDependencyValidationException.Should()
                .BeEquivalentTo(expectedConsumerAdoptionValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyAddAuditValuesAsync(It.IsAny<ConsumerAdoption>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedConsumerAdoptionValidationException))),
                        Times.Once);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.GetCurrentUserIdAsync(),
                    Times.Never);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Never());

            this.storageBrokerMock.Verify(broker =>
                broker.InsertConsumerAdoptionAsync(It.IsAny<ConsumerAdoption>()),
                    Times.Never);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnAddIfDatabaseUpdateErrorOccursAndLogItAsync()
        {
            // given
            ConsumerAdoption someConsumerAdoption = CreateRandomConsumerAdoption();

            var databaseUpdateException = new DbUpdateException();

            var failedConsumerAdoptionStorageException =
                new FailedConsumerAdoptionStorageException(
                    message: "Failed consumerAdoption storage error occurred, contact support.",
                    innerException: databaseUpdateException);

            var expectedConsumerAdoptionDependencyException =
                new ConsumerAdoptionDependencyException(
                    message: "ConsumerAdoption dependency error occurred, contact support.",
                    innerException: failedConsumerAdoptionStorageException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyAddAuditValuesAsync(It.IsAny<ConsumerAdoption>()))
                    .ThrowsAsync(databaseUpdateException);

            // when
            ValueTask<ConsumerAdoption> addConsumerAdoptionTask =
                this.consumerAdoptionService.AddConsumerAdoptionAsync(someConsumerAdoption);

            ConsumerAdoptionDependencyException actualConsumerAdoptionDependencyException =
                await Assert.ThrowsAsync<ConsumerAdoptionDependencyException>(
                    addConsumerAdoptionTask.AsTask);

            // then
            actualConsumerAdoptionDependencyException.Should()
                .BeEquivalentTo(expectedConsumerAdoptionDependencyException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyAddAuditValuesAsync(It.IsAny<ConsumerAdoption>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedConsumerAdoptionDependencyException))),
                        Times.Once);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnAddIfServiceErrorOccursAndLogItAsync()
        {
            // given
            ConsumerAdoption someConsumerAdoption = CreateRandomConsumerAdoption();
            var serviceException = new Exception();

            var failedConsumerAdoptionServiceException =
                new FailedConsumerAdoptionServiceException(
                    message: "Failed consumerAdoption service occurred, please contact support",
                    innerException: serviceException);

            var expectedConsumerAdoptionServiceException =
                new ConsumerAdoptionServiceException(
                    message: "ConsumerAdoption service error occurred, contact support.",
                    innerException: failedConsumerAdoptionServiceException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyAddAuditValuesAsync(It.IsAny<ConsumerAdoption>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<ConsumerAdoption> addConsumerAdoptionTask =
                this.consumerAdoptionService.AddConsumerAdoptionAsync(someConsumerAdoption);

            ConsumerAdoptionServiceException actualConsumerAdoptionServiceException =
                await Assert.ThrowsAsync<ConsumerAdoptionServiceException>(
                    addConsumerAdoptionTask.AsTask);

            // then
            actualConsumerAdoptionServiceException.Should()
                .BeEquivalentTo(expectedConsumerAdoptionServiceException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyAddAuditValuesAsync(It.IsAny<ConsumerAdoption>()),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertConsumerAdoptionAsync(It.IsAny<ConsumerAdoption>()),
                    Times.Never);

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
