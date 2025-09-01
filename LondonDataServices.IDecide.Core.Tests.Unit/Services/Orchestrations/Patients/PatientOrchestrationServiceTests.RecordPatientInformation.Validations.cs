// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using LondonDataServices.IDecide.Core.Models.Foundations.Notifications;
using LondonDataServices.IDecide.Core.Models.Orchestrations.Patients.Exceptions;
using LondonDataServices.IDecide.Core.Services.Orchestrations.Patients;
using Moq;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Orchestrations.Patients
{
    public partial class PatientOrchestrationServiceTests
    {
        [Theory]
        [InlineData("123456789")]
        [InlineData("01234567890")]
        public async Task ShouldThrowValidationExceptionOnRecordPatientInformationAsyncWithInvalidNhsNumber(
            string invalidNhsNumber)
        {
            // given
            string randomCaptchaToken = GetRandomString();
            string inputCaptchaToken = randomCaptchaToken.DeepClone();
            NotificationPreference randomNotificationPreference = NotificationPreference.Email;
            NotificationPreference inputNotificationPreference = randomNotificationPreference.DeepClone();
            string notificationPreferenceString = inputNotificationPreference.ToString();

            var invalidPatientOrchestrationArgumentException =
                new InvalidPatientOrchestrationArgumentException(
                    "Invalid patient orchestration argument. Please correct the errors and try again.");

            invalidPatientOrchestrationArgumentException.AddData(
                key: "nhsNumber",
                values: "Text must be exactly 10 digits.");

            var expectedPatientOrchestrationValidationException =
                new PatientOrchestrationValidationException(
                    message: "Patient orchestration validation error occurred, please fix the errors and try again.",
                    innerException: invalidPatientOrchestrationArgumentException);

            var patientOrchestrationServiceMock = new Mock<PatientOrchestrationService>(
               this.loggingBrokerMock.Object,
               this.securityBrokerMock.Object,
               this.dateTimeBrokerMock.Object,
               this.pdsServiceMock.Object,
               this.patientServiceMock.Object,
               this.notificationServiceMock.Object,
               this.decisionConfigurations)
            { CallBase = true };

            patientOrchestrationServiceMock.Setup(broker =>
                broker.CheckIfIsAuthenticatedUserWithRequiredRoleAsync(inputCaptchaToken))
                    .ThrowsAsync(invalidPatientOrchestrationArgumentException);

            // when
            ValueTask recordPatientInformationTask =
                patientOrchestrationServiceMock.Object.RecordPatientInformationAsync(
                    invalidNhsNumber,
                    inputCaptchaToken,
                    notificationPreferenceString,
                    false);

            PatientOrchestrationValidationException actualException =
                await Assert.ThrowsAsync<PatientOrchestrationValidationException>(
                    recordPatientInformationTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedPatientOrchestrationValidationException);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedPatientOrchestrationValidationException))),
                       Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.pdsServiceMock.VerifyNoOtherCalls();
            this.patientServiceMock.VerifyNoOtherCalls();
            this.notificationServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnRecordPatientInformationAsyncWithInvalidCaptchaToken(
            string invalidCaptchaToken)
        {
            // given
            string randomNhsNumber = GenerateRandom10DigitNumber();
            string inputNhsNumber = randomNhsNumber.DeepClone();
            NotificationPreference randomNotificationPreference = NotificationPreference.Email;
            NotificationPreference inputNotificationPreference = randomNotificationPreference.DeepClone();
            string notificationPreferenceString = inputNotificationPreference.ToString();

            var invalidPatientOrchestrationArgumentException =
                new InvalidPatientOrchestrationArgumentException(
                    "Invalid patient orchestration argument. Please correct the errors and try again.");

            invalidPatientOrchestrationArgumentException.AddData(
                key: "captchaToken",
                values: "Text is invalid");

            var expectedPatientOrchestrationValidationException =
                new PatientOrchestrationValidationException(
                    message: "Patient orchestration validation error occurred, please fix the errors and try again.",
                    innerException: invalidPatientOrchestrationArgumentException);

            var patientOrchestrationServiceMock = new Mock<PatientOrchestrationService>(
               this.loggingBrokerMock.Object,
               this.securityBrokerMock.Object,
               this.dateTimeBrokerMock.Object,
               this.pdsServiceMock.Object,
               this.patientServiceMock.Object,
               this.notificationServiceMock.Object,
               this.decisionConfigurations)
            { CallBase = true };

            patientOrchestrationServiceMock.Setup(broker =>
                broker.CheckIfIsAuthenticatedUserWithRequiredRoleAsync(invalidCaptchaToken))
                    .ThrowsAsync(invalidPatientOrchestrationArgumentException);

            // when
            ValueTask recordPatientInformationTask =
                patientOrchestrationServiceMock.Object.RecordPatientInformationAsync(
                    inputNhsNumber,
                    invalidCaptchaToken,
                    notificationPreferenceString,
                    false);

            PatientOrchestrationValidationException actualException =
                await Assert.ThrowsAsync<PatientOrchestrationValidationException>(
                    recordPatientInformationTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedPatientOrchestrationValidationException);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedPatientOrchestrationValidationException))),
                       Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.pdsServiceMock.VerifyNoOtherCalls();
            this.patientServiceMock.VerifyNoOtherCalls();
            this.notificationServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("test")]
        public async Task ShouldThrowValidationExceptionOnRecordPatientInformationAsyncWithInvalidNotificationPreference(
            string invalidNotificationPreference)
        {
            // given
            string randomCaptchaToken = GetRandomString();
            string inputCaptchaToken = randomCaptchaToken.DeepClone();
            string randomNhsNumber = GenerateRandom10DigitNumber();
            string inputNhsNumber = randomNhsNumber.DeepClone();

            var invalidPatientOrchestrationArgumentException =
                new InvalidPatientOrchestrationArgumentException(
                    "Invalid patient orchestration argument. Please correct the errors and try again.");

            invalidPatientOrchestrationArgumentException.AddData(
                key: "notificationPreference",
                values: "Text is not a valid notification preference");

            var expectedPatientOrchestrationValidationException =
                new PatientOrchestrationValidationException(
                    message: "Patient orchestration validation error occurred, please fix the errors and try again.",
                    innerException: invalidPatientOrchestrationArgumentException);

            var patientOrchestrationServiceMock = new Mock<PatientOrchestrationService>(
               this.loggingBrokerMock.Object,
               this.securityBrokerMock.Object,
               this.dateTimeBrokerMock.Object,
               this.pdsServiceMock.Object,
               this.patientServiceMock.Object,
               this.notificationServiceMock.Object,
               this.decisionConfigurations)
            { CallBase = true };

            patientOrchestrationServiceMock.Setup(broker =>
                broker.CheckIfIsAuthenticatedUserWithRequiredRoleAsync(inputCaptchaToken))
                    .ThrowsAsync(invalidPatientOrchestrationArgumentException);

            // when
            ValueTask recordPatientInformationTask =
                patientOrchestrationServiceMock.Object.RecordPatientInformationAsync(
                    inputNhsNumber,
                    inputCaptchaToken,
                    invalidNotificationPreference,
                    false);

            PatientOrchestrationValidationException actualException =
                await Assert.ThrowsAsync<PatientOrchestrationValidationException>(
                    recordPatientInformationTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedPatientOrchestrationValidationException);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedPatientOrchestrationValidationException))),
                       Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.pdsServiceMock.VerifyNoOtherCalls();
            this.patientServiceMock.VerifyNoOtherCalls();
            this.notificationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnRecordPatientInformationAsyncWithRejectedCaptcha()
        {
            // given
            string randomCaptchaToken = GetRandomString();
            string inputCaptchaToken = randomCaptchaToken.DeepClone();
            string randomNhsNumber = GenerateRandom10DigitNumber();
            string inputNhsNumber = randomNhsNumber.DeepClone();
            NotificationPreference randomNotificationPreference = NotificationPreference.Email;
            NotificationPreference inputNotificationPreference = randomNotificationPreference.DeepClone();
            string notificationPreferenceString = inputNotificationPreference.ToString();

            var invalidCaptchaPatientOrchestrationServiceException =
                new InvalidCaptchaPatientOrchestrationServiceException(
                    "The provided captcha token is invalid.");

            var expectedPatientOrchestrationValidationException =
                new PatientOrchestrationValidationException(
                    message: "Patient orchestration validation error occurred, please fix the errors and try again.",
                    innerException: invalidCaptchaPatientOrchestrationServiceException);

            var patientOrchestrationServiceMock = new Mock<PatientOrchestrationService>(
                 this.loggingBrokerMock.Object,
                 this.securityBrokerMock.Object,
                 this.dateTimeBrokerMock.Object,
                 this.pdsServiceMock.Object,
                 this.patientServiceMock.Object,
                 this.notificationServiceMock.Object,
                 this.decisionConfigurations)
            { CallBase = true };

            patientOrchestrationServiceMock.Setup(broker =>
                broker.CheckIfIsAuthenticatedUserWithRequiredRoleAsync(inputCaptchaToken))
                    .ThrowsAsync(invalidCaptchaPatientOrchestrationServiceException);

            // when
            ValueTask recordPatientInformationTask =
                patientOrchestrationServiceMock.Object.RecordPatientInformationAsync(
                    inputNhsNumber,
                    inputCaptchaToken,
                    notificationPreferenceString,
                    false);

            PatientOrchestrationValidationException actualException =
                await Assert.ThrowsAsync<PatientOrchestrationValidationException>(
                    recordPatientInformationTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedPatientOrchestrationValidationException);

            patientOrchestrationServiceMock.Verify(broker =>
                 broker.CheckIfIsAuthenticatedUserWithRequiredRoleAsync(inputCaptchaToken),
                     Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedPatientOrchestrationValidationException))),
                       Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.pdsServiceMock.VerifyNoOtherCalls();
            this.patientServiceMock.VerifyNoOtherCalls();
            this.notificationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnRecordPatientInformationAsyncWithAlreadyValidCode()
        {
            // given
            string randomCaptchaToken = GetRandomString();
            string inputCaptchaToken = randomCaptchaToken.DeepClone();
            string randomNhsNumber = GenerateRandom10DigitNumber();
            string inputNhsNumber = randomNhsNumber.DeepClone();
            NotificationPreference randomNotificationPreference = NotificationPreference.Email;
            NotificationPreference inputNotificationPreference = randomNotificationPreference.DeepClone();
            string notificationPreferenceString = inputNotificationPreference.ToString();

            var validPatientCodeExistsException =
                new ValidPatientCodeExistsException(
                    "A valid code already exists for this patient, please go to the enter code screen.");

            var expectedPatientOrchestrationValidationException =
                new PatientOrchestrationValidationException(
                    message: "Patient orchestration validation error occurred, please fix the errors and try again.",
                    innerException: validPatientCodeExistsException);

            var patientOrchestrationServiceMock = new Mock<PatientOrchestrationService>(
                this.loggingBrokerMock.Object,
                this.securityBrokerMock.Object,
                this.dateTimeBrokerMock.Object,
                this.pdsServiceMock.Object,
                this.patientServiceMock.Object,
                this.notificationServiceMock.Object,
                this.decisionConfigurations)
            { CallBase = true };

            patientOrchestrationServiceMock.Setup(broker =>
                broker.CheckIfIsAuthenticatedUserWithRequiredRoleAsync(inputCaptchaToken))
                    .ThrowsAsync(validPatientCodeExistsException);

            // when
            ValueTask recordPatientInformationTask =
                patientOrchestrationServiceMock.Object.RecordPatientInformationAsync(
                     inputNhsNumber,
                     inputCaptchaToken,
                     notificationPreferenceString,
                     false);

            PatientOrchestrationValidationException actualException =
                await Assert.ThrowsAsync<PatientOrchestrationValidationException>(
                    recordPatientInformationTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedPatientOrchestrationValidationException);

            patientOrchestrationServiceMock.Verify(broker =>
                 broker.CheckIfIsAuthenticatedUserWithRequiredRoleAsync(inputCaptchaToken),
                     Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedPatientOrchestrationValidationException))),
                       Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.pdsServiceMock.VerifyNoOtherCalls();
            this.patientServiceMock.VerifyNoOtherCalls();
            this.notificationServiceMock.VerifyNoOtherCalls();
        }
    }
}
