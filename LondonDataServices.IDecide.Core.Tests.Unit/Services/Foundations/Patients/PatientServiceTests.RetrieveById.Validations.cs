// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients.Exceptions;
using Moq;

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
                values: "Id is required");

            var expectedPatientValidationException =
                new PatientValidationException(
                    message: "Patient validation errors occurred, please try again.",
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

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowNotFoundExceptionOnRetrieveByIdIfPatientIsNotFoundAndLogItAsync()
        {
            //given
            Guid somePatientId = Guid.NewGuid();
            Patient noPatient = null;

            var notFoundPatientException = new NotFoundPatientException(
                $"Couldn't find patient with patientId: {somePatientId}.");

            var expectedPatientValidationException =
                new PatientValidationException(
                    message: "Patient validation errors occurred, please try again.",
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

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}