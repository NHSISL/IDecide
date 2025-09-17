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
        public async Task ShouldThrowCriticalDependencyExceptionOnModifyIfSqlErrorOccursAndLogItAsync()
        {
            // given
            ConsumerAdoption randomConsumerAdoption = CreateRandomConsumerAdoption();
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
                broker.ApplyModifyAuditValuesAsync(It.IsAny<ConsumerAdoption>()))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<ConsumerAdoption> modifyConsumerAdoptionTask =
                this.consumerAdoptionService.ModifyConsumerAdoptionAsync(randomConsumerAdoption);

            ConsumerAdoptionDependencyException actualConsumerAdoptionDependencyException =
                await Assert.ThrowsAsync<ConsumerAdoptionDependencyException>(
                    modifyConsumerAdoptionTask.AsTask);

            // then
            actualConsumerAdoptionDependencyException.Should()
                .BeEquivalentTo(expectedConsumerAdoptionDependencyException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyModifyAuditValuesAsync(It.IsAny<ConsumerAdoption>()),
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
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectConsumerAdoptionByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.securityAuditBrokerMock.Verify(broker => broker
                .EnsureAddAuditValuesRemainsUnchangedOnModifyAsync(
                    It.IsAny<ConsumerAdoption>(),
                    It.IsAny<ConsumerAdoption>()),
                        Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateConsumerAdoptionAsync(It.IsAny<ConsumerAdoption>()),
                    Times.Never);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.securityAuditBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfReferenceErrorOccursAndLogItAsync()
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

            ConsumerAdoptionDependencyValidationException expectedConsumerAdoptionDependencyValidationException =
                new ConsumerAdoptionDependencyValidationException(
                    message: "ConsumerAdoption dependency validation occurred, please try again.",
                    innerException: invalidConsumerAdoptionReferenceException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyModifyAuditValuesAsync(It.IsAny<ConsumerAdoption>()))
                    .ThrowsAsync(foreignKeyConstraintConflictException);

            // when
            ValueTask<ConsumerAdoption> modifyConsumerAdoptionTask =
                this.consumerAdoptionService.ModifyConsumerAdoptionAsync(someConsumerAdoption);

            ConsumerAdoptionDependencyValidationException actualConsumerAdoptionDependencyValidationException =
                await Assert.ThrowsAsync<ConsumerAdoptionDependencyValidationException>(
                    modifyConsumerAdoptionTask.AsTask);

            // then
            actualConsumerAdoptionDependencyValidationException.Should()
                .BeEquivalentTo(expectedConsumerAdoptionDependencyValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyModifyAuditValuesAsync(It.IsAny<ConsumerAdoption>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedConsumerAdoptionDependencyValidationException))),
                    Times.Once);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.GetCurrentUserIdAsync(),
                    Times.Never);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectConsumerAdoptionByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.securityAuditBrokerMock.Verify(broker => broker
                .EnsureAddAuditValuesRemainsUnchangedOnModifyAsync(
                    It.IsAny<ConsumerAdoption>(),
                    It.IsAny<ConsumerAdoption>()),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateConsumerAdoptionAsync(It.IsAny<ConsumerAdoption>()),
                    Times.Never);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.securityAuditBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnModifyIfDatabaseUpdateExceptionOccursAndLogItAsync()
        {
            // given
            ConsumerAdoption randomConsumerAdoption = CreateRandomConsumerAdoption();
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
                broker.ApplyModifyAuditValuesAsync(It.IsAny<ConsumerAdoption>()))
                    .ThrowsAsync(databaseUpdateException);

            // when
            ValueTask<ConsumerAdoption> modifyConsumerAdoptionTask =
                this.consumerAdoptionService.ModifyConsumerAdoptionAsync(randomConsumerAdoption);

            ConsumerAdoptionDependencyException actualConsumerAdoptionDependencyException =
                await Assert.ThrowsAsync<ConsumerAdoptionDependencyException>(
                    modifyConsumerAdoptionTask.AsTask);

            // then
            actualConsumerAdoptionDependencyException.Should()
                .BeEquivalentTo(expectedConsumerAdoptionDependencyException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyModifyAuditValuesAsync(It.IsAny<ConsumerAdoption>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedConsumerAdoptionDependencyException))),
                        Times.Once);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.GetCurrentUserIdAsync(),
                    Times.Never);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectConsumerAdoptionByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.securityAuditBrokerMock.Verify(broker => broker
                .EnsureAddAuditValuesRemainsUnchangedOnModifyAsync(
                    It.IsAny<ConsumerAdoption>(),
                    It.IsAny<ConsumerAdoption>()),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateConsumerAdoptionAsync(It.IsAny<ConsumerAdoption>()),
                    Times.Never);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.securityAuditBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnModifyIfDbUpdateConcurrencyErrorOccursAndLogAsync()
        {
            // given
            ConsumerAdoption randomConsumerAdoption = CreateRandomConsumerAdoption();
            var databaseUpdateConcurrencyException = new DbUpdateConcurrencyException();

            var lockedConsumerAdoptionException =
                new LockedConsumerAdoptionException(
                    message: "Locked consumerAdoption record exception, please try again later",
                    innerException: databaseUpdateConcurrencyException);

            var expectedConsumerAdoptionDependencyValidationException =
                new ConsumerAdoptionDependencyValidationException(
                    message: "ConsumerAdoption dependency validation occurred, please try again.",
                    innerException: lockedConsumerAdoptionException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyModifyAuditValuesAsync(It.IsAny<ConsumerAdoption>()))
                    .ThrowsAsync(databaseUpdateConcurrencyException);

            // when
            ValueTask<ConsumerAdoption> modifyConsumerAdoptionTask =
                this.consumerAdoptionService.ModifyConsumerAdoptionAsync(randomConsumerAdoption);

            ConsumerAdoptionDependencyValidationException actualConsumerAdoptionDependencyValidationException =
                await Assert.ThrowsAsync<ConsumerAdoptionDependencyValidationException>(
                    modifyConsumerAdoptionTask.AsTask);

            // then
            actualConsumerAdoptionDependencyValidationException.Should()
                .BeEquivalentTo(expectedConsumerAdoptionDependencyValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyModifyAuditValuesAsync(It.IsAny<ConsumerAdoption>()),
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
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectConsumerAdoptionByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.securityAuditBrokerMock.Verify(broker => broker
                .EnsureAddAuditValuesRemainsUnchangedOnModifyAsync(
                    It.IsAny<ConsumerAdoption>(),
                    It.IsAny<ConsumerAdoption>()),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateConsumerAdoptionAsync(It.IsAny<ConsumerAdoption>()),
                    Times.Never);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.securityAuditBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnModifyIfServiceErrorOccursAndLogItAsync()
        {
            // given
            ConsumerAdoption randomConsumerAdoption = CreateRandomConsumerAdoption();
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
                broker.ApplyModifyAuditValuesAsync(It.IsAny<ConsumerAdoption>()))
                    .Throws(serviceException);

            // when
            ValueTask<ConsumerAdoption> modifyConsumerAdoptionTask =
                this.consumerAdoptionService.ModifyConsumerAdoptionAsync(randomConsumerAdoption);

            ConsumerAdoptionServiceException actualConsumerAdoptionServiceException =
                await Assert.ThrowsAsync<ConsumerAdoptionServiceException>(
                    modifyConsumerAdoptionTask.AsTask);

            // then
            actualConsumerAdoptionServiceException.Should()
                .BeEquivalentTo(expectedConsumerAdoptionServiceException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyModifyAuditValuesAsync(It.IsAny<ConsumerAdoption>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedConsumerAdoptionServiceException))),
                        Times.Once);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.GetCurrentUserIdAsync(),
                    Times.Never);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectConsumerAdoptionByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.securityAuditBrokerMock.Verify(broker => broker
                .EnsureAddAuditValuesRemainsUnchangedOnModifyAsync(
                    It.IsAny<ConsumerAdoption>(),
                    It.IsAny<ConsumerAdoption>()),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateConsumerAdoptionAsync(It.IsAny<ConsumerAdoption>()),
                    Times.Never);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.securityAuditBrokerMock.VerifyNoOtherCalls();
        }
    }
}
