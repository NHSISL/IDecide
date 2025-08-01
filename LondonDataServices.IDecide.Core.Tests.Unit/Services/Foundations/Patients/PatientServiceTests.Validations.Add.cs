// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using Moq;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients.Exceptions;
using FluentAssertions;
using LondonDataServices.IDecide.Core.Models.Securities;
using System;

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
        public async Task ShouldThrowValidationExceptionOnAddIfPatientIsInvalidAndLogItAsync(string invalidText)
        {
            // given
            User randomUser = CreateRandomUser();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            DateTimeOffset startDate = randomDateTimeOffset.AddSeconds(-90);
            DateTimeOffset endDate = randomDateTimeOffset.AddSeconds(0);

            var invalidPatient = new Patient
            {
                NhsNumber = invalidText,
                ValidationCode = invalidText,
                Address = invalidText,
                CreatedBy = invalidText,
                Email = invalidText,
                Gender = invalidText,
                GivenName = invalidText,
                Phone = invalidText,
                PostCode = invalidText,
                Surname = invalidText,
                Title = invalidText,
                UpdatedBy = invalidText
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
                key: nameof(Patient.DateOfBirth),
                values: "Date is required");

            invalidPatientException.AddData(
                key: nameof(Patient.RetryCount),
                values: "Number is invalid");

            invalidPatientException.AddData(
                key: nameof(Patient.Address),
                values: "Text is invalid");

            invalidPatientException.AddData(
                key: nameof(Patient.Email),
                values: "Text is invalid");

            invalidPatientException.AddData(
                key: nameof(Patient.Gender),
                values: "Text is invalid");

            invalidPatientException.AddData(
                key: nameof(Patient.GivenName),
                values: "Text is invalid");

            invalidPatientException.AddData(
                key: nameof(Patient.Phone),
                values: "Text is invalid");

            invalidPatientException.AddData(
                key: nameof(Patient.PostCode),
                values: "Text is invalid");

            invalidPatientException.AddData(
                key: nameof(Patient.Surname),
                values: "Text is invalid");

            invalidPatientException.AddData(
                key: nameof(Patient.Title),
                values: "Text is invalid");

            invalidPatientException.AddData(
                key: nameof(Patient.CreatedDate),
                values:
                [
                    "Date is required",
                    $"Date is not recent. Expected a value between " +
                        $"{startDate} and {endDate} but found {invalidPatient.CreatedDate}"
                ]);

            invalidPatientException.AddData(
                key: nameof(Patient.CreatedBy),
                values:
                [
                    "Text is invalid",
                    $"Expected value to be '{randomUser.UserId}' but found " +
                        $"'{invalidPatient.CreatedBy}'."
                ]);

            invalidPatientException.AddData(
                key: nameof(Patient.UpdatedDate),
                values: "Date is required");

            invalidPatientException.AddData(
                key: nameof(Patient.UpdatedBy),
                values: "Text is invalid");

            var expectedPatientValidationException =
                new PatientValidationException(
                    message: "Patient validation error occurred, please fix errors and try again.",
                    innerException: invalidPatientException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomUser);

            // when
            ValueTask<Patient> addPatientTask =
                this.patientService.AddPatientAsync(invalidPatient);

            PatientValidationException actualPatientValidationException =
                await Assert.ThrowsAsync<PatientValidationException>(testCode: addPatientTask.AsTask);

            // then
            actualPatientValidationException.Should()
                .BeEquivalentTo(expectedPatientValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedPatientValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertPatientAsync(It.IsAny<Patient>()),
                    Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfPatientHasInvalidLengthProperty()
        {
            // given
            User randomUser = CreateRandomUser(GetRandomStringWithLengthOf(256));
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            string invalidNhsNumber = GetRandomStringWithLengthOf(11);
            string invalidValidationCode = GetRandomStringWithLengthOf(6);
            string randomInvalidLengthString = GetRandomStringWithLengthOf(256);

            Patient invalidPatient = CreateRandomPatient(
                invalidNhsNumber, 
                invalidValidationCode, 
                randomDateTimeOffset, 
                randomUser.UserId);

            invalidPatient.Address = randomInvalidLengthString;
            invalidPatient.Email = randomInvalidLengthString;
            invalidPatient.Gender = randomInvalidLengthString;
            invalidPatient.GivenName = randomInvalidLengthString;
            invalidPatient.Phone = randomInvalidLengthString;
            invalidPatient.PostCode = randomInvalidLengthString;
            invalidPatient.Surname = randomInvalidLengthString;
            invalidPatient.Title = randomInvalidLengthString;

            var invalidPatientException = new InvalidPatientException(
                message: "Invalid patient. Please correct the errors and try again.");

            invalidPatientException.AddData(
                key: nameof(Patient.NhsNumber),
                values: $"Text must be exactly 10 digits.");

            invalidPatientException.AddData(
                key: nameof(Patient.ValidationCode),
                values: $"Text must be exactly 5 digits.");

            invalidPatientException.AddData(
                key: nameof(Patient.Address),
                values: $"Text exceed max length of {invalidPatient.Address.Length - 1} characters");

            invalidPatientException.AddData(
                key: nameof(Patient.Email),
                values: $"Text exceed max length of {invalidPatient.Email.Length - 1} characters");

            invalidPatientException.AddData(
                key: nameof(Patient.Gender),
                values: $"Text exceed max length of {invalidPatient.Gender.Length - 1} characters");

            invalidPatientException.AddData(
                key: nameof(Patient.GivenName),
                values: $"Text exceed max length of {invalidPatient.GivenName.Length - 1} characters");

            invalidPatientException.AddData(
                key: nameof(Patient.Phone),
                values: $"Text exceed max length of {invalidPatient.Phone.Length - 1} characters");

            invalidPatientException.AddData(
                key: nameof(Patient.PostCode),
                values: $"Text exceed max length of {invalidPatient.PostCode.Length - 1} characters");

            invalidPatientException.AddData(
                key: nameof(Patient.Surname),
                values: $"Text exceed max length of {invalidPatient.Surname.Length - 1} characters");

            invalidPatientException.AddData(
                key: nameof(Patient.Title),
                values: $"Text exceed max length of {invalidPatient.Title.Length - 1} characters");

            var expectedPatientValidationException =
                new PatientValidationException(
                    message: "Patient validation error occurred, please fix errors and try again.",
                    innerException: invalidPatientException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomUser);

            // when
            ValueTask<Patient> addPatientTask =
                this.patientService.AddPatientAsync(invalidPatient);

            PatientValidationException actualPatientValidationException =
                await Assert.ThrowsAsync<PatientValidationException>(
                    testCode: addPatientTask.AsTask);

            // then
            actualPatientValidationException.Should()
                .BeEquivalentTo(expectedPatientValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedPatientValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertPatientAsync(It.IsAny<Patient>()),
                    Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfCreateAndUpdateDatesIsNotSameAndLogItAsync()
        {
            // given
            int randomNumber = GetRandomNumber();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            User randomUser = CreateRandomUser();
            string randomNhsNumber = GenerateRandom10DigitNumber();
            string randomValidationCode = GenerateRandom5DigitNumber();

            Patient randomPatient = CreateRandomPatient(
                nhsNumber: randomNhsNumber,
                validationCode: randomValidationCode,
                dateTimeOffset: randomDateTimeOffset,
                userId: randomUser.UserId);

            Patient invalidPatient = randomPatient;

            invalidPatient.UpdatedDate =
                invalidPatient.CreatedDate.AddDays(randomNumber);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomUser);

            var invalidPatientException =
                new InvalidPatientException(
                    message: "Invalid patient. Please correct the errors and try again.");

            invalidPatientException.AddData(
                key: nameof(Patient.UpdatedDate),
                values: $"Date is not the same as {nameof(Patient.CreatedDate)}");

            var expectedPatientValidationException =
                new PatientValidationException(
                    message: "Patient validation error occurred, please fix errors and try again.",
                    innerException: invalidPatientException);

            // when
            ValueTask<Patient> addPatientTask =
                this.patientService.AddPatientAsync(invalidPatient);

            PatientValidationException actualPatientValidationException =
                await Assert.ThrowsAsync<PatientValidationException>(() =>
                    addPatientTask.AsTask());

            // then
            actualPatientValidationException.Should()
                .BeEquivalentTo(expectedPatientValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once());

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedPatientValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertPatientAsync(It.IsAny<Patient>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfCreateAndUpdateUsersIsNotSameAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            User randomUser = CreateRandomUser();
            string randomNhsNumber = GenerateRandom10DigitNumber();
            string randomValidationCode = GenerateRandom5DigitNumber();

           Patient randomPatient = CreateRandomPatient(
                nhsNumber: randomNhsNumber,
                validationCode: randomValidationCode,
                dateTimeOffset: randomDateTimeOffset,
                userId: randomUser.UserId);

            Patient invalidPatient = randomPatient;
            invalidPatient.UpdatedBy = Guid.NewGuid().ToString();

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomUser);

            var invalidPatientException =
                new InvalidPatientException(
                    message: "Invalid patient. Please correct the errors and try again.");

            invalidPatientException.AddData(
                key: nameof(Patient.UpdatedBy),
                values: $"Text is not the same as {nameof(Patient.CreatedBy)}");

            var expectedPatientValidationException =
                new PatientValidationException(
                    message: "Patient validation error occurred, please fix errors and try again.",
                    innerException: invalidPatientException);

            // when
            ValueTask<Patient> addPatientTask =
                this.patientService.AddPatientAsync(invalidPatient);

            PatientValidationException actualPatientValidationException =
                await Assert.ThrowsAsync<PatientValidationException>(() =>
                    addPatientTask.AsTask());

            // then
            actualPatientValidationException.Should()
                .BeEquivalentTo(expectedPatientValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once());

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedPatientValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertPatientAsync(It.IsAny<Patient>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(-91)]
        public async Task ShouldThrowValidationExceptionOnAddIfCreatedDateIsNotRecentAndLogItAsync(
            int invalidSeconds)
        {
            // given
            DateTimeOffset randomDateTime =
                GetRandomDateTimeOffset();

            DateTimeOffset now = randomDateTime;
            DateTimeOffset startDate = now.AddSeconds(-90);
            DateTimeOffset endDate = now.AddSeconds(0);
            User randomUser = CreateRandomUser();
            string randomNhsNumber = GenerateRandom10DigitNumber();
            string randomValidationCode = GenerateRandom5DigitNumber();

            Patient randomPatient = CreateRandomPatient(
                nhsNumber: randomNhsNumber, 
                validationCode: randomValidationCode,
                dateTimeOffset: randomDateTime, 
                userId: randomUser.UserId);

            Patient invalidPatient = randomPatient;

            DateTimeOffset invalidDate =
                now.AddSeconds(invalidSeconds);

            invalidPatient.CreatedDate = invalidDate;
            invalidPatient.UpdatedDate = invalidDate;

            var invalidPatientException = new InvalidPatientException(
                message: "Invalid patient. Please correct the errors and try again.");

            invalidPatientException.AddData(
            key: nameof(Patient.CreatedDate),
                values:
                    $"Date is not recent. Expected a value between " +
                    $"{startDate} and {endDate} but found {invalidDate}");

            var expectedPatientValidationException =
                new PatientValidationException(
                    message: "Patient validation error occurred, please fix errors and try again.",
                    innerException: invalidPatientException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(now);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomUser);

            // when
            ValueTask<Patient> addPatientTask =
                this.patientService.AddPatientAsync(invalidPatient);

            PatientValidationException actualPatientValidationException =
                await Assert.ThrowsAsync<PatientValidationException>(
                    testCode: addPatientTask.AsTask);

            // then
            actualPatientValidationException.Should().BeEquivalentTo(
                expectedPatientValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedPatientValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertPatientAsync(It.IsAny<Patient>()),
                    Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
