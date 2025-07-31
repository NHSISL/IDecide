// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using Moq;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients.Exceptions;
using FluentAssertions;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.Patients
{
    public partial class PatientServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddPatientAsync()
        {
            // given
            Patient nullPatient = null;
            var nullPatientException = new NullPatientException(message: "Patient is null.");

            var expectedPatientValidationException =
                new PatientValidationException(
                    message: "Patient validation error occurred, please fix errors and try again.",
                    innerException: nullPatientException);

            // when
            ValueTask<Patient> addPatientTask = this.patientService.AddPatientAsync(nullPatient);

            PatientValidationException actualPatientValidationException =
                await Assert.ThrowsAsync<PatientValidationException>(testCode: addPatientTask.AsTask);

            // then
            actualPatientValidationException.Should().BeEquivalentTo(expectedPatientValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedPatientValidationException))), Times.Once());

            this.storageBrokerMock.Verify(broker =>
                broker.InsertPatientAsync(It.IsAny<Patient>()),
                    Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("1234")]
        public async Task ShouldThrowValidationExceptionOnAddIfPatientIsInvalidAndLogItAsync(string invalidText)
        {
            // given
            var invalidPatient = new Patient
            {
                NhsNumber = invalidText,
                ValidationCode = invalidText,
            };

            var invalidPatientException =
                new InvalidPatientException(
                    message: "Invalid patient. Please correct the errors and try again.");

            invalidPatientException.AddData(
                key: nameof(Patient.Id),
                values: "Id is invalid");

            invalidPatientException.AddData(
                key: nameof(Patient.NhsNumber),
                values: "Text must be exactly 10 digits.");

            invalidPatientException.AddData(
                key: nameof(Patient.ValidationCode),
                values: "Text must be exactly 5 digits.");

            invalidPatientException.AddData(
                key: nameof(Patient.ValidationCodeExpiresOn),
                values: "Date is required");

            invalidPatientException.AddData(
                key: nameof(Patient.RetryCount),
                values: "Number is invalid");

            var expectedPatientValidationException =
                new PatientValidationException(
                    message: "Patient validation error occurred, please fix errors and try again.",
                    innerException: invalidPatientException);

            // when
            ValueTask<Patient> addPatientTask =
                this.patientService.AddPatientAsync(invalidPatient);

            PatientValidationException actualPatientValidationException =
                await Assert.ThrowsAsync<PatientValidationException>(testCode: addPatientTask.AsTask);

            // then
            actualPatientValidationException.Should()
                .BeEquivalentTo(expectedPatientValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedPatientValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertPatientAsync(It.IsAny<Patient>()),
                    Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfPatientHasInvalidLengthProperty()
        {
            // given
            string invalidNhsNumber = GetRandomStringWithLengthOf(11);
            string invalidValidationCode = GetRandomStringWithLengthOf(6);
            Patient invalidPatient = CreateRandomPatient(invalidNhsNumber, invalidValidationCode);

            var invalidPatientException = new InvalidPatientException(
                message: "Invalid patient. Please correct the errors and try again.");

            invalidPatientException.AddData(
                key: nameof(Patient.NhsNumber),
                values: $"Text must be exactly 10 digits.");

            invalidPatientException.AddData(
                key: nameof(Patient.ValidationCode),
                values: $"Text must be exactly 5 digits.");

            var expectedPatientValidationException =
                new PatientValidationException(
                    message: "Patient validation error occurred, please fix errors and try again.",
                    innerException: invalidPatientException);

            // when
            ValueTask<Patient> addPatientTask =
                this.patientService.AddPatientAsync(invalidPatient);

            PatientValidationException actualPatientValidationException =
                await Assert.ThrowsAsync<PatientValidationException>(
                    testCode: addPatientTask.AsTask);

            // then
            actualPatientValidationException.Should()
                .BeEquivalentTo(expectedPatientValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedPatientValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertPatientAsync(It.IsAny<Patient>()),
                    Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
