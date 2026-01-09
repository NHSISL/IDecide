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
        //[Fact]
        //public async Task ShouldRecordPatientInformationAsyncWhenNhsLogin()
        //{
        //    // given
        //    Guid randomGuid = Guid.NewGuid();
        //    int expireAfterMinutes = this.decisionConfigurations.PatientValidationCodeExpireAfterMinutes;
        //    string randomNhsNumber = GenerateRandom10DigitNumber();
        //    string inputNhsNumber = randomNhsNumber.DeepClone();
        //    NotificationPreference randomNotificationPreference = NotificationPreference.Email;
        //    NotificationPreference inputNotificationPreference = randomNotificationPreference.DeepClone();
        //    string notificationPreferenceString = inputNotificationPreference.ToString();
        //    string outputValidationCode = GetRandomStringWithLengthOf(5);
        //    DateTimeOffset randomDateTimeOffest = GetRandomDateTimeOffset();
        //    DateTimeOffset outputDateTimeOffset = randomDateTimeOffest.DeepClone();
        //    Patient randomPatient = GetRandomPatientWithNhsNumber(inputNhsNumber);
        //    randomPatient.NotificationPreference = inputNotificationPreference;
        //    Patient outputPatient = randomPatient.DeepClone();
        //    List<Patient> randomPatients = GetRandomPatients();
        //    List<Patient> outputPatients = randomPatients.DeepClone();
        //    Patient updatedPatient = outputPatient.DeepClone();
        //    updatedPatient.ValidationCode = outputValidationCode;
        //    updatedPatient.ValidationCodeMatchedOn = null;
        //    updatedPatient.ValidationCodeExpiresOn = outputDateTimeOffset.AddMinutes(expireAfterMinutes);
        //    updatedPatient.NotificationPreference = inputNotificationPreference;

        //    NotificationInfo inputNotificationInfo = new NotificationInfo
        //    {
        //        Patient = updatedPatient
        //    };

        //    var patientOrchestrationServiceMock = new Mock<PatientOrchestrationService>(
        //        this.loggingBrokerMock.Object,
        //        this.securityBrokerMock.Object,
        //        this.dateTimeBrokerMock.Object,
        //        this.auditBrokerMock.Object,
        //        this.identifierBrokerMock.Object,
        //        this.pdsServiceMock.Object,
        //        this.patientServiceMock.Object,
        //        this.notificationServiceMock.Object,
        //        this.decisionConfigurations,
        //        this.securityBrokerConfigurations)
        //    { CallBase = true };

        //    this.patientServiceMock.Setup(service =>
        //        service.RetrieveAllPatientsAsync())
        //            .ReturnsAsync(outputPatients.AsQueryable);

        //    this.dateTimeBrokerMock.Setup(broker =>
        //        broker.GetCurrentDateTimeOffsetAsync())
        //            .ReturnsAsync(outputDateTimeOffset);

        //    this.identifierBrokerMock.Setup(broker =>
        //        broker.GetIdentifierAsync())
        //            .ReturnsAsync(randomGuid);

        //    patientOrchestrationServiceMock.Setup(service =>
        //        service.CreateNewPatientAsync(
        //            inputNhsNumber,
        //            inputNotificationPreference,
        //            outputDateTimeOffset))
        //                .ReturnsAsync(updatedPatient);

        //    // when
        //    await patientOrchestrationServiceMock.Object.RecordPatientInformationNhsLoginAsync(
        //        inputNhsNumber,
        //        notificationPreferenceString,
        //        false);

        //    //then

        //    this.patientServiceMock.Verify(service =>
        //        service.RetrieveAllPatientsAsync(),
        //            Times.Once);

        //    this.dateTimeBrokerMock.Verify(broker =>
        //        broker.GetCurrentDateTimeOffsetAsync(),
        //            Times.Once);

        //    this.identifierBrokerMock.Verify(broker =>
        //        broker.GetIdentifierAsync(),
        //            Times.Once);

        //    this.auditBrokerMock.Verify(broker => broker.LogInformationAsync(
        //        "Patient",
        //        "Recording Patient Information",
        //        $"Recording a patient with NHS Number {randomNhsNumber}.",
        //        null,
        //        randomGuid.ToString()),
        //            Times.Once);

        //    patientOrchestrationServiceMock.Verify(service =>
        //        service.CreateNewPatientAsync(
        //            inputNhsNumber,
        //            inputNotificationPreference,
        //            outputDateTimeOffset),
        //                Times.Once);

        //    this.notificationServiceMock.Verify(service =>
        //        service.SendCodeNotificationAsync(It.Is(SameNotificationInfoAs(inputNotificationInfo))),
        //                Times.Once);

        //    this.auditBrokerMock.Verify(broker => broker.LogInformationAsync(
        //        "Patient",
        //        "Patient Recorded",
        //        $"A new patient was created with NHS Number {randomNhsNumber} and validation code was sent.",
        //        null,
        //        randomGuid.ToString()),
        //            Times.Once);

        //    this.loggingBrokerMock.VerifyNoOtherCalls();
        //    this.securityBrokerMock.VerifyNoOtherCalls();
        //    this.dateTimeBrokerMock.VerifyNoOtherCalls();
        //    this.auditBrokerMock.VerifyNoOtherCalls();
        //    this.identifierBrokerMock.VerifyNoOtherCalls();
        //    this.pdsServiceMock.VerifyNoOtherCalls();
        //    this.patientServiceMock.VerifyNoOtherCalls();
        //    this.notificationServiceMock.VerifyNoOtherCalls();
        //}
    }
}
