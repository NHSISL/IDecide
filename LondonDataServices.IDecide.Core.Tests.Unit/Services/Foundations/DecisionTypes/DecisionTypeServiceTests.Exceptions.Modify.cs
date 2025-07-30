// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Core.Models.Foundations.DecisionTypes;
using LondonDataServices.IDecide.Core.Models.Foundations.DecisionTypes.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.DecisionTypes
{
    public partial class DecisionTypeServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnModifyIfSqlErrorOccursAndLogItAsync()
        {
            // given
            DecisionType someDecisionType = CreateRandomDecisionType();
            SqlException sqlException = GetSqlException();

            var failedDecisionTypeStorageException =
                new FailedDecisionTypeStorageException(
                    message: "Failed decision type storage error occurred, contact support.",
                    innerException: sqlException);

            var expectedDecisionTypeDependencyException =
                new DecisionTypeDependencyException(
                    message: "DecisionType dependency error occurred, contact support.",
                    innerException: failedDecisionTypeStorageException);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<DecisionType> addDecisionTypeTask =
                this.decisionTypeService.ModifyDecisionTypeAsync(someDecisionType);

            DecisionTypeDependencyException actualDecisionTypeDependencyException =
                await Assert.ThrowsAsync<DecisionTypeDependencyException>(
                    addDecisionTypeTask.AsTask);

            // then
            actualDecisionTypeDependencyException.Should()
                .BeEquivalentTo(expectedDecisionTypeDependencyException);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateDecisionTypeAsync(It.IsAny<DecisionType>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedDecisionTypeDependencyException))),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnModifyIfDatabaseUpdateErrorOccursAndLogItAsync()
        {
            // given
            DecisionType someDecisionType = CreateRandomDecisionType();

            var databaseUpdateException =
                new DbUpdateException();

            var failedDecisionTypeStorageException =
                new FailedDecisionTypeStorageException(
                    message: "Failed decision type storage error occurred, contact support.",
                    innerException: databaseUpdateException);

            var expectedDecisionTypeDependencyException =
                new DecisionTypeDependencyException(
                    message: "DecisionType dependency error occurred, contact support.",
                    innerException: failedDecisionTypeStorageException);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ThrowsAsync(databaseUpdateException);

            // when
            ValueTask<DecisionType> addDecisionTypeTask =
                this.decisionTypeService.ModifyDecisionTypeAsync(someDecisionType);

            DecisionTypeDependencyException actualDecisionTypeDependencyException =
                await Assert.ThrowsAsync<DecisionTypeDependencyException>(
                    addDecisionTypeTask.AsTask);

            // then
            actualDecisionTypeDependencyException.Should()
                .BeEquivalentTo(expectedDecisionTypeDependencyException);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateDecisionTypeAsync(It.IsAny<DecisionType>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedDecisionTypeDependencyException))),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnModifyIfDbUpdateConcurrencyErrorOccursAndLogAsync()
        {
            // given
            DecisionType randomDecisionType = CreateRandomDecisionType();
            var databaseUpdateConcurrencyException = new DbUpdateConcurrencyException();

            var lockedDecisionTypeException =
                new LockedDecisionTypeException(
                    message: "Locked decision type record exception, please fix errors and try again.",
                    innerException: databaseUpdateConcurrencyException);

            var expectedDecisionTypeDependencyValidationException =
                new DecisionTypeDependencyValidationException(
                    message: "DecisionType dependency validation occurred, please fix errors and try again.",
                    innerException: lockedDecisionTypeException);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ThrowsAsync(databaseUpdateConcurrencyException);

            // when
            ValueTask<DecisionType> modifyDecisionTypeTask =
                this.decisionTypeService.ModifyDecisionTypeAsync(randomDecisionType);

            DecisionTypeDependencyValidationException actualDecisionTypeDependencyValidationException =
                await Assert.ThrowsAsync<DecisionTypeDependencyValidationException>(
                    modifyDecisionTypeTask.AsTask);

            // then
            actualDecisionTypeDependencyValidationException.Should()
                .BeEquivalentTo(expectedDecisionTypeDependencyValidationException);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectDecisionTypeByIdAsync(randomDecisionType.Id),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedDecisionTypeDependencyValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateDecisionTypeAsync(randomDecisionType),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnModifyIfServiceErrorOccursAndLogItAsync()
        {
            // given
            DecisionType someDecisionType = CreateRandomDecisionType();
            var serviceException = new Exception();

            var failedDecisionTypeServiceException =
                new FailedDecisionTypeServiceException(
                    message: "Failed decision type service occurred, please contact support",
                    innerException: serviceException);

            var expectedDecisionTypeServiceException =
                new DecisionTypeServiceException(
                    message: "DecisionType service error occurred, contact support.",
                    innerException: failedDecisionTypeServiceException);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<DecisionType> modifyDecisionTypeTask =
                this.decisionTypeService.ModifyDecisionTypeAsync(someDecisionType);

            DecisionTypeServiceException actualDecisionTypeServiceException =
                await Assert.ThrowsAsync<DecisionTypeServiceException>(
                    modifyDecisionTypeTask.AsTask);

            // then
            actualDecisionTypeServiceException.Should()
                .BeEquivalentTo(expectedDecisionTypeServiceException);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateDecisionTypeAsync(It.IsAny<DecisionType>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedDecisionTypeServiceException))),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}