// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients.Exceptions;
using LondonDataServices.IDecide.Core.Models.Securities;
using Moq;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.Patients
{
    public partial class PatientServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfPatientIsNullAndLogItAsync()
        {
            // given
            Patient nullPatient = null;

            var nullPatientException =
                new NullPatientException(message: "Patient is null.");

            var expectedPatientValidationException =
                new PatientValidationException(
                    message: "Patient validation errors occurred, please try again.",
                    innerException: nullPatientException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyAddAuditValuesAsync(nullPatient))
                    .ReturnsAsync(nullPatient);

            // when
            ValueTask<Patient> addPatientTask =
                this.patientService.AddPatientAsync(nullPatient);

            PatientValidationException actualPatientValidationException =
                await Assert.ThrowsAsync<PatientValidationException>(() =>
                    addPatientTask.AsTask());

            // then
            actualPatientValidationException.Should()
                .BeEquivalentTo(expectedPatientValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyAddAuditValuesAsync(nullPatient),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedPatientValidationException))),
                        Times.Once);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnAddIfPatientIsInvalidAndLogItAsync(string invalidText)
        {
            // given
            string randomUserId = GetRandomString();
            User randomUser = CreateRandomUser(userId: randomUserId);

            var invalidPatient = new Patient
            {
                NhsNumber = invalidText,
                GivenName = invalidText,
                Surname = invalidText,
                Gender = invalidText,
                ValidationCode = invalidText,
            };

            var invalidPatientException =
                new InvalidPatientException(
                    message: "Invalid patient. Please correct the errors and try again.");

            invalidPatientException.AddData(
                key: nameof(Patient.Id),
                values: "Id is required");

            invalidPatientException.AddData(
                key: nameof(Patient.NhsNumber),
                values: "Text is required");

            invalidPatientException.AddData(
                key: nameof(Patient.GivenName),
                values: "Text is required");

            invalidPatientException.AddData(
                key: nameof(Patient.Surname),
                values: "Text is required");

            invalidPatientException.AddData(
                key: nameof(Patient.Gender),
                values: "Text is required");

            invalidPatientException.AddData(
                key: nameof(Patient.ValidationCode),
                values: "Text is required");

            invalidPatientException.AddData(
                key: nameof(Patient.DateOfBirth),
                values: "Date is required");

            invalidPatientException.AddData(
                key: nameof(Patient.ValidationCodeExpiresOn),
                values: "Date is required");

            invalidPatientException.AddData(
                key: nameof(Patient.CreatedDate),
                values: "Date is required");

            invalidPatientException.AddData(
                key: nameof(Patient.CreatedBy),
                values:
                    [
                        "Text is required",
                        $"Expected value to be '{randomUserId}' but found '{invalidPatient.CreatedBy}'."
                    ]);

            invalidPatientException.AddData(
                key: nameof(Patient.UpdatedDate),
                values: "Date is required");

            invalidPatientException.AddData(
                key: nameof(Patient.UpdatedBy),
                values: "Text is required");

            var expectedPatientValidationException =
                new PatientValidationException(
                    message: "Patient validation errors occurred, please try again.",
                    innerException: invalidPatientException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyAddAuditValuesAsync(invalidPatient))
                    .ReturnsAsync(invalidPatient);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.GetCurrentUserIdAsync())
                    .ReturnsAsync(randomUserId);

            // when
            ValueTask<Patient> addPatientTask =
                this.patientService.AddPatientAsync(invalidPatient);

            PatientValidationException actualPatientValidationException =
                await Assert.ThrowsAsync<PatientValidationException>(() =>
                    addPatientTask.AsTask());

            // then
            actualPatientValidationException.Should()
                .BeEquivalentTo(expectedPatientValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyAddAuditValuesAsync(invalidPatient),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once());

            this.securityAuditBrokerMock.Verify(broker =>
                broker.GetCurrentUserIdAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedPatientValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertPatientAsync(It.IsAny<Patient>()),
                    Times.Never);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfPatientHasInvalidLengthProperty()
        {
            // given
            string randomUserId = GetRandomString();
            User randomUser = CreateRandomUser(userId: randomUserId);

            var invalidPatient = CreateRandomPatient(GetRandomDateTimeOffset(), userId: randomUserId);
            invalidPatient.NhsNumber = GetRandomStringWithLengthOf(11);
            invalidPatient.Title = GetRandomStringWithLengthOf(36);
            invalidPatient.GivenName = GetRandomStringWithLengthOf(256);
            invalidPatient.Surname = GetRandomStringWithLengthOf(256);
            invalidPatient.Email = GetRandomStringWithLengthOf(256);
            invalidPatient.Phone = GetRandomStringWithLengthOf(16);
            invalidPatient.PostCode = GetRandomStringWithLengthOf(9);
            invalidPatient.ValidationCode = GetRandomStringWithLengthOf(6);

            var invalidPatientException =
                new InvalidPatientException(
                    message: "Invalid patient. Please correct the errors and try again.");

            invalidPatientException.AddData(
                key: nameof(Patient.NhsNumber),
                values: $"Text exceed max length of {invalidPatient.NhsNumber.Length - 1} characters");

            invalidPatientException.AddData(
                key: nameof(Patient.Title),
                values: $"Text exceed max length of {invalidPatient.Title.Length - 1} characters");

            invalidPatientException.AddData(
                key: nameof(Patient.GivenName),
                values: $"Text exceed max length of {invalidPatient.GivenName.Length - 1} characters");

            invalidPatientException.AddData(
                key: nameof(Patient.Surname),
                values: $"Text exceed max length of {invalidPatient.Surname.Length - 1} characters");

            invalidPatientException.AddData(
                key: nameof(Patient.Email),
                values: $"Text exceed max length of {invalidPatient.Email.Length - 1} characters");

            invalidPatientException.AddData(
                key: nameof(Patient.Phone),
                values: $"Text exceed max length of {invalidPatient.Phone.Length - 1} characters");

            invalidPatientException.AddData(
                key: nameof(Patient.PostCode),
                values: $"Text exceed max length of {invalidPatient.PostCode.Length - 1} characters");

            invalidPatientException.AddData(
                key: nameof(Patient.ValidationCode),
                values: $"Text exceed max length of {invalidPatient.ValidationCode.Length - 1} characters");

            var expectedPatientValidationException =
                new PatientValidationException(
                    message: "Patient validation errors occurred, please try again.",
                    innerException: invalidPatientException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyAddAuditValuesAsync(invalidPatient))
                    .ReturnsAsync(invalidPatient);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.GetCurrentUserIdAsync())
                    .ReturnsAsync(randomUserId);

            // when
            ValueTask<Patient> addPatientTask =
                this.patientService.AddPatientAsync(invalidPatient);

            PatientValidationException actualPatientValidationException =
                await Assert.ThrowsAsync<PatientValidationException>(
                    addPatientTask.AsTask);

            // then
            actualPatientValidationException.Should()
                .BeEquivalentTo(expectedPatientValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyAddAuditValuesAsync(invalidPatient),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once());

            this.securityAuditBrokerMock.Verify(broker =>
               broker.GetCurrentUserIdAsync(),
                   Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedPatientValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertPatientAsync(It.IsAny<Patient>()),
                    Times.Never);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfCreateAndUpdateDatesIsNotSameAndLogItAsync()
        {
            // given
            int randomNumber = GetRandomNumber();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            string randomUserId = GetRandomString();
            User randomUser = CreateRandomUser(userId: randomUserId);
            Patient randomPatient = CreateRandomPatient(randomDateTimeOffset, userId: randomUserId);
            Patient invalidPatient = randomPatient;

            invalidPatient.UpdatedDate =
                invalidPatient.CreatedDate.AddDays(randomNumber);

            var invalidPatientException =
                new InvalidPatientException(
                    message: "Invalid patient. Please correct the errors and try again.");

            invalidPatientException.AddData(
                key: nameof(Patient.UpdatedDate),
                values: $"Date is not the same as {nameof(Patient.CreatedDate)}");

            var expectedPatientValidationException =
                new PatientValidationException(
                    message: "Patient validation errors occurred, please try again.",
                    innerException: invalidPatientException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyAddAuditValuesAsync(invalidPatient))
                    .ReturnsAsync(invalidPatient);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.GetCurrentUserIdAsync())
                    .ReturnsAsync(randomUserId);

            // when
            ValueTask<Patient> addPatientTask =
                this.patientService.AddPatientAsync(invalidPatient);

            PatientValidationException actualPatientValidationException =
                await Assert.ThrowsAsync<PatientValidationException>(() =>
                    addPatientTask.AsTask());

            // then
            actualPatientValidationException.Should()
                .BeEquivalentTo(expectedPatientValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyAddAuditValuesAsync(invalidPatient),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once());

            this.securityAuditBrokerMock.Verify(broker =>
                broker.GetCurrentUserIdAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedPatientValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertPatientAsync(It.IsAny<Patient>()),
                    Times.Never);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfCreateAndUpdateUsersIsNotSameAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            string randomUserId = GetRandomString();
            User randomUser = CreateRandomUser(userId: randomUserId);
            Patient randomPatient = CreateRandomPatient(randomDateTimeOffset, userId: randomUserId);
            Patient invalidPatient = randomPatient.DeepClone();
            invalidPatient.UpdatedBy = Guid.NewGuid().ToString();

            var invalidPatientException =
                new InvalidPatientException(
                    message: "Invalid patient. Please correct the errors and try again.");

            invalidPatientException.AddData(
                key: nameof(Patient.UpdatedBy),
                values: $"Text is not the same as {nameof(Patient.CreatedBy)}");

            var expectedPatientValidationException =
                new PatientValidationException(
                    message: "Patient validation errors occurred, please try again.",
                    innerException: invalidPatientException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyAddAuditValuesAsync(invalidPatient))
                    .ReturnsAsync(invalidPatient);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.GetCurrentUserIdAsync())
                    .ReturnsAsync(randomUserId);

            // when
            ValueTask<Patient> addPatientTask =
                this.patientService.AddPatientAsync(invalidPatient);

            PatientValidationException actualPatientValidationException =
                await Assert.ThrowsAsync<PatientValidationException>(() =>
                    addPatientTask.AsTask());

            // then
            actualPatientValidationException.Should()
                .BeEquivalentTo(expectedPatientValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyAddAuditValuesAsync(invalidPatient),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once());

            this.securityAuditBrokerMock.Verify(broker =>
               broker.GetCurrentUserIdAsync(),
                   Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedPatientValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertPatientAsync(It.IsAny<Patient>()),
                    Times.Never);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(MinutesBeforeOrAfter))]
        public async Task ShouldThrowValidationExceptionOnAddIfCreatedDateIsNotRecentAndLogItAsync(
            int minutesBeforeOrAfter)
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();

            DateTimeOffset invalidDateTime =
                randomDateTimeOffset.AddMinutes(minutesBeforeOrAfter);

            DateTimeOffset invalidDate = randomDateTimeOffset.AddMinutes(minutesBeforeOrAfter);
            DateTimeOffset startDate = randomDateTimeOffset.AddSeconds(-90);
            DateTimeOffset endDate = randomDateTimeOffset.AddSeconds(0);
            string randomUserId = GetRandomString();
            User randomUser = CreateRandomUser(userId: randomUserId);
            Patient randomPatient = CreateRandomPatient(invalidDateTime, userId: randomUserId);
            Patient invalidPatient = randomPatient;

            var invalidPatientException =
                new InvalidPatientException(
                    message: "Invalid patient. Please correct the errors and try again.");

            invalidPatientException.AddData(
                key: nameof(Patient.CreatedDate),
                values:
                    $"Date is not recent. Expected a value between {startDate} and {endDate} but found {invalidDate}");

            var expectedPatientValidationException =
                new PatientValidationException(
                    message: "Patient validation errors occurred, please try again.",
                    innerException: invalidPatientException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyAddAuditValuesAsync(invalidPatient))
                    .ReturnsAsync(invalidPatient);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.GetCurrentUserIdAsync())
                    .ReturnsAsync(randomUserId);

            // when
            ValueTask<Patient> addPatientTask =
                this.patientService.AddPatientAsync(invalidPatient);

            PatientValidationException actualPatientValidationException =
                await Assert.ThrowsAsync<PatientValidationException>(() =>
                    addPatientTask.AsTask());

            // then
            actualPatientValidationException.Should()
                .BeEquivalentTo(expectedPatientValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyAddAuditValuesAsync(invalidPatient),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once());

            this.securityAuditBrokerMock.Verify(broker =>
                broker.GetCurrentUserIdAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedPatientValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertPatientAsync(It.IsAny<Patient>()),
                    Times.Never);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}