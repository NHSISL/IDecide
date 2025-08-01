// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using Moq;
using FluentAssertions;
using System.Threading.Tasks;
using System;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients.Exceptions;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.Patients
{
    public partial class PatientServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRetrieveByIdIfIdIsInvalidAndLogItAsync()
        {
            // given
            var invalidPatientId = Guid.Empty;

            var invalidPatientException =
                new InvalidPatientException(
                    message: "Invalid patient. Please correct the errors and try again.");

            invalidPatientException.AddData(
                key: nameof(Patient.Id),
                values: "Id is invalid");

            var expectedPatientValidationException =
                new PatientValidationException(
                    message: "Patient validation error occurred, please fix errors and try again.",
                    innerException: invalidPatientException);

            // when
            ValueTask<Patient> retrievePatientByIdTask =
                this.patientService.RetrievePatientByIdAsync(invalidPatientId);

            PatientValidationException actualPatientValidationException =
                await Assert.ThrowsAsync<PatientValidationException>(
                    retrievePatientByIdTask.AsTask);

            // then
            actualPatientValidationException.Should()
                .BeEquivalentTo(expectedPatientValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedPatientValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectPatientByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowNotFoundExceptionOnRetrieveByIdIfPatientIsNotFoundAndLogItAsync()
        {
            //given
            Guid somePatientId = Guid.NewGuid();
            Patient noPatient = null;

            var notFoundPatientException =
                new NotFoundPatientException(
                    $"Couldn't find patient with patientId: {somePatientId}.");

            var expectedPatientValidationException =
                new PatientValidationException(
                    message: "Patient validation error occurred, please fix errors and try again.",
                    innerException: notFoundPatientException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectPatientByIdAsync(It.IsAny<Guid>()))
                    .ReturnsAsync(noPatient);

            //when
            ValueTask<Patient> retrievePatientByIdTask =
                this.patientService.RetrievePatientByIdAsync(somePatientId);

            PatientValidationException actualPatientValidationException =
                await Assert.ThrowsAsync<PatientValidationException>(
                    retrievePatientByIdTask.AsTask);

            //then
            actualPatientValidationException.Should().BeEquivalentTo(expectedPatientValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectPatientByIdAsync(It.IsAny<Guid>()),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedPatientValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
