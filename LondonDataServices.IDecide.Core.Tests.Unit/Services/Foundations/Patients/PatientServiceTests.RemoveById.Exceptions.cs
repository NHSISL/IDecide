// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
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
        public async Task ShouldThrowCriticalDependencyExceptionOnRemoveIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Patient randomPatient = CreateRandomPatient();
            SqlException sqlException = GetSqlException();

            var failedPatientStorageException =
                new FailedPatientStorageException(
                    message: "Failed patient storage error occurred, contact support.",
                    innerException: sqlException);

            var expectedPatientDependencyException =
                new PatientDependencyException(
                    message: "Patient dependency error occurred, contact support.",
                    innerException: failedPatientStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectPatientByIdAsync(randomPatient.Id))
                    .Throws(sqlException);

            // when
            ValueTask<Patient> addPatientTask =
                this.patientService.RemovePatientByIdAsync(randomPatient.Id);

            PatientDependencyException actualPatientDependencyException =
                await Assert.ThrowsAsync<PatientDependencyException>(
                    addPatientTask.AsTask);

            // then
            actualPatientDependencyException.Should()
                .BeEquivalentTo(expectedPatientDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectPatientByIdAsync(randomPatient.Id),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedPatientDependencyException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeletePatientAsync(It.IsAny<Patient>()),
                    Times.Never);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Never);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationOnRemoveIfDatabaseUpdateConcurrencyErrorOccursAndLogItAsync()
        {
            // given
            Guid somePatientId = Guid.NewGuid();

            var databaseUpdateConcurrencyException =
                new DbUpdateConcurrencyException();

            var lockedPatientException =
                new LockedPatientException(
                    message: "Locked patient record exception, please try again later",
                    innerException: databaseUpdateConcurrencyException);

            var expectedPatientDependencyValidationException =
                new PatientDependencyValidationException(
                    message: "Patient dependency validation occurred, please try again.",
                    innerException: lockedPatientException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectPatientByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(databaseUpdateConcurrencyException);

            // when
            ValueTask<Patient> removePatientByIdTask =
                this.patientService.RemovePatientByIdAsync(somePatientId);

            PatientDependencyValidationException actualPatientDependencyValidationException =
                await Assert.ThrowsAsync<PatientDependencyValidationException>(
                    removePatientByIdTask.AsTask);

            // then
            actualPatientDependencyValidationException.Should()
                .BeEquivalentTo(expectedPatientDependencyValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectPatientByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedPatientDependencyValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeletePatientAsync(It.IsAny<Patient>()),
                    Times.Never);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRemoveWhenSqlExceptionOccursAndLogItAsync()
        {
            // given
            Guid somePatientId = Guid.NewGuid();
            SqlException sqlException = GetSqlException();

            var failedPatientStorageException =
                new FailedPatientStorageException(
                    message: "Failed patient storage error occurred, contact support.",
                    innerException: sqlException);

            var expectedPatientDependencyException =
                new PatientDependencyException(
                    message: "Patient dependency error occurred, contact support.",
                    innerException: failedPatientStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectPatientByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<Patient> deletePatientTask =
                this.patientService.RemovePatientByIdAsync(somePatientId);

            PatientDependencyException actualPatientDependencyException =
                await Assert.ThrowsAsync<PatientDependencyException>(
                    deletePatientTask.AsTask);

            // then
            actualPatientDependencyException.Should()
                .BeEquivalentTo(expectedPatientDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectPatientByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedPatientDependencyException))),
                        Times.Once);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRemoveIfServiceErrorOccursAndLogItAsync()
        {
            // given
            Guid somePatientId = Guid.NewGuid();
            var serviceException = new Exception();

            var failedPatientServiceException =
                new FailedPatientServiceException(
                    message: "Failed patient service occurred, please contact support",
                    innerException: serviceException);

            var expectedPatientServiceException =
                new PatientServiceException(
                    message: "Patient service error occurred, contact support.",
                    innerException: failedPatientServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectPatientByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<Patient> removePatientByIdTask =
                this.patientService.RemovePatientByIdAsync(somePatientId);

            PatientServiceException actualPatientServiceException =
                await Assert.ThrowsAsync<PatientServiceException>(
                    removePatientByIdTask.AsTask);

            // then
            actualPatientServiceException.Should()
                .BeEquivalentTo(expectedPatientServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectPatientByIdAsync(It.IsAny<Guid>()),
                        Times.Once());

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