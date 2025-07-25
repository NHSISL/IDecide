// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Core.Models.Foundations.DecisionTypes.Exceptions;
using LondonDataServices.IDecide.Core.Models.Foundations.DecisionTypes;
using Moq;
using EFxceptions.Models.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.DecisionTypes
{
    public partial class DecisionTypeServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnAddIfSqlErrorOccursAndLogItAsync()
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
                this.decisionTypeService.AddDecisionTypeAsync(someDecisionType);

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
                broker.InsertDecisionTypeAsync(It.IsAny<DecisionType>()),
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
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfDecisionTypeAlreadyExsitsAndLogItAsync()
        {
            // given
            DecisionType randomDecisionType = CreateRandomDecisionType();
            DecisionType alreadyExistsDecisionType = randomDecisionType;
            string randomMessage = GetRandomString();

            var duplicateKeyException =
                new DuplicateKeyException(randomMessage);

            var alreadyExistsDecisionTypeException =
                new AlreadyExistsDecisionTypeException(
                    message: "DecisionType with the same Id already exists.",
                    innerException: duplicateKeyException,
                    data: duplicateKeyException.Data);

            var expectedDecisionTypeDependencyValidationException =
                new DecisionTypeDependencyValidationException(
                    message: "DecisionType dependency validation occurred, please fix errors and try again.",
                    innerException: alreadyExistsDecisionTypeException);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ThrowsAsync(duplicateKeyException);

            // when
            ValueTask<DecisionType> addDecisionTypeTask =
                this.decisionTypeService.AddDecisionTypeAsync(alreadyExistsDecisionType);

            // then
            DecisionTypeDependencyValidationException actualDecisionTypeDependencyValidationException =
                await Assert.ThrowsAsync<DecisionTypeDependencyValidationException>(
                    addDecisionTypeTask.AsTask);

            actualDecisionTypeDependencyValidationException.Should()
                .BeEquivalentTo(expectedDecisionTypeDependencyValidationException);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertDecisionTypeAsync(It.IsAny<DecisionType>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedDecisionTypeDependencyValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnAddIfDatabaseUpdateErrorOccursAndLogItAsync()
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
                this.decisionTypeService.AddDecisionTypeAsync(someDecisionType);

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
                broker.InsertDecisionTypeAsync(It.IsAny<DecisionType>()),
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
        public async Task ShouldThrowServiceExceptionOnAddIfServiceErrorOccursAndLogItAsync()
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
            ValueTask<DecisionType> addDecisionTypeTask =
                this.decisionTypeService.AddDecisionTypeAsync(someDecisionType);

            DecisionTypeServiceException actualDecisionTypeServiceException =
                await Assert.ThrowsAsync<DecisionTypeServiceException>(
                    addDecisionTypeTask.AsTask);

            // then
            actualDecisionTypeServiceException.Should()
                .BeEquivalentTo(expectedDecisionTypeServiceException);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertDecisionTypeAsync(It.IsAny<DecisionType>()),
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
