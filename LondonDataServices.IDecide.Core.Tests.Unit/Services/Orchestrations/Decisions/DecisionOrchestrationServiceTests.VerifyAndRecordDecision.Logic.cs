// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Force.DeepCloner;
using LondonDataServices.IDecide.Core.Models.Foundations.Decisions;
using LondonDataServices.IDecide.Core.Models.Foundations.Notifications;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using Moq;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Orchestrations.Decisions
{
    public partial class DecisionOrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldVerifyAndRecordDecisionAsync()
        {
            // given
            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();
            Patient randomPatient = GetRandomPatient(randomDateTime);
            List<Patient> randomPatients = GetRandomPatients(randomDateTime);
            randomPatients.Add(randomPatient);
            List<Patient> outputPatients = randomPatients.DeepClone();
            Decision randomDecision = GetRandomDecision(randomPatient);
            Decision inputDecision = randomDecision.DeepClone();
            Decision outputDecision = inputDecision.DeepClone();

            NotificationInfo inputNotificationInfo = new NotificationInfo
            {
                Patient = randomPatient,
                Decision = outputDecision
            };

            this.patientServiceMock.Setup(service =>
                service.RetrieveAllPatientsAsync())
                    .ReturnsAsync(outputPatients.AsQueryable);

            this.decisionServiceMock.Setup(service =>
                service.AddDecisionAsync(inputDecision))
                    .ReturnsAsync(outputDecision);

            // when
            await this.decisionOrchestrationService.VerifyAndRecordDecisionAsync(inputDecision);

            //then

            this.patientServiceMock.Verify(service =>
                service.RetrieveAllPatientsAsync(),
                    Times.Once);

            this.decisionServiceMock.Verify(service =>
                service.AddDecisionAsync(inputDecision),
                    Times.Once);

            this.notificationServiceMock.Verify(service =>
                service.SendSubmissionSuccessNotificationAsync(inputNotificationInfo),
                    Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.patientServiceMock.VerifyNoOtherCalls();
            this.notificationServiceMock.VerifyNoOtherCalls();
            this.decisionServiceMock.VerifyNoOtherCalls();
        }
    }
}
