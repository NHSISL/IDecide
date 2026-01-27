// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using LondonDataServices.IDecide.Core.Models.Foundations.Decisions;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using LondonDataServices.IDecide.Core.Models.Orchestrations.Decisions.Exceptions;
using LondonDataServices.IDecide.Core.Models.Securities;
using LondonDataServices.IDecide.Core.Services.Orchestrations.Decisions;
using Moq;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Orchestrations.Decisions
{
    public partial class DecisionOrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldVerifyAndRecordDecisionNhsLoginAsyncWithValidDecisionAndNonDecisionWorflowRoleUser()
        {
            // given
            string randomNhsNumber = GenerateRandom10DigitNumber();
            string randomValidationCode = GetRandomStringWithLengthOf(5);
            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();
            Guid randomGuid = Guid.NewGuid();
            string randomIpAddress = GetRandomString();
            Patient randomPatient = GetRandomPatient(randomDateTime, randomNhsNumber, randomValidationCode);
            Patient outputPatient = randomPatient.DeepClone();
            List<Patient> randomPatients = GetRandomPatients(randomDateTime);
            randomPatients.Add(randomPatient);
            List<Patient> outputPatients = randomPatients.DeepClone();
            Decision randomDecision = GetRandomDecision(randomPatient);
            randomDecision.PatientId = randomPatient.Id;
            Decision inputDecision = randomDecision.DeepClone();
            Decision outputDecision = inputDecision.DeepClone();

            var decisionOrchestrationServiceMock = new Mock<DecisionOrchestrationService>(
                this.loggingBrokerMock.Object,
                this.dateTimeBrokerMock.Object,
                this.securityBrokerMock.Object,
                this.auditBrokerMock.Object,
                this.identifierBrokerMock.Object,
                this.patientServiceMock.Object,
                this.decisionServiceMock.Object,
                this.consumerServiceMock.Object,
                this.decisionConfigurations,
                this.securityBrokerConfigurations)
            { CallBase = true };

            this.patientServiceMock.Setup(service =>
                service.RetrieveAllPatientsAsync())
                    .ReturnsAsync(outputPatients.AsQueryable);

            this.identifierBrokerMock.Setup(broker =>
                broker.GetIdentifierAsync())
                    .ReturnsAsync(randomGuid);

            this.securityBrokerMock.Setup(broker =>
                broker.GetIpAddressAsync())
                    .ReturnsAsync(randomIpAddress);

            this.patientServiceMock.Setup(service =>
                service.ModifyPatientAsync(It.Is(SamePatientAs(randomPatient))))
                    .ReturnsAsync(outputPatient);

            this.decisionServiceMock.Setup(service =>
                service.AddDecisionAsync(It.Is(SameDecisionAs(inputDecision))))
                    .ReturnsAsync(outputDecision);

            // when
            await decisionOrchestrationServiceMock.Object.VerifyAndRecordDecisionNhsLoginAsync(
                inputDecision);

            //then
            this.patientServiceMock.Verify(service =>
                service.RetrieveAllPatientsAsync(),
                    Times.Once);

            this.identifierBrokerMock.Verify(broker =>
                broker.GetIdentifierAsync(),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetIpAddressAsync(),
                    Times.Once);

            this.auditBrokerMock.Verify(broker =>
                broker.LogInformationAsync(
                    "Decision",
                    "Verifying Decision",

                    $"Patient with IP address {randomIpAddress} is validating a code for " +
                        $"patient Nhs Number: {randomPatient.NhsNumber}, with PatientId {randomPatient.Id}",

                    null,
                    randomGuid.ToString()),
                        Times.Once);

            this.patientServiceMock.Verify(service =>
                service.ModifyPatientAsync(It.Is(SamePatientAs(randomPatient))),
                    Times.Once);

            this.decisionServiceMock.Verify(service =>
                service.AddDecisionAsync(It.Is(SameDecisionAs(inputDecision))),
                    Times.Once);

            this.auditBrokerMock.Verify(broker =>
                broker.LogInformationAsync(
                    "Decision",
                    "Decision Submitted",

                    $"The patient's decision has been successfully submitted for decisionId {randomDecision.Id}, " +
                        $"patient Nhs Number: {randomPatient.NhsNumber}, with PatientId {randomPatient.Id}",

                    null,
                    randomGuid.ToString()),
                        Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.auditBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
            this.patientServiceMock.VerifyNoOtherCalls();
            this.decisionServiceMock.VerifyNoOtherCalls();
        }
    }
}
