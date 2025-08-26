// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Force.DeepCloner;
using LondonDataServices.IDecide.Core.Models.Foundations.Notifications;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using LondonDataServices.IDecide.Core.Models.Orchestrations.Decisions;
using LondonDataServices.IDecide.Core.Services.Orchestrations.Patients;
using Moq;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Orchestrations.Patients
{
    public partial class PatientOrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldRecordPatientInformationAsyncWhenNoPatientFound()
        {
            // given
            int expireAfterMinutes = this.decisionConfigurations.PatientValidationCodeExpireAfterMinutes;
            int maxRetryCount = this.decisionConfigurations.MaxRetryCount;
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
            updatedPatient.ValidationCodeExpiresOn = outputDateTimeOffset.AddMinutes(expireAfterMinutes);
            updatedPatient.NotificationPreference = inputNotificationPreference;

            NotificationInfo inputNotificationInfo = new NotificationInfo
            {
                Patient = updatedPatient
            };

            DecisionConfigurations decisionConfigurations =
                new DecisionConfigurations
                {
                    PatientValidationCodeExpireAfterMinutes = expireAfterMinutes,
                    MaxRetryCount = maxRetryCount
                };

            var patientOrchestrationServiceMock = new Mock<PatientOrchestrationService>(
                loggingBrokerMock.Object,
                securityBrokerMock.Object,
                dateTimeBrokerMock.Object,
                pdsServiceMock.Object,
                patientServiceMock.Object,
                notificationServiceMock.Object,
                decisionConfigurations)
            { CallBase = true };

            this.securityBrokerMock.Setup(broker =>
                broker.ValidateCaptchaAsync(inputCaptchaToken, ""))
                    .ReturnsAsync(true);

            this.patientServiceMock.Setup(service =>
                service.RetrieveAllPatientsAsync())
                    .ReturnsAsync(outputPatients.AsQueryable);

            this.pdsServiceMock.Setup(service =>
                service.PatientLookupByNhsNumberAsync(inputNhsNumber))
                    .ReturnsAsync(outputPatient);

            patientOrchestrationServiceMock.Setup(service =>
                service.GenerateValidationCode())
                    .Returns(outputValidationCode);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(outputDateTimeOffset);

            this.patientServiceMock.Setup(service =>
                service.AddPatientAsync(It.Is(SamePatientAs(updatedPatient))))
                    .ReturnsAsync(updatedPatient);

            // when
            await patientOrchestrationServiceMock.Object.RecordPatientInformation(
                inputNhsNumber,
                inputCaptchaToken,
                notificationPreferenceString,
                false);

            //then
            this.securityBrokerMock.Verify(broker =>
                broker.ValidateCaptchaAsync(inputCaptchaToken, ""),
                    Times.Once);

            this.patientServiceMock.Verify(service =>
                service.RetrieveAllPatientsAsync(),
                    Times.Once);

            this.pdsServiceMock.Verify(service =>
                service.PatientLookupByNhsNumberAsync(inputNhsNumber),
                    Times.Once);

            patientOrchestrationServiceMock.Verify(service =>
                service.GenerateValidationCode(),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.patientServiceMock.Verify(service =>
                service.AddPatientAsync(It.Is(SamePatientAs(updatedPatient))),
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
        public async Task ShouldRecordPatientInformationAsyncWhenPatientFoundWithExpiredCode()
        {
            // given
            int expireAfterMinutes = this.decisionConfigurations.PatientValidationCodeExpireAfterMinutes;
            int maxRetryCount = this.decisionConfigurations.MaxRetryCount;
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
            randomPatient.ValidationCodeExpiresOn = outputDateTimeOffset.AddMinutes(-1 * expireAfterMinutes);
            randomPatient.NotificationPreference = inputNotificationPreference;
            Patient outputPatient = randomPatient.DeepClone();
            List<Patient> randomPatients = GetRandomPatients();
            randomPatients.Add(outputPatient);
            List<Patient> outputPatients = randomPatients.DeepClone();
            Patient updatedPatient = outputPatient.DeepClone();
            updatedPatient.ValidationCode = outputValidationCode;
            updatedPatient.ValidationCodeExpiresOn = outputDateTimeOffset.AddMinutes(expireAfterMinutes);
            updatedPatient.NotificationPreference = inputNotificationPreference;

            NotificationInfo inputNotificationInfo = new NotificationInfo
            {
                Patient = updatedPatient
            };

            DecisionConfigurations decisionConfigurations =
                new DecisionConfigurations
                {
                    PatientValidationCodeExpireAfterMinutes = expireAfterMinutes,
                    MaxRetryCount = maxRetryCount
                };

            var patientOrchestrationServiceMock = new Mock<PatientOrchestrationService>(
                loggingBrokerMock.Object,
                securityBrokerMock.Object,
                dateTimeBrokerMock.Object,
                pdsServiceMock.Object,
                patientServiceMock.Object,
                notificationServiceMock.Object,
                decisionConfigurations)
            { CallBase = true };

            this.securityBrokerMock.Setup(broker =>
                broker.ValidateCaptchaAsync(inputCaptchaToken, ""))
                    .ReturnsAsync(true);

            this.patientServiceMock.Setup(service =>
                service.RetrieveAllPatientsAsync())
                    .ReturnsAsync(outputPatients.AsQueryable);

            this.pdsServiceMock.Setup(service =>
                service.PatientLookupByNhsNumberAsync(inputNhsNumber))
                    .ReturnsAsync(outputPatient);

            patientOrchestrationServiceMock.Setup(service =>
                service.GenerateValidationCode())
                    .Returns(outputValidationCode);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(outputDateTimeOffset);

            this.patientServiceMock.Setup(service =>
                service.ModifyPatientAsync(It.Is(SamePatientAs(updatedPatient))))
                    .ReturnsAsync(updatedPatient);

            // when
            await patientOrchestrationServiceMock.Object.RecordPatientInformation(
                inputNhsNumber,
                inputCaptchaToken,
                notificationPreferenceString,
                false);

            //then
            this.securityBrokerMock.Verify(broker =>
                broker.ValidateCaptchaAsync(inputCaptchaToken, ""),
                    Times.Once);

            this.patientServiceMock.Verify(service =>
                service.RetrieveAllPatientsAsync(),
                    Times.Once);

            this.pdsServiceMock.Verify(service =>
                service.PatientLookupByNhsNumberAsync(inputNhsNumber),
                    Times.Once);

            patientOrchestrationServiceMock.Verify(service =>
                service.GenerateValidationCode(),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.patientServiceMock.Verify(service =>
                service.ModifyPatientAsync(It.Is(SamePatientAs(updatedPatient))),
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
        public async Task ShouldRecordPatientInformationAsyncWhenPatientFoundWithValidCodeAndGenerateNewCodeIsTrue()
        {
            // given
            int expireAfterMinutes = this.decisionConfigurations.PatientValidationCodeExpireAfterMinutes;
            int maxRetryCount = this.decisionConfigurations.MaxRetryCount;
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
            randomPatient.ValidationCodeExpiresOn = outputDateTimeOffset.AddMinutes(expireAfterMinutes);
            randomPatient.NotificationPreference = inputNotificationPreference;
            Patient outputPatient = randomPatient.DeepClone();
            List<Patient> randomPatients = GetRandomPatients();
            randomPatients.Add(outputPatient);
            List<Patient> outputPatients = randomPatients.DeepClone();
            Patient updatedPatient = outputPatient.DeepClone();
            updatedPatient.ValidationCode = outputValidationCode;
            updatedPatient.ValidationCodeExpiresOn = outputDateTimeOffset.AddMinutes(expireAfterMinutes);
            updatedPatient.NotificationPreference = inputNotificationPreference;

            NotificationInfo inputNotificationInfo = new NotificationInfo
            {
                Patient = updatedPatient
            };

            DecisionConfigurations decisionConfigurations =
                new DecisionConfigurations
                {
                    PatientValidationCodeExpireAfterMinutes = expireAfterMinutes,
                    MaxRetryCount = maxRetryCount
                };

            var patientOrchestrationServiceMock = new Mock<PatientOrchestrationService>(
                loggingBrokerMock.Object,
                securityBrokerMock.Object,
                dateTimeBrokerMock.Object,
                pdsServiceMock.Object,
                patientServiceMock.Object,
                notificationServiceMock.Object,
                decisionConfigurations)
            { CallBase = true };

            this.securityBrokerMock.Setup(broker =>
                broker.ValidateCaptchaAsync(inputCaptchaToken, ""))
                    .ReturnsAsync(true);

            this.patientServiceMock.Setup(service =>
                service.RetrieveAllPatientsAsync())
                    .ReturnsAsync(outputPatients.AsQueryable);

            this.pdsServiceMock.Setup(service =>
                service.PatientLookupByNhsNumberAsync(inputNhsNumber))
                    .ReturnsAsync(outputPatient);

            patientOrchestrationServiceMock.Setup(service =>
                service.GenerateValidationCode())
                    .Returns(outputValidationCode);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(outputDateTimeOffset);

            this.patientServiceMock.Setup(service =>
                service.ModifyPatientAsync(It.Is(SamePatientAs(updatedPatient))))
                    .ReturnsAsync(updatedPatient);

            // when
            await patientOrchestrationServiceMock.Object.RecordPatientInformation(
                inputNhsNumber,
                inputCaptchaToken,
                notificationPreferenceString,
                true);

            //then
            this.securityBrokerMock.Verify(broker =>
                broker.ValidateCaptchaAsync(inputCaptchaToken, ""),
                    Times.Once);

            this.patientServiceMock.Verify(service =>
                service.RetrieveAllPatientsAsync(),
                    Times.Once);

            this.pdsServiceMock.Verify(service =>
                service.PatientLookupByNhsNumberAsync(inputNhsNumber),
                    Times.Once);

            patientOrchestrationServiceMock.Verify(service =>
                service.GenerateValidationCode(),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.patientServiceMock.Verify(service =>
                service.ModifyPatientAsync(It.Is(SamePatientAs(updatedPatient))),
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
    }
}
