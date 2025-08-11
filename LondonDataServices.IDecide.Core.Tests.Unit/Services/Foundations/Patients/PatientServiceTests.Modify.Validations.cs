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
        public async Task ShouldThrowValidationExceptionOnModifyIfPatientIsNullAndLogItAsync()
        {
            // given
            Patient nullPatient = null;
            var nullPatientException = new NullPatientException(message: "Patient is null.");

            var expectedPatientValidationException =
                new PatientValidationException(
                    message: "Patient validation errors occurred, please try again.",
                    innerException: nullPatientException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyModifyAuditValuesAsync(nullPatient))
                    .ReturnsAsync(nullPatient);

            // when
            ValueTask<Patient> modifyPatientTask =
                this.patientService.ModifyPatientAsync(nullPatient);

            PatientValidationException actualPatientValidationException =
                await Assert.ThrowsAsync<PatientValidationException>(
                    modifyPatientTask.AsTask);

            // then
            actualPatientValidationException.Should()
                .BeEquivalentTo(expectedPatientValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyModifyAuditValuesAsync(nullPatient),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedPatientValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdatePatientAsync(It.IsAny<Patient>()),
                    Times.Never);

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
        public async Task ShouldThrowValidationExceptionOnModifyIfPatientIsInvalidAndLogItAsync(string invalidText)
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
                values: "Text is required");

            invalidPatientException.AddData(
                key: nameof(Patient.UpdatedDate),
                values:
                new[] {
                    "Date is required",
                    $"Date is the same as {nameof(Patient.CreatedDate)}"
                });

            invalidPatientException.AddData(
                key: nameof(Patient.UpdatedBy),
                values:
                    [
                        "Text is required",
                        $"Expected value to be '{randomUserId}' but found '{invalidPatient.UpdatedBy}'."
                    ]);

            var expectedPatientValidationException =
                new PatientValidationException(
                    message: "Patient validation errors occurred, please try again.",
                    innerException: invalidPatientException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyModifyAuditValuesAsync(invalidPatient))
                    .ReturnsAsync(invalidPatient);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomUser);

            // when
            ValueTask<Patient> modifyPatientTask =
                this.patientService.ModifyPatientAsync(invalidPatient);

            PatientValidationException actualPatientValidationException =
                await Assert.ThrowsAsync<PatientValidationException>(
                    modifyPatientTask.AsTask);

            //then
            actualPatientValidationException.Should()
                .BeEquivalentTo(expectedPatientValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyModifyAuditValuesAsync(invalidPatient),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedPatientValidationException))),
                        Times.Once());

            this.storageBrokerMock.Verify(broker =>
                broker.UpdatePatientAsync(It.IsAny<Patient>()),
                    Times.Never);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }


        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfPatientHasInvalidLengthProperty()
        {
            // given
            string randomUserId = GetRandomString();
            User randomUser = CreateRandomUser(userId: randomUserId);

            var invalidPatient = CreateRandomModifyPatient(GetRandomDateTimeOffset(), userId: randomUserId);
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
                broker.ApplyModifyAuditValuesAsync(invalidPatient))
                    .ReturnsAsync(invalidPatient);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomUser);

            // when
            ValueTask<Patient> modifyPatientTask =
                this.patientService.ModifyPatientAsync(invalidPatient);

            PatientValidationException actualPatientValidationException =
                await Assert.ThrowsAsync<PatientValidationException>(
                    modifyPatientTask.AsTask);

            // then
            actualPatientValidationException.Should()
                .BeEquivalentTo(expectedPatientValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyModifyAuditValuesAsync(invalidPatient),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once());

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

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdatedDateIsSameAsCreatedDateAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            string randomUserId = GetRandomString();
            User randomUser = CreateRandomUser(userId: randomUserId);

            Patient randomPatient =
                CreateRandomPatient(dateTimeOffset: randomDateTimeOffset, userId: randomUserId);

            Patient invalidPatient = randomPatient;

            var invalidPatientException =
                new InvalidPatientException(
                    message: "Invalid patient. Please correct the errors and try again.");

            invalidPatientException.AddData(
                key: nameof(Patient.UpdatedDate),
                values: $"Date is the same as {nameof(Patient.CreatedDate)}");

            var expectedPatientValidationException =
                new PatientValidationException(
                    message: "Patient validation errors occurred, please try again.",
                    innerException: invalidPatientException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyModifyAuditValuesAsync(invalidPatient))
                    .ReturnsAsync(invalidPatient);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomUser);

            // when
            ValueTask<Patient> modifyPatientTask =
                this.patientService.ModifyPatientAsync(invalidPatient);

            PatientValidationException actualPatientValidationException =
                await Assert.ThrowsAsync<PatientValidationException>(
                    modifyPatientTask.AsTask);

            // then
            actualPatientValidationException.Should()
                .BeEquivalentTo(expectedPatientValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyModifyAuditValuesAsync(invalidPatient),
                    Times.Once);

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
                broker.SelectPatientByIdAsync(invalidPatient.Id),
                    Times.Never);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(MinutesBeforeOrAfter))]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdatedDateIsNotRecentAndLogItAsync(int minutes)
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            DateTimeOffset invalidDate = randomDateTimeOffset.AddMinutes(minutes);
            DateTimeOffset startDate = randomDateTimeOffset.AddSeconds(-90);
            DateTimeOffset endDate = randomDateTimeOffset.AddSeconds(0);
            string randomUserId = GetRandomString();
            User randomUser = CreateRandomUser(userId: randomUserId);

            Patient randomPatient =
                CreateRandomPatient(dateTimeOffset: randomDateTimeOffset, userId: randomUserId);

            randomPatient.UpdatedDate = randomDateTimeOffset.AddMinutes(minutes);

            var invalidPatientException =
                new InvalidPatientException(
                    message: "Invalid patient. Please correct the errors and try again.");

            invalidPatientException.AddData(
                key: nameof(Patient.UpdatedDate),
                values:
                    $"Date is not recent. Expected a value between {startDate} and {endDate} but found {invalidDate}");

            var expectedPatientValidatonException =
                new PatientValidationException(
                    message: "Patient validation errors occurred, please try again.",
                    innerException: invalidPatientException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyModifyAuditValuesAsync(randomPatient))
                    .ReturnsAsync(randomPatient);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomUser);

            // when
            ValueTask<Patient> modifyPatientTask =
                this.patientService.ModifyPatientAsync(randomPatient);

            PatientValidationException actualPatientValidationException =
                await Assert.ThrowsAsync<PatientValidationException>(
                    modifyPatientTask.AsTask);

            // then
            actualPatientValidationException.Should()
                .BeEquivalentTo(expectedPatientValidatonException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyModifyAuditValuesAsync(randomPatient),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedPatientValidatonException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectPatientByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfPatientDoesNotExistAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            string randomUserId = GetRandomString();
            User randomUser = CreateRandomUser(userId: randomUserId);

            Patient randomPatient = CreateRandomModifyPatient(
                dateTimeOffset: randomDateTimeOffset, userId: randomUserId);

            Patient nonExistPatient = randomPatient;
            Patient nullPatient = null;

            var notFoundPatientException = new NotFoundPatientException(
                message: $"Couldn't find decision type with patientId: {nonExistPatient.Id}.");

            var expectedPatientValidationException =
                new PatientValidationException(
                    message: "Patient validation errors occurred, please try again.",
                    innerException: notFoundPatientException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyModifyAuditValuesAsync(nonExistPatient))
                    .ReturnsAsync(nonExistPatient);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectPatientByIdAsync(nonExistPatient.Id))
                    .ReturnsAsync(nullPatient);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomUser);

            // when 
            ValueTask<Patient> modifyPatientTask =
                this.patientService.ModifyPatientAsync(nonExistPatient);

            PatientValidationException actualPatientValidationException =
                await Assert.ThrowsAsync<PatientValidationException>(
                    modifyPatientTask.AsTask);

            // then
            actualPatientValidationException.Should()
                .BeEquivalentTo(expectedPatientValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyModifyAuditValuesAsync(nonExistPatient),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectPatientByIdAsync(nonExistPatient.Id),
                    Times.Once);

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

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfStorageCreatedDateNotSameAsCreatedDateAndLogItAsync()
        {
            // given
            int randomNumber = GetRandomNegativeNumber();
            int randomMinutes = randomNumber;
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            string randomUserId = GetRandomString();
            User randomUser = CreateRandomUser(userId: randomUserId);

            Patient randomPatient = CreateRandomModifyPatient(
                dateTimeOffset: randomDateTimeOffset, userId: randomUserId);

            Patient invalidPatient = randomPatient.DeepClone();
            Patient storagePatient = invalidPatient.DeepClone();
            storagePatient.CreatedDate = storagePatient.CreatedDate.AddMinutes(randomMinutes);
            storagePatient.UpdatedDate = storagePatient.UpdatedDate.AddMinutes(randomMinutes);

            var invalidPatientException =
                new InvalidPatientException(
                    message: "Invalid patient. Please correct the errors and try again.");

            invalidPatientException.AddData(
                key: nameof(Patient.CreatedDate),
                values: $"Date is not the same as {nameof(Patient.CreatedDate)}");

            var expectedPatientValidationException =
                new PatientValidationException(
                    message: "Patient validation errors occurred, please try again.",
                    innerException: invalidPatientException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyModifyAuditValuesAsync(invalidPatient))
                    .ReturnsAsync(invalidPatient);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectPatientByIdAsync(invalidPatient.Id))
                    .ReturnsAsync(storagePatient);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomUser);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.EnsureAddAuditValuesRemainsUnchangedOnModifyAsync(invalidPatient, storagePatient))
                    .ReturnsAsync(invalidPatient);

            // when
            ValueTask<Patient> modifyPatientTask =
                this.patientService.ModifyPatientAsync(invalidPatient);

            PatientValidationException actualPatientValidationException =
                await Assert.ThrowsAsync<PatientValidationException>(
                    modifyPatientTask.AsTask);

            // then
            actualPatientValidationException.Should()
                .BeEquivalentTo(expectedPatientValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyModifyAuditValuesAsync(invalidPatient),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectPatientByIdAsync(invalidPatient.Id),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.EnsureAddAuditValuesRemainsUnchangedOnModifyAsync(invalidPatient, storagePatient),
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

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfCreatedUserDontMacthStorageAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            string randomUserId = GetRandomString();
            User randomUser = CreateRandomUser(userId: randomUserId);

            Patient randomPatient =
                CreateRandomModifyPatient(dateTimeOffset: randomDateTimeOffset, userId: randomUserId);

            Patient invalidPatient = randomPatient.DeepClone();
            Patient storagePatient = invalidPatient.DeepClone();
            invalidPatient.CreatedBy = Guid.NewGuid().ToString();
            storagePatient.UpdatedDate = storagePatient.CreatedDate;

            var invalidPatientException =
                new InvalidPatientException(
                    message: "Invalid patient. Please correct the errors and try again.");

            invalidPatientException.AddData(
                key: nameof(Patient.CreatedBy),
                values: $"Text is not the same as {nameof(Patient.CreatedBy)}");

            var expectedPatientValidationException =
                new PatientValidationException(
                    message: "Patient validation errors occurred, please try again.",
                    innerException: invalidPatientException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyModifyAuditValuesAsync(invalidPatient))
                    .ReturnsAsync(invalidPatient);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectPatientByIdAsync(invalidPatient.Id))
                    .ReturnsAsync(storagePatient);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomUser);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.EnsureAddAuditValuesRemainsUnchangedOnModifyAsync(invalidPatient, storagePatient))
                    .ReturnsAsync(invalidPatient);

            // when
            ValueTask<Patient> modifyPatientTask =
                this.patientService.ModifyPatientAsync(invalidPatient);

            PatientValidationException actualPatientValidationException =
                await Assert.ThrowsAsync<PatientValidationException>(
                    modifyPatientTask.AsTask);

            // then
            actualPatientValidationException.Should().BeEquivalentTo(expectedPatientValidationException);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyModifyAuditValuesAsync(invalidPatient),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectPatientByIdAsync(invalidPatient.Id),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.EnsureAddAuditValuesRemainsUnchangedOnModifyAsync(invalidPatient, storagePatient),
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

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfStorageUpdatedDateSameAsUpdatedDateAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            string randomUserId = GetRandomString();
            User randomUser = CreateRandomUser(userId: randomUserId);

            Patient randomPatient =
                CreateRandomModifyPatient(dateTimeOffset: randomDateTimeOffset, userId: randomUserId);

            Patient invalidPatient = randomPatient;
            Patient storagePatient = randomPatient.DeepClone();

            var invalidPatientException =
                new InvalidPatientException(
                    message: "Invalid patient. Please correct the errors and try again.");

            invalidPatientException.AddData(
                key: nameof(Patient.UpdatedDate),
                values: $"Date is the same as {nameof(Patient.UpdatedDate)}");

            var expectedPatientValidationException =
                new PatientValidationException(
                    message: "Patient validation errors occurred, please try again.",
                    innerException: invalidPatientException);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyModifyAuditValuesAsync(invalidPatient))
                    .ReturnsAsync(invalidPatient);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectPatientByIdAsync(invalidPatient.Id))
                    .ReturnsAsync(storagePatient);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomUser);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.EnsureAddAuditValuesRemainsUnchangedOnModifyAsync(invalidPatient, storagePatient))
                    .ReturnsAsync(invalidPatient);

            // when
            ValueTask<Patient> modifyPatientTask =
                this.patientService.ModifyPatientAsync(invalidPatient);

            // then
            await Assert.ThrowsAsync<PatientValidationException>(
                modifyPatientTask.AsTask);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyModifyAuditValuesAsync(invalidPatient),
                    Times.Once);

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
                broker.SelectPatientByIdAsync(invalidPatient.Id),
                    Times.Once);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.EnsureAddAuditValuesRemainsUnchangedOnModifyAsync(invalidPatient, storagePatient),
                    Times.Once);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}