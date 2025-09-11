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
        public async Task ShouldCreateNewPatientAsync()
        {
            // given
            Guid randomIdentifier = Guid.NewGuid();
            int expireAfterMinutes = this.decisionConfigurations.PatientValidationCodeExpireAfterMinutes;
            string randomNhsNumber = GenerateRandom10DigitNumber();
            string inputNhsNumber = randomNhsNumber.DeepClone();
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
            updatedPatient.Id = randomIdentifier;
            updatedPatient.ValidationCode = outputValidationCode;
            updatedPatient.ValidationCodeMatchedOn = null;
            updatedPatient.ValidationCodeExpiresOn = outputDateTimeOffset.AddMinutes(expireAfterMinutes);
            updatedPatient.NotificationPreference = inputNotificationPreference;
            Patient outputUpdatedPatient = updatedPatient.DeepClone();
            Patient expectedPatient = outputUpdatedPatient.DeepClone();

            this.pdsServiceMock.Setup(service =>
                service.PatientLookupByNhsNumberAsync(inputNhsNumber))
                    .ReturnsAsync(outputPatient);

            this.patientServiceMock.Setup(service =>
                service.GenerateValidationCodeAsync())
                    .ReturnsAsync(outputValidationCode);

            this.identifierBrokerMock.Setup(broker =>
                broker.GetIdentifierAsync())
                    .ReturnsAsync(randomIdentifier);

            this.patientServiceMock.Setup(service =>
                service.AddPatientAsync(It.Is(SamePatientAs(updatedPatient))))
                    .ReturnsAsync(outputUpdatedPatient);

            // when
            Patient actualPatient = await patientOrchestrationService.CreateNewPatientAsync(
                inputNhsNumber,
                inputNotificationPreference,
                outputDateTimeOffset);

            //then
            actualPatient.Should().BeEquivalentTo(expectedPatient);

            this.pdsServiceMock.Verify(service =>
                service.PatientLookupByNhsNumberAsync(inputNhsNumber),
                    Times.Once);

            this.patientServiceMock.Verify(service =>
                service.GenerateValidationCodeAsync(),
                    Times.Once);

            this.identifierBrokerMock.Verify(service =>
                service.GetIdentifierAsync(),
                    Times.Once);

            this.patientServiceMock.Verify(service =>
                service.AddPatientAsync(It.Is(SamePatientAs(updatedPatient))),
                    Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.auditBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
            this.pdsServiceMock.VerifyNoOtherCalls();
            this.patientServiceMock.VerifyNoOtherCalls();
            this.notificationServiceMock.VerifyNoOtherCalls();
        }
    }
}
