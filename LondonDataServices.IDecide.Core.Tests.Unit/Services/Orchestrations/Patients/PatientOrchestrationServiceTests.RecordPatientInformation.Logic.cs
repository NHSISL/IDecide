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
            Patient randomPatient = GetRandomPatient(inputNhsNumber);
            Patient outputPatient = randomPatient.DeepClone();
            List<Patient> randomPatients = GetRandomPatients();
            List<Patient> outputPatients = randomPatients.DeepClone();
            Patient updatedPatient = outputPatient.DeepClone();
            updatedPatient.ValidationCode = outputValidationCode;
            updatedPatient.ValidationCodeExpiresOn = outputDateTimeOffset.AddMinutes(10);
            updatedPatient.NotificationPreference = inputNotificationPreference;

            NotificationInfo inputNotificationInfo = new NotificationInfo
            {
                Patient = updatedPatient
            };

            var patientOrchestrationServiceMock = new Mock<PatientOrchestrationService>(
                loggingBrokerMock.Object,
                securityBrokerMock.Object,
                dateTimeBrokerMock.Object,
                pdsServiceMock.Object,
                patientServiceMock.Object,
                notificationServiceMock.Object)
            { CallBase = true };

            this.patientServiceMock.Setup(service =>
                service.RetrieveAllPatientsAsync())
                    .ReturnsAsync(randomPatients.AsQueryable);

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
                service.AddPatientAsync(updatedPatient))
                    .ReturnsAsync(updatedPatient);

            // when
            await patientOrchestrationServiceMock.Object.RecordPatientInformation(
                inputNhsNumber,
                inputCaptchaToken,
                notificationPreferenceString,
                false);

            //then
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
                service.AddPatientAsync(updatedPatient),
                    Times.Once);

            this.notificationServiceMock.Verify(service =>
                service.SendCodeNotificationAsync(inputNotificationInfo),
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
