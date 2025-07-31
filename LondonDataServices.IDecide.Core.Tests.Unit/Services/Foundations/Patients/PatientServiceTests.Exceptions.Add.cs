// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using Moq;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients.Exceptions;
using FluentAssertions;
using EFxceptions.Models.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.Patients
{
    public partial class PatientServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnAddIfSqlErrorOccursAndLogItAsync()
        {
            // given
            string randomNhsNumber = GenerateRandom10DigitNumber();
            string randomValidationCode = GenerateRandom5DigitNumber();
            Patient somePatient = CreateRandomPatient(randomNhsNumber, randomValidationCode);
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
                broker.InsertPatientAsync(somePatient))
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

            this.storageBrokerMock.Verify(broker =>
                broker.InsertPatientAsync(somePatient),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedPatientDependencyException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfPatientAlreadyExsitsAndLogItAsync()
        {
            // given
            string randomNhsNumber = GenerateRandom10DigitNumber();
            string randomValidationCode = GenerateRandom5DigitNumber();
            Patient somePatient = CreateRandomPatient(randomNhsNumber, randomValidationCode);
            Patient alreadyExistsPatient = somePatient;
            string randomMessage = GetRandomString();

            var duplicateKeyException =
                new DuplicateKeyException(randomMessage);

            var alreadyExistsPatientException =
                new AlreadyExistsPatientException(
                    message: "Patient with the same Id already exists.",
                    innerException: duplicateKeyException,
                    data: duplicateKeyException.Data);

            var expectedPatientDependencyValidationException =
                new PatientDependencyValidationException(
                    message: "Patient dependency validation occurred, please fix errors and try again.",
                    innerException: alreadyExistsPatientException);

            this.storageBrokerMock.Setup(broker =>
                broker.InsertPatientAsync(somePatient))
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

            this.storageBrokerMock.Verify(broker =>
                broker.InsertPatientAsync(somePatient),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedPatientDependencyValidationException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnAddIfDatabaseUpdateErrorOccursAndLogItAsync()
        {
            // given
            string randomNhsNumber = GenerateRandom10DigitNumber();
            string randomValidationCode = GenerateRandom5DigitNumber();
            Patient somePatient = CreateRandomPatient(randomNhsNumber, randomValidationCode);

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

            this.storageBrokerMock.Setup(broker =>
                broker.InsertPatientAsync(somePatient))
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

            this.storageBrokerMock.Verify(broker =>
                broker.InsertPatientAsync(somePatient),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedPatientDependencyException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnAddIfServiceErrorOccursAndLogItAsync()
        {
            // given
            string randomNhsNumber = GenerateRandom10DigitNumber();
            string randomValidationCode = GenerateRandom5DigitNumber();
            Patient somePatient = CreateRandomPatient(randomNhsNumber, randomValidationCode);
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
                broker.InsertPatientAsync(somePatient))
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

            this.storageBrokerMock.Verify(broker =>
                broker.InsertPatientAsync(somePatient),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedPatientServiceException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
