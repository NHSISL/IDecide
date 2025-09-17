// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using FluentAssertions;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.Patients
{
    public partial class PatientServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnAddIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Patient somePatient = CreateRandomPatient();
            SqlException sqlException = GetSqlException();

            var failedPatientStorageException =
                new FailedPatientStorageException(
                    message: "Failed patient storage error occurred, contact support.",
                    innerException: sqlException);

            var expectedPatientDependencyException =
                new PatientDependencyException(
                    message: "Patient dependency error occurred, contact support.",
                    innerException: failedPatientStorageException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyAddAuditValuesAsync(It.IsAny<Patient>()))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<Patient> addPatientTask =
                this.patientService.AddPatientAsync(somePatient);

            PatientDependencyException actualPatientDependencyException =
                await Assert.ThrowsAsync<PatientDependencyException>(
                    addPatientTask.AsTask);

            // then
            actualPatientDependencyException.Should()
                .BeEquivalentTo(expectedPatientDependencyException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyAddAuditValuesAsync(It.IsAny<Patient>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedPatientDependencyException))),
                        Times.Once);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.GetCurrentUserIdAsync(),
                    Times.Never);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Never());

            this.storageBrokerMock.Verify(broker =>
                broker.InsertPatientAsync(It.IsAny<Patient>()),
                    Times.Never);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfPatientAlreadyExsitsAndLogItAsync()
        {
            // given
            Patient randomPatient = CreateRandomPatient();
            Patient alreadyExistsPatient = randomPatient;
            string randomMessage = GetRandomString();

            var duplicateKeyException =
                new DuplicateKeyException(randomMessage);

            var alreadyExistsPatientException =
                new AlreadyExistsPatientException(
                    message: "Patient with the same Id already exists.",
                    innerException: duplicateKeyException);

            var expectedPatientDependencyValidationException =
                new PatientDependencyValidationException(
                    message: "Patient dependency validation occurred, please try again.",
                    innerException: alreadyExistsPatientException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyAddAuditValuesAsync(It.IsAny<Patient>()))
                    .ThrowsAsync(duplicateKeyException);

            // when
            ValueTask<Patient> addPatientTask =
                this.patientService.AddPatientAsync(alreadyExistsPatient);

            // then
            PatientDependencyValidationException actualPatientDependencyValidationException =
                await Assert.ThrowsAsync<PatientDependencyValidationException>(
                    addPatientTask.AsTask);

            actualPatientDependencyValidationException.Should()
                .BeEquivalentTo(expectedPatientDependencyValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyAddAuditValuesAsync(It.IsAny<Patient>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedPatientDependencyValidationException))),
                        Times.Once);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.GetCurrentUserIdAsync(),
                    Times.Never);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Never());

            this.storageBrokerMock.Verify(broker =>
                broker.InsertPatientAsync(It.IsAny<Patient>()),
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
            Patient somePatient = CreateRandomPatient();
            string randomMessage = GetRandomString();
            string exceptionMessage = randomMessage;

            var foreignKeyConstraintConflictException =
                new ForeignKeyConstraintConflictException(exceptionMessage);

            var invalidPatientReferenceException =
                new InvalidPatientReferenceException(
                    message: "Invalid patient reference error occurred.",
                    innerException: foreignKeyConstraintConflictException);

            var expectedPatientValidationException =
                new PatientDependencyValidationException(
                    message: "Patient dependency validation occurred, please try again.",
                    innerException: invalidPatientReferenceException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyAddAuditValuesAsync(It.IsAny<Patient>()))
                    .ThrowsAsync(foreignKeyConstraintConflictException);

            // when
            ValueTask<Patient> addPatientTask =
                this.patientService.AddPatientAsync(somePatient);

            // then
            PatientDependencyValidationException actualPatientDependencyValidationException =
                await Assert.ThrowsAsync<PatientDependencyValidationException>(
                    addPatientTask.AsTask);

            actualPatientDependencyValidationException.Should()
                .BeEquivalentTo(expectedPatientValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyAddAuditValuesAsync(It.IsAny<Patient>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedPatientValidationException))),
                        Times.Once);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.GetCurrentUserIdAsync(),
                    Times.Never);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Never());

            this.storageBrokerMock.Verify(broker =>
                broker.InsertPatientAsync(It.IsAny<Patient>()),
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
            Patient somePatient = CreateRandomPatient();

            var databaseUpdateException =
                new DbUpdateException();

            var failedPatientStorageException =
                new FailedPatientStorageException(
                    message: "Failed patient storage error occurred, contact support.",
                    innerException: databaseUpdateException);

            var expectedPatientDependencyException =
                new PatientDependencyException(
                    message: "Patient dependency error occurred, contact support.",
                    innerException: failedPatientStorageException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyAddAuditValuesAsync(It.IsAny<Patient>()))
                    .ThrowsAsync(databaseUpdateException);

            // when
            ValueTask<Patient> addPatientTask =
                this.patientService.AddPatientAsync(somePatient);

            PatientDependencyException actualPatientDependencyException =
                await Assert.ThrowsAsync<PatientDependencyException>(
                    addPatientTask.AsTask);

            // then
            actualPatientDependencyException.Should()
                .BeEquivalentTo(expectedPatientDependencyException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyAddAuditValuesAsync(It.IsAny<Patient>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedPatientDependencyException))),
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
            Patient somePatient = CreateRandomPatient();
            var serviceException = new Exception();

            var failedPatientServiceException =
                new FailedPatientServiceException(
                    message: "Failed patient service occurred, please contact support",
                    innerException: serviceException);

            var expectedPatientServiceException =
                new PatientServiceException(
                    message: "Patient service error occurred, contact support.",
                    innerException: failedPatientServiceException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyAddAuditValuesAsync(It.IsAny<Patient>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<Patient> addPatientTask =
                this.patientService.AddPatientAsync(somePatient);

            PatientServiceException actualPatientServiceException =
                await Assert.ThrowsAsync<PatientServiceException>(
                    addPatientTask.AsTask);

            // then
            actualPatientServiceException.Should()
                .BeEquivalentTo(expectedPatientServiceException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyAddAuditValuesAsync(It.IsAny<Patient>()),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertPatientAsync(It.IsAny<Patient>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedPatientServiceException))),
                        Times.Once);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}