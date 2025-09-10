// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using LondonDataServices.IDecide.Core.Models.Foundations.Notifications;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using LondonDataServices.IDecide.Core.Models.Orchestrations.Patients.Exceptions;
using LondonDataServices.IDecide.Core.Services.Orchestrations.Patients;
using Moq;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Orchestrations.Patients
{
    public partial class PatientOrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldRecordPatientInformationAsyncWhenNoPatientFoundForAuthenticatedUser()
        {
            // given
            int expireAfterMinutes = this.decisionConfigurations.PatientValidationCodeExpireAfterMinutes;
            string randomNhsNumber = GenerateRandom10DigitNumber();
            string inputNhsNumber = randomNhsNumber.DeepClone();
            string randomCaptchaToken = GetRandomString();
            string inputCaptchaToken = randomCaptchaToken.DeepClone();
            NotificationPreference randomNotificationPreference = NotificationPreference.Email;
            NotificationPreference inputNotificationPreference = randomNotificationPreference.DeepClone();
            string notificationPreferenceString = inputNotificationPreference.ToString();
            string outputValidationCode = GetRandomStringWithLengthOf(5);
            DateTimeOffset randomDateTimeOffest = GetRandomDateTimeOffset();
            DateTimeOffset outputDateTimeOffset = randomDateTimeOffest.DeepClone();
            Patient randomPatient = GetRandomPatientWithNhsNumber(inputNhsNumber);
            randomPatient.NotificationPreference = inputNotificationPreference;
            Patient outputPatient = randomPatient.DeepClone();
            List<Patient> randomPatients = GetRandomPatients();
            List<Patient> outputPatients = randomPatients.DeepClone();
            Patient updatedPatient = outputPatient.DeepClone();
            updatedPatient.ValidationCode = outputValidationCode;
            updatedPatient.ValidationCodeMatchedOn = null;
            updatedPatient.ValidationCodeExpiresOn = outputDateTimeOffset.AddMinutes(expireAfterMinutes);
            updatedPatient.NotificationPreference = inputNotificationPreference;

            NotificationInfo inputNotificationInfo = new NotificationInfo
            {
                Patient = updatedPatient
            };

            var patientOrchestrationServiceMock = new Mock<PatientOrchestrationService>(
                this.loggingBrokerMock.Object,
                this.securityBrokerMock.Object,
                this.dateTimeBrokerMock.Object,
                this.pdsServiceMock.Object,
                this.patientServiceMock.Object,
                this.notificationServiceMock.Object,
                this.identifierBrokerMock.Object,
                this.decisionConfigurations)
            { CallBase = true };

            patientOrchestrationServiceMock.Setup(service =>
                service.CheckIfIsAuthenticatedUserWithRequiredRoleAsync())
                    .ReturnsAsync(true);

            this.patientServiceMock.Setup(service =>
                service.RetrieveAllPatientsAsync())
                    .ReturnsAsync(outputPatients.AsQueryable);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(outputDateTimeOffset);

            patientOrchestrationServiceMock.Setup(service =>
                service.GenerateNewPatientWithCodeAsync(
                    inputNhsNumber,
                    inputNotificationPreference,
                    outputDateTimeOffset))
                        .ReturnsAsync(updatedPatient);

            // when
            await patientOrchestrationServiceMock.Object.RecordPatientInformationAsync(
                inputNhsNumber,
                notificationPreferenceString,
                false);

            //then
            patientOrchestrationServiceMock.Verify(service =>
                service.CheckIfIsAuthenticatedUserWithRequiredRoleAsync(),
                    Times.Once);

            this.patientServiceMock.Verify(service =>
                service.RetrieveAllPatientsAsync(),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            patientOrchestrationServiceMock.Verify(service =>
                service.GenerateNewPatientWithCodeAsync(
                    inputNhsNumber,
                    inputNotificationPreference,
                    outputDateTimeOffset),
                        Times.Once);

            this.notificationServiceMock.Verify(service =>
                service.SendCodeNotificationAsync(It.Is(SameNotificationInfoAs(inputNotificationInfo))),
                        Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.pdsServiceMock.VerifyNoOtherCalls();
            this.patientServiceMock.VerifyNoOtherCalls();
            this.notificationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldRecordPatientInformationAsyncWhenNoPatientFoundForNonAuthenticatedUser()
        {
            // given
            int expireAfterMinutes = this.decisionConfigurations.PatientValidationCodeExpireAfterMinutes;
            string randomNhsNumber = GenerateRandom10DigitNumber();
            string inputNhsNumber = randomNhsNumber.DeepClone();
            string randomCaptchaToken = GetRandomString();
            string inputCaptchaToken = randomCaptchaToken.DeepClone();
            NotificationPreference randomNotificationPreference = NotificationPreference.Email;
            NotificationPreference inputNotificationPreference = randomNotificationPreference.DeepClone();
            string notificationPreferenceString = inputNotificationPreference.ToString();
            string outputValidationCode = GetRandomStringWithLengthOf(5);
            DateTimeOffset randomDateTimeOffest = GetRandomDateTimeOffset();
            DateTimeOffset outputDateTimeOffset = randomDateTimeOffest.DeepClone();
            Patient randomPatient = GetRandomPatientWithNhsNumber(inputNhsNumber);
            randomPatient.NotificationPreference = inputNotificationPreference;
            Patient outputPatient = randomPatient.DeepClone();
            List<Patient> randomPatients = GetRandomPatients();
            List<Patient> outputPatients = randomPatients.DeepClone();
            Patient updatedPatient = outputPatient.DeepClone();
            updatedPatient.ValidationCode = outputValidationCode;
            updatedPatient.ValidationCodeMatchedOn = null;
            updatedPatient.ValidationCodeExpiresOn = outputDateTimeOffset.AddMinutes(expireAfterMinutes);
            updatedPatient.NotificationPreference = inputNotificationPreference;

            NotificationInfo inputNotificationInfo = new NotificationInfo
            {
                Patient = updatedPatient
            };

            var patientOrchestrationServiceMock = new Mock<PatientOrchestrationService>(
                this.loggingBrokerMock.Object,
                this.securityBrokerMock.Object,
                this.dateTimeBrokerMock.Object,
                this.pdsServiceMock.Object,
                this.patientServiceMock.Object,
                this.notificationServiceMock.Object,
                this.identifierBrokerMock.Object,
                this.decisionConfigurations)
            { CallBase = true };

            patientOrchestrationServiceMock.Setup(service =>
                service.CheckIfIsAuthenticatedUserWithRequiredRoleAsync())
                    .ReturnsAsync(false);

            this.patientServiceMock.Setup(service =>
                service.RetrieveAllPatientsAsync())
                    .ReturnsAsync(outputPatients.AsQueryable);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(outputDateTimeOffset);

            patientOrchestrationServiceMock.Setup(service =>
                service.GenerateNewPatientWithCodeAsync(
                    inputNhsNumber,
                    inputNotificationPreference,
                    outputDateTimeOffset))
                        .ReturnsAsync(updatedPatient);

            this.patientServiceMock.Setup(service =>
                service.AddPatientAsync(It.Is(SamePatientAs(updatedPatient))))
                    .ReturnsAsync(updatedPatient);

            // when
            await patientOrchestrationServiceMock.Object.RecordPatientInformationAsync(
                inputNhsNumber,
                notificationPreferenceString,
                false);

            //then
            patientOrchestrationServiceMock.Verify(service =>
                service.CheckIfIsAuthenticatedUserWithRequiredRoleAsync(),
                    Times.Once);

            this.patientServiceMock.Verify(service =>
                service.RetrieveAllPatientsAsync(),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            patientOrchestrationServiceMock.Verify(service =>
                service.GenerateNewPatientWithCodeAsync(
                    inputNhsNumber,
                    inputNotificationPreference,
                    outputDateTimeOffset),
                        Times.Once);

            this.notificationServiceMock.Verify(service =>
                service.SendCodeNotificationAsync(It.Is(SameNotificationInfoAs(inputNotificationInfo))),
                        Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.pdsServiceMock.VerifyNoOtherCalls();
            this.patientServiceMock.VerifyNoOtherCalls();
            this.notificationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task
            ShouldErrorOnRecordPatientInformationAsyncWhenHasActiveCodeAndGenerateCodeIsFalseForAuthenticatedUser()
        {
            // given
            int expireAfterMinutes = this.decisionConfigurations.PatientValidationCodeExpireAfterMinutes;
            string randomNhsNumber = GenerateRandom10DigitNumber();
            string inputNhsNumber = randomNhsNumber.DeepClone();
            string randomCaptchaToken = GetRandomString();
            string inputCaptchaToken = randomCaptchaToken.DeepClone();
            NotificationPreference randomNotificationPreference = NotificationPreference.Email;
            NotificationPreference inputNotificationPreference = randomNotificationPreference.DeepClone();
            string notificationPreferenceString = inputNotificationPreference.ToString();
            string outputValidationCode = GetRandomStringWithLengthOf(5);
            DateTimeOffset randomDateTimeOffest = GetRandomDateTimeOffset();
            DateTimeOffset outputDateTimeOffset = randomDateTimeOffest.DeepClone();
            Patient randomPatient = GetRandomPatientWithNhsNumber(inputNhsNumber);
            randomPatient.NotificationPreference = inputNotificationPreference;
            randomPatient.ValidationCodeExpiresOn = outputDateTimeOffset.AddMinutes(1);
            randomPatient.ValidationCodeMatchedOn = null;
            Patient outputPatient = randomPatient.DeepClone();
            List<Patient> randomPatients = GetRandomPatients();
            randomPatients.Add(outputPatient);
            List<Patient> outputPatients = randomPatients.DeepClone();

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
                this.identifierBrokerMock.Object,
                this.decisionConfigurations)
            { CallBase = true };

            patientOrchestrationServiceMock.Setup(service =>
                service.CheckIfIsAuthenticatedUserWithRequiredRoleAsync())
                    .ReturnsAsync(true);

            this.patientServiceMock.Setup(service =>
                service.RetrieveAllPatientsAsync())
                    .ReturnsAsync(outputPatients.AsQueryable);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(outputDateTimeOffset);

            // when
            ValueTask recordPatientInformationTask = patientOrchestrationServiceMock.Object.RecordPatientInformationAsync(
                inputNhsNumber,
                notificationPreferenceString,
                false);

            PatientOrchestrationValidationException
                actualPatientOrchestrationValidationException =
                    await Assert.ThrowsAsync<PatientOrchestrationValidationException>(
                        testCode: recordPatientInformationTask.AsTask);

            //then
            actualPatientOrchestrationValidationException
                .Should().BeEquivalentTo(expectedPatientOrchestrationValidationException);

            patientOrchestrationServiceMock.Verify(service =>
                service.CheckIfIsAuthenticatedUserWithRequiredRoleAsync(),
                    Times.Once);

            this.patientServiceMock.Verify(service =>
                service.RetrieveAllPatientsAsync(),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
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
            this.identifierBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task
            ShouldErrorOnRecordPatientInformationAsyncWhenHasActiveCodeAndGenerateCodeIsFalseForNonAuthenticatedUser()
        {
            // given
            int expireAfterMinutes = this.decisionConfigurations.PatientValidationCodeExpireAfterMinutes;
            string randomNhsNumber = GenerateRandom10DigitNumber();
            string inputNhsNumber = randomNhsNumber.DeepClone();
            string randomCaptchaToken = GetRandomString();
            string inputCaptchaToken = randomCaptchaToken.DeepClone();
            NotificationPreference randomNotificationPreference = NotificationPreference.Email;
            NotificationPreference inputNotificationPreference = randomNotificationPreference.DeepClone();
            string notificationPreferenceString = inputNotificationPreference.ToString();
            string outputValidationCode = GetRandomStringWithLengthOf(5);
            DateTimeOffset randomDateTimeOffest = GetRandomDateTimeOffset();
            DateTimeOffset outputDateTimeOffset = randomDateTimeOffest.DeepClone();
            Patient randomPatient = GetRandomPatientWithNhsNumber(inputNhsNumber);
            randomPatient.NotificationPreference = inputNotificationPreference;
            randomPatient.ValidationCodeExpiresOn = outputDateTimeOffset.AddMinutes(1);
            randomPatient.ValidationCodeMatchedOn = null;
            Patient outputPatient = randomPatient.DeepClone();
            List<Patient> randomPatients = GetRandomPatients();
            randomPatients.Add(outputPatient);
            List<Patient> outputPatients = randomPatients.DeepClone();

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
                this.identifierBrokerMock.Object,
                this.decisionConfigurations)
            { CallBase = true };

            patientOrchestrationServiceMock.Setup(service =>
                service.CheckIfIsAuthenticatedUserWithRequiredRoleAsync())
                    .ReturnsAsync(false);

            this.patientServiceMock.Setup(service =>
                service.RetrieveAllPatientsAsync())
                    .ReturnsAsync(outputPatients.AsQueryable);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(outputDateTimeOffset);

            // when
            ValueTask recordPatientInformationTask = patientOrchestrationServiceMock.Object.RecordPatientInformationAsync(
                inputNhsNumber,
                notificationPreferenceString,
                false);

            PatientOrchestrationValidationException
                actualPatientOrchestrationValidationException =
                    await Assert.ThrowsAsync<PatientOrchestrationValidationException>(
                        testCode: recordPatientInformationTask.AsTask);

            //then
            actualPatientOrchestrationValidationException
                .Should().BeEquivalentTo(expectedPatientOrchestrationValidationException);

            patientOrchestrationServiceMock.Verify(service =>
                service.CheckIfIsAuthenticatedUserWithRequiredRoleAsync(),
                    Times.Once);

            this.patientServiceMock.Verify(service =>
                service.RetrieveAllPatientsAsync(),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
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
        public async Task ShouldRecordPatientInformationAsyncWhenGenerateNewCodeForAuthenticatedUser()
        {
            // given
            int expireAfterMinutes = this.decisionConfigurations.PatientValidationCodeExpireAfterMinutes;
            string randomNhsNumber = GenerateRandom10DigitNumber();
            string inputNhsNumber = randomNhsNumber.DeepClone();
            string randomCaptchaToken = GetRandomString();
            string inputCaptchaToken = randomCaptchaToken.DeepClone();
            NotificationPreference randomNotificationPreference = NotificationPreference.Email;
            NotificationPreference inputNotificationPreference = randomNotificationPreference.DeepClone();
            string notificationPreferenceString = inputNotificationPreference.ToString();
            string outputValidationCode = GetRandomStringWithLengthOf(5);
            DateTimeOffset randomDateTimeOffest = GetRandomDateTimeOffset();
            DateTimeOffset outputDateTimeOffset = randomDateTimeOffest.DeepClone();
            Patient randomPatient = GetRandomPatientWithNhsNumber(inputNhsNumber);
            randomPatient.NotificationPreference = inputNotificationPreference;
            randomPatient.RetryCount = 0;
            Patient outputPatient = randomPatient.DeepClone();
            List<Patient> randomPatients = GetRandomPatients();
            randomPatients.Add(outputPatient);
            List<Patient> outputPatients = randomPatients.DeepClone();
            Patient updatedPatient = outputPatient.DeepClone();
            updatedPatient.ValidationCode = outputValidationCode;
            updatedPatient.ValidationCodeMatchedOn = null;
            updatedPatient.ValidationCodeExpiresOn = outputDateTimeOffset.AddMinutes(expireAfterMinutes);
            updatedPatient.NotificationPreference = inputNotificationPreference;
            updatedPatient.RetryCount = 0;

            NotificationInfo inputNotificationInfo = new NotificationInfo
            {
                Patient = updatedPatient
            };

            var patientOrchestrationServiceMock = new Mock<PatientOrchestrationService>(
                this.loggingBrokerMock.Object,
                this.securityBrokerMock.Object,
                this.dateTimeBrokerMock.Object,
                this.pdsServiceMock.Object,
                this.patientServiceMock.Object,
                this.notificationServiceMock.Object,
                this.identifierBrokerMock.Object,
                this.decisionConfigurations)
            { CallBase = true };

            patientOrchestrationServiceMock.Setup(service =>
                service.CheckIfIsAuthenticatedUserWithRequiredRoleAsync())
                    .ReturnsAsync(true);

            this.patientServiceMock.Setup(service =>
                service.RetrieveAllPatientsAsync())
                    .ReturnsAsync(outputPatients.AsQueryable);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(outputDateTimeOffset);

            patientOrchestrationServiceMock.Setup(service =>
                service.UpdatePatientWithNewCodeAsync(
                    It.Is(SamePatientAs(outputPatient)),
                    inputNotificationPreference,
                    outputDateTimeOffset,
                    true))
                        .ReturnsAsync(updatedPatient);

            // when
            await patientOrchestrationServiceMock.Object.RecordPatientInformationAsync(
                inputNhsNumber,
                notificationPreferenceString,
                true);

            //then
            patientOrchestrationServiceMock.Verify(service =>
                service.CheckIfIsAuthenticatedUserWithRequiredRoleAsync(),
                    Times.Once);

            this.patientServiceMock.Verify(service =>
                service.RetrieveAllPatientsAsync(),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            patientOrchestrationServiceMock.Verify(service =>
                service.UpdatePatientWithNewCodeAsync(
                    It.Is(SamePatientAs(outputPatient)),
                    inputNotificationPreference,
                    outputDateTimeOffset,
                    true),
                        Times.Once);

            this.notificationServiceMock.Verify(service =>
                service.SendCodeNotificationAsync(It.Is(SameNotificationInfoAs(inputNotificationInfo))),
                        Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.pdsServiceMock.VerifyNoOtherCalls();
            this.patientServiceMock.VerifyNoOtherCalls();
            this.notificationServiceMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldRecordPatientInformationAsyncWhenCodeIsExpiredForNonAuthenticatedUser()
        {
            // given
            int expireAfterMinutes = this.decisionConfigurations.PatientValidationCodeExpireAfterMinutes;
            string randomNhsNumber = GenerateRandom10DigitNumber();
            string inputNhsNumber = randomNhsNumber.DeepClone();
            string randomCaptchaToken = GetRandomString();
            string inputCaptchaToken = randomCaptchaToken.DeepClone();
            NotificationPreference randomNotificationPreference = NotificationPreference.Email;
            NotificationPreference inputNotificationPreference = randomNotificationPreference.DeepClone();
            string notificationPreferenceString = inputNotificationPreference.ToString();
            string outputValidationCode = GetRandomStringWithLengthOf(5);
            DateTimeOffset randomDateTimeOffest = GetRandomDateTimeOffset();
            DateTimeOffset outputDateTimeOffset = randomDateTimeOffest.DeepClone();
            Patient randomPatient = GetRandomPatientWithNhsNumber(inputNhsNumber);
            randomPatient.NotificationPreference = inputNotificationPreference;
            randomPatient.ValidationCodeExpiresOn = outputDateTimeOffset.AddMinutes(-1);
            Patient outputPatient = randomPatient.DeepClone();
            List<Patient> randomPatients = GetRandomPatients();
            randomPatients.Add(outputPatient);
            List<Patient> outputPatients = randomPatients.DeepClone();
            Patient updatedPatient = outputPatient.DeepClone();
            updatedPatient.ValidationCode = outputValidationCode;
            updatedPatient.ValidationCodeMatchedOn = null;
            updatedPatient.ValidationCodeExpiresOn = outputDateTimeOffset.AddMinutes(expireAfterMinutes);
            updatedPatient.NotificationPreference = inputNotificationPreference;
            updatedPatient.RetryCount = 0;

            NotificationInfo inputNotificationInfo = new NotificationInfo
            {
                Patient = updatedPatient
            };

            var patientOrchestrationServiceMock = new Mock<PatientOrchestrationService>(
                this.loggingBrokerMock.Object,
                this.securityBrokerMock.Object,
                this.dateTimeBrokerMock.Object,
                this.pdsServiceMock.Object,
                this.patientServiceMock.Object,
                this.notificationServiceMock.Object,
                this.identifierBrokerMock.Object,
                this.decisionConfigurations)
            { CallBase = true };

            patientOrchestrationServiceMock.Setup(service =>
                service.CheckIfIsAuthenticatedUserWithRequiredRoleAsync())
                    .ReturnsAsync(false);

            this.patientServiceMock.Setup(service =>
                service.RetrieveAllPatientsAsync())
                    .ReturnsAsync(outputPatients.AsQueryable);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(outputDateTimeOffset);

            patientOrchestrationServiceMock.Setup(service =>
                service.UpdatePatientWithNewCodeAsync(
                    It.Is(SamePatientAs(outputPatient)),
                    inputNotificationPreference,
                    outputDateTimeOffset,
                    true))
                        .ReturnsAsync(updatedPatient);

            // when
            await patientOrchestrationServiceMock.Object.RecordPatientInformationAsync(
                inputNhsNumber,
                notificationPreferenceString,
                false);

            //then
            patientOrchestrationServiceMock.Verify(service =>
                service.CheckIfIsAuthenticatedUserWithRequiredRoleAsync(),
                    Times.Once);

            this.patientServiceMock.Verify(service =>
                service.RetrieveAllPatientsAsync(),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            patientOrchestrationServiceMock.Verify(service =>
                service.UpdatePatientWithNewCodeAsync(
                    It.Is(SamePatientAs(outputPatient)),
                    inputNotificationPreference,
                    outputDateTimeOffset,
                    true),
                        Times.Once);

            this.notificationServiceMock.Verify(service =>
                service.SendCodeNotificationAsync(It.Is(SameNotificationInfoAs(inputNotificationInfo))),
                        Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.pdsServiceMock.VerifyNoOtherCalls();
            this.patientServiceMock.VerifyNoOtherCalls();
            this.notificationServiceMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldErrorOnRecordPatientInformationAsyncWhenMaxRetryExceededForNonAuthenticatedUser()
        {
            // given
            int expireAfterMinutes = this.decisionConfigurations.PatientValidationCodeExpireAfterMinutes;
            string randomNhsNumber = GenerateRandom10DigitNumber();
            string inputNhsNumber = randomNhsNumber.DeepClone();
            string randomCaptchaToken = GetRandomString();
            string inputCaptchaToken = randomCaptchaToken.DeepClone();
            NotificationPreference randomNotificationPreference = NotificationPreference.Email;
            NotificationPreference inputNotificationPreference = randomNotificationPreference.DeepClone();
            string notificationPreferenceString = inputNotificationPreference.ToString();
            string outputValidationCode = GetRandomStringWithLengthOf(5);
            DateTimeOffset randomDateTimeOffest = GetRandomDateTimeOffset();
            DateTimeOffset outputDateTimeOffset = randomDateTimeOffest.DeepClone();
            Patient randomPatient = GetRandomPatientWithNhsNumber(inputNhsNumber);
            randomPatient.NotificationPreference = inputNotificationPreference;
            randomPatient.ValidationCodeExpiresOn = outputDateTimeOffset.AddMinutes(1);
            randomPatient.RetryCount = this.decisionConfigurations.MaxRetryCount + 1;
            Patient outputPatient = randomPatient.DeepClone();
            List<Patient> randomPatients = GetRandomPatients();
            randomPatients.Add(outputPatient);
            List<Patient> outputPatients = randomPatients.DeepClone();
            Patient updatedPatient = outputPatient.DeepClone();

            var maxRetryAttemptsExceededException =
                new MaxRetryAttemptsExceededException(
                    "The maximum number of validation attempts has been exceeded, please contact support.");

            var expectedPatientOrchestrationValidationException =
                new PatientOrchestrationValidationException(
                    message: "Patient orchestration validation error occurred, please fix the errors and try again.",
                    innerException: maxRetryAttemptsExceededException);

            var patientOrchestrationServiceMock = new Mock<PatientOrchestrationService>(
                this.loggingBrokerMock.Object,
                this.securityBrokerMock.Object,
                this.dateTimeBrokerMock.Object,
                this.pdsServiceMock.Object,
                this.patientServiceMock.Object,
                this.notificationServiceMock.Object,
                this.identifierBrokerMock.Object,
                this.decisionConfigurations)
            { CallBase = true };

            patientOrchestrationServiceMock.Setup(service =>
                service.CheckIfIsAuthenticatedUserWithRequiredRoleAsync())
                    .ReturnsAsync(false);

            this.patientServiceMock.Setup(service =>
                service.RetrieveAllPatientsAsync())
                    .ReturnsAsync(outputPatients.AsQueryable);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(outputDateTimeOffset);

            // when
            ValueTask recordPatientInformationTask = patientOrchestrationServiceMock.Object.RecordPatientInformationAsync(
                inputNhsNumber,
                notificationPreferenceString,
                true);

            PatientOrchestrationValidationException
                actualPatientOrchestrationValidationException =
                    await Assert.ThrowsAsync<PatientOrchestrationValidationException>(
                        testCode: recordPatientInformationTask.AsTask);

            //then
            actualPatientOrchestrationValidationException
                .Should().BeEquivalentTo(expectedPatientOrchestrationValidationException);

            patientOrchestrationServiceMock.Verify(service =>
                service.CheckIfIsAuthenticatedUserWithRequiredRoleAsync(),
                    Times.Once);

            this.patientServiceMock.Verify(service =>
                service.RetrieveAllPatientsAsync(),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
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
            this.identifierBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldRecordPatientInformationAsyncWhenMaxRetryNotExceededForNonAuthenticatedUser()
        {
            // given
            int expireAfterMinutes = this.decisionConfigurations.PatientValidationCodeExpireAfterMinutes;
            string randomNhsNumber = GenerateRandom10DigitNumber();
            string inputNhsNumber = randomNhsNumber.DeepClone();
            string randomCaptchaToken = GetRandomString();
            string inputCaptchaToken = randomCaptchaToken.DeepClone();
            NotificationPreference randomNotificationPreference = NotificationPreference.Email;
            NotificationPreference inputNotificationPreference = randomNotificationPreference.DeepClone();
            string notificationPreferenceString = inputNotificationPreference.ToString();
            string outputValidationCode = GetRandomStringWithLengthOf(5);
            DateTimeOffset randomDateTimeOffest = GetRandomDateTimeOffset();
            DateTimeOffset outputDateTimeOffset = randomDateTimeOffest.DeepClone();
            Patient randomPatient = GetRandomPatientWithNhsNumber(inputNhsNumber);
            randomPatient.NotificationPreference = inputNotificationPreference;
            randomPatient.ValidationCodeExpiresOn = outputDateTimeOffset.AddMinutes(1);
            randomPatient.RetryCount = 1;
            Patient outputPatient = randomPatient.DeepClone();
            List<Patient> randomPatients = GetRandomPatients();
            randomPatients.Add(outputPatient);
            List<Patient> outputPatients = randomPatients.DeepClone();
            Patient updatedPatient = outputPatient.DeepClone();
            updatedPatient.ValidationCode = outputValidationCode;
            updatedPatient.ValidationCodeMatchedOn = null;
            updatedPatient.ValidationCodeExpiresOn = outputDateTimeOffset.AddMinutes(expireAfterMinutes);
            updatedPatient.NotificationPreference = inputNotificationPreference;

            NotificationInfo inputNotificationInfo = new NotificationInfo
            {
                Patient = updatedPatient
            };

            var patientOrchestrationServiceMock = new Mock<PatientOrchestrationService>(
                this.loggingBrokerMock.Object,
                this.securityBrokerMock.Object,
                this.dateTimeBrokerMock.Object,
                this.pdsServiceMock.Object,
                this.patientServiceMock.Object,
                this.notificationServiceMock.Object,
                this.identifierBrokerMock.Object,
                this.decisionConfigurations)
            { CallBase = true };

            patientOrchestrationServiceMock.Setup(service =>
                service.CheckIfIsAuthenticatedUserWithRequiredRoleAsync())
                    .ReturnsAsync(false);

            this.patientServiceMock.Setup(service =>
                service.RetrieveAllPatientsAsync())
                    .ReturnsAsync(outputPatients.AsQueryable);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(outputDateTimeOffset);

            patientOrchestrationServiceMock.Setup(service =>
                service.UpdatePatientWithNewCodeAsync(
                    It.Is(SamePatientAs(outputPatient)),
                    inputNotificationPreference,
                    outputDateTimeOffset,
                    false))
                        .ReturnsAsync(updatedPatient);

            // when
            await patientOrchestrationServiceMock.Object.RecordPatientInformationAsync(
                inputNhsNumber,
                notificationPreferenceString,
                false);

            //then
            patientOrchestrationServiceMock.Verify(service =>
                service.CheckIfIsAuthenticatedUserWithRequiredRoleAsync(),
                    Times.Once);

            this.patientServiceMock.Verify(service =>
                service.RetrieveAllPatientsAsync(),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            patientOrchestrationServiceMock.Verify(service =>
                service.UpdatePatientWithNewCodeAsync(
                    It.Is(SamePatientAs(outputPatient)),
                    inputNotificationPreference,
                    outputDateTimeOffset,
                    false),
                        Times.Once);

            this.notificationServiceMock.Verify(service =>
                service.SendCodeNotificationAsync(It.Is(SameNotificationInfoAs(inputNotificationInfo))),
                        Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.pdsServiceMock.VerifyNoOtherCalls();
            this.patientServiceMock.VerifyNoOtherCalls();
            this.notificationServiceMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
        }
    }
}
