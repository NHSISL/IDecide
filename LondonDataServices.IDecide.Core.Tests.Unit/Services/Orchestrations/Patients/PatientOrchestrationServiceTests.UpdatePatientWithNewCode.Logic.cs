// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using LondonDataServices.IDecide.Core.Models.Foundations.Notifications;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using Moq;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Orchestrations.Patients
{
    public partial class PatientOrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldUpdatePatientWithNewCodeAsyncWithoutResetRetry()
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
            randomPatient.RetryCount = 1;
            Patient outputPatient = randomPatient.DeepClone();
            List<Patient> randomPatients = GetRandomPatients();
            List<Patient> outputPatients = randomPatients.DeepClone();
            Patient updatedPatient = outputPatient.DeepClone();
            updatedPatient.ValidationCode = outputValidationCode;
            updatedPatient.ValidationCodeMatchedOn = null;
            updatedPatient.ValidationCodeExpiresOn = outputDateTimeOffset.AddMinutes(expireAfterMinutes);
            updatedPatient.NotificationPreference = inputNotificationPreference;
            Patient outputUpdatedPatient = updatedPatient.DeepClone();
            Patient expectedPatient = outputUpdatedPatient.DeepClone();

            this.pdsServiceMock.Setup(service =>
                service.PatientLookupByNhsNumberAsync(outputPatient.NhsNumber))
                    .ReturnsAsync(outputPatient);

            this.patientServiceMock.Setup(service =>
                service.GenerateValidationCodeAsync())
                    .ReturnsAsync(outputValidationCode);

            this.patientServiceMock.Setup(service =>
                service.ModifyPatientAsync(It.Is(SamePatientAs(updatedPatient))))
                    .ReturnsAsync(outputUpdatedPatient);

            // when
            Patient actualPatient = await patientOrchestrationService.UpdatePatientWithNewCodeAsync(
                outputPatient,
                inputNotificationPreference,
                outputDateTimeOffset,
                false);

            //then
            actualPatient.Should().BeEquivalentTo(expectedPatient);

            this.pdsServiceMock.Verify(service =>
                service.PatientLookupByNhsNumberAsync(outputPatient.NhsNumber),
                    Times.Once);

            this.patientServiceMock.Verify(service =>
                service.GenerateValidationCodeAsync(),
                    Times.Once);

            this.patientServiceMock.Verify(service =>
                service.ModifyPatientAsync(It.Is(SamePatientAs(updatedPatient))),
                    Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.pdsServiceMock.VerifyNoOtherCalls();
            this.patientServiceMock.VerifyNoOtherCalls();
            this.notificationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldUpdatePatientWithNewCodeAsyncWithResetRetry()
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
            randomPatient.RetryCount = 1;
            Patient outputPatient = randomPatient.DeepClone();
            List<Patient> randomPatients = GetRandomPatients();
            List<Patient> outputPatients = randomPatients.DeepClone();
            Patient updatedPatient = outputPatient.DeepClone();
            updatedPatient.ValidationCode = outputValidationCode;
            updatedPatient.ValidationCodeMatchedOn = null;
            updatedPatient.ValidationCodeExpiresOn = outputDateTimeOffset.AddMinutes(expireAfterMinutes);
            updatedPatient.NotificationPreference = inputNotificationPreference;
            updatedPatient.RetryCount = 0;
            Patient outputUpdatedPatient = updatedPatient.DeepClone();
            Patient expectedPatient = outputUpdatedPatient.DeepClone();

            this.pdsServiceMock.Setup(service =>
                service.PatientLookupByNhsNumberAsync(outputPatient.NhsNumber))
                    .ReturnsAsync(outputPatient);

            this.patientServiceMock.Setup(service =>
                service.GenerateValidationCodeAsync())
                    .ReturnsAsync(outputValidationCode);

            this.patientServiceMock.Setup(service =>
                service.ModifyPatientAsync(It.Is(SamePatientAs(updatedPatient))))
                    .ReturnsAsync(outputUpdatedPatient);

            // when
            Patient actualPatient = await patientOrchestrationService.UpdatePatientWithNewCodeAsync(
                outputPatient,
                inputNotificationPreference,
                outputDateTimeOffset,
                true);

            //then
            actualPatient.Should().BeEquivalentTo(expectedPatient);

            this.pdsServiceMock.Verify(service =>
                service.PatientLookupByNhsNumberAsync(outputPatient.NhsNumber),
                    Times.Once);

            this.patientServiceMock.Verify(service =>
                service.GenerateValidationCodeAsync(),
                    Times.Once);

            this.patientServiceMock.Verify(service =>
                service.ModifyPatientAsync(It.Is(SamePatientAs(updatedPatient))),
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
