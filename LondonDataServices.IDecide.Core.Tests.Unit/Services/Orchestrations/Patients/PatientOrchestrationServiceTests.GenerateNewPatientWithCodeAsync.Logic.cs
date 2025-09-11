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
using Moq;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Orchestrations.Patients
{
    public partial class PatientOrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldGenerateNewPatientWithCodeAsync()
        {
            // given
            int expireAfterMinutes = this.decisionConfigurations.PatientValidationCodeExpireAfterMinutes;
            string inputNhsNumber = GenerateRandom10DigitNumber();
            NotificationPreference inputNotificationPreference = NotificationPreference.Email;
            string outputValidationCode = GetRandomStringWithLengthOf(5);
            DateTimeOffset inputDateTimeOffset = GetRandomDateTimeOffset();
            Guid generatedId = Guid.NewGuid();

            Patient inputPatient = GetRandomPatientWithNhsNumber(inputNhsNumber);
            inputPatient.NotificationPreference = inputNotificationPreference;
            Patient outputPatient = inputPatient.DeepClone();

            Patient updatedPatient = outputPatient.DeepClone();
            updatedPatient.ValidationCode = outputValidationCode;
            updatedPatient.ValidationCodeExpiresOn = inputDateTimeOffset.AddMinutes(expireAfterMinutes);
            updatedPatient.ValidationCodeMatchedOn = null;
            updatedPatient.NotificationPreference = inputNotificationPreference;

            Patient expectedPatient = updatedPatient.DeepClone();

            this.pdsServiceMock.Setup(service =>
                service.PatientLookupByNhsNumberAsync(inputNhsNumber))
                    .ReturnsAsync(outputPatient);

            this.patientServiceMock.Setup(service =>
                service.GenerateValidationCodeAsync())
                    .ReturnsAsync(outputValidationCode);

            this.patientServiceMock.Setup(service =>
                service.RetrieveAllPatientsAsync())
                    .ReturnsAsync(new List<Patient>().AsQueryable());

            this.identifierBrokerMock.Setup(broker =>
                broker.GetIdentifierAsync())
                    .ReturnsAsync(generatedId);

            this.patientServiceMock.Setup(service =>
                service.AddPatientAsync(It.Is<Patient>(p =>
                    p.NhsNumber == updatedPatient.NhsNumber &&
                    p.ValidationCode == updatedPatient.ValidationCode &&
                    p.ValidationCodeExpiresOn == updatedPatient.ValidationCodeExpiresOn &&
                    p.NotificationPreference == updatedPatient.NotificationPreference)))
                .ReturnsAsync(updatedPatient);

            // when
            Patient actualPatient = await patientOrchestrationService.GenerateNewPatientWithCodeAsync(
                inputNhsNumber,
                inputNotificationPreference,
                inputDateTimeOffset);

            // then
            actualPatient.Should().BeEquivalentTo(expectedPatient);

            this.pdsServiceMock.Verify(service =>
                service.PatientLookupByNhsNumberAsync(inputNhsNumber), Times.Once);

            this.patientServiceMock.Verify(service =>
                service.GenerateValidationCodeAsync(), Times.Once);

            this.patientServiceMock.Verify(service =>
                service.RetrieveAllPatientsAsync(), Times.Once);

            this.identifierBrokerMock.Verify(broker =>
                broker.GetIdentifierAsync(), Times.Once);

            this.patientServiceMock.Verify(service =>
                service.AddPatientAsync(It.IsAny<Patient>()), Times.Exactly(2));

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
