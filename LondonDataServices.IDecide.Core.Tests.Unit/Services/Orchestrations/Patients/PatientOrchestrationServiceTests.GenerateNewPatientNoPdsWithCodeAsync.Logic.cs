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
        public async Task ShouldCreateNewPatientNoPdsAsync()
        {
            // given
            Guid randomIdentifier = Guid.NewGuid();
            int expireAfterMinutes = this.decisionConfigurations.PatientValidationCodeExpireAfterMinutes;
            string randomNhsNumber = GenerateRandom10DigitNumber();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            DateTimeOffset inputNow = randomDateTimeOffset.DeepClone();
            Patient inputPatient = GetRandomPatientWithNhsNumber(randomNhsNumber);
            Patient updatedPatient = inputPatient.DeepClone();
            updatedPatient.Id = randomIdentifier;
            updatedPatient.ValidationCode = "LOGIN";
            updatedPatient.ValidationCodeExpiresOn = inputNow.AddMinutes(expireAfterMinutes);
            updatedPatient.ValidationCodeMatchedOn = null;
            updatedPatient.NotificationPreference = NotificationPreference.None;
            updatedPatient.Gender = "Unknown";
            Patient outputUpdatedPatient = updatedPatient.DeepClone();
            Patient expectedPatient = outputUpdatedPatient.DeepClone();

            this.identifierBrokerMock.Setup(broker =>
                broker.GetIdentifierAsync())
                    .ReturnsAsync(randomIdentifier);

            this.patientServiceMock.Setup(service =>
                service.AddPatientAsync(It.Is(SamePatientAs(updatedPatient))))
                    .ReturnsAsync(outputUpdatedPatient);

            // when
            Patient actualPatient = await this.patientOrchestrationService.CreateNewPatientNoPdsAsync(
                inputPatient,
                inputNow);

            // then
            actualPatient.Should().BeEquivalentTo(expectedPatient);

            this.identifierBrokerMock.Verify(broker =>
                broker.GetIdentifierAsync(),
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