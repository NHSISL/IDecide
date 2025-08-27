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
using LondonDataServices.IDecide.Core.Models.Foundations.Notifications;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using LondonDataServices.IDecide.Core.Models.Orchestrations.Decisions.Exceptions;
using Moq;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Orchestrations.Decisions
{
    public partial class DecisionOrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldVerifyAndRecordDecisionAsyncWithValidDecisionAndNonDecisionWorflowRoleUser()
        {
            // given
            string randomNhsNumber = GenerateRandom10DigitNumber();
            string randomValidationCode = GetRandomStringWithLengthOf(5);
            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();
            Patient randomPatient = GetRandomPatient(randomDateTime, randomNhsNumber, randomValidationCode);
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

            foreach (string role in this.decisionConfigurations.DecisionWorkflowRoles)
            {
                this.securityBrokerMock.Setup(broker =>
                broker.IsInRoleAsync(role))
                    .ReturnsAsync(false);
            }

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTime);

            this.decisionServiceMock.Setup(service =>
                service.AddDecisionAsync(It.Is(SameDecisionAs(inputDecision))))
                    .ReturnsAsync(outputDecision);

            // when
            await this.decisionOrchestrationService.VerifyAndRecordDecisionAsync(inputDecision);

            //then
            this.patientServiceMock.Verify(service =>
                service.RetrieveAllPatientsAsync(),
                    Times.Once);

            foreach (string role in this.decisionConfigurations.DecisionWorkflowRoles)
            {
                this.securityBrokerMock.Verify(broker =>
                broker.IsInRoleAsync(role),
                    Times.Once);
            }

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.decisionServiceMock.Verify(service =>
                service.AddDecisionAsync(It.Is(SameDecisionAs(inputDecision))),
                    Times.Once);

            this.notificationServiceMock.Verify(service =>
                service.SendSubmissionSuccessNotificationAsync(It.Is(SameNotificationInfoAs(inputNotificationInfo))),
                    Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.patientServiceMock.VerifyNoOtherCalls();
            this.notificationServiceMock.VerifyNoOtherCalls();
            this.decisionServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldVerifyAndRecordDecisionAsyncWithValidDecisionAndDecisionWorflowRoleUser()
        {
            // given
            string randomNhsNumber = GenerateRandom10DigitNumber();
            string randomValidationCode = GetRandomStringWithLengthOf(5);
            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();
            Patient randomPatient = GetRandomPatient(randomDateTime, randomNhsNumber, randomValidationCode);
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

            foreach (string role in this.decisionConfigurations.DecisionWorkflowRoles)
            {
                this.securityBrokerMock.Setup(broker =>
                broker.IsInRoleAsync(role))
                    .ReturnsAsync(true);
            }

            this.decisionServiceMock.Setup(service =>
                service.AddDecisionAsync(It.Is(SameDecisionAs(inputDecision))))
                    .ReturnsAsync(outputDecision);

            // when
            await this.decisionOrchestrationService.VerifyAndRecordDecisionAsync(inputDecision);

            //then
            this.patientServiceMock.Verify(service =>
                service.RetrieveAllPatientsAsync(),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.IsInRoleAsync(this.decisionConfigurations.DecisionWorkflowRoles.First()),
                    Times.Once);

            this.decisionServiceMock.Verify(service =>
                service.AddDecisionAsync(It.Is(SameDecisionAs(inputDecision))),
                    Times.Once);

            this.notificationServiceMock.Verify(service =>
                service.SendSubmissionSuccessNotificationAsync(It.Is(SameNotificationInfoAs(inputNotificationInfo))),
                    Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.patientServiceMock.VerifyNoOtherCalls();
            this.notificationServiceMock.VerifyNoOtherCalls();
            this.decisionServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldErrorOnVerifyAndRecordDecisionAsyncWithInvalidValidationCodeAndDecisionWorflowRoleUser()
        {
            // given
            string randomNhsNumber = GenerateRandom10DigitNumber();
            string randomValidationCode = GetRandomStringWithLengthOf(5);
            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();
            Patient randomPatient = GetRandomPatient(randomDateTime, randomNhsNumber, randomValidationCode);
            Patient invalidCodePatient = randomPatient.DeepClone();
            string invalidValidationCode = GetRandomStringWithLengthOf(5);
            invalidCodePatient.ValidationCode = invalidValidationCode;
            List<Patient> randomPatients = GetRandomPatients(randomDateTime);
            randomPatients.Add(randomPatient);
            List<Patient> outputPatients = randomPatients.DeepClone();
            Decision randomDecision = GetRandomDecision(invalidCodePatient);
            Decision inputDecision = randomDecision.DeepClone();
            Decision outputDecision = inputDecision.DeepClone();

            var incorrectValidationCodeException =
                new IncorrectValidationCodeException("The validation code provided is incorrect.");

            var expectedDecisionOrchestrationValidationException =
                new DecisionOrchestrationValidationException(
                    message: "Decision orchestration validation error occurred, please fix the errors and try again.",
                    innerException: incorrectValidationCodeException);

            this.patientServiceMock.Setup(service =>
                service.RetrieveAllPatientsAsync())
                    .ReturnsAsync(outputPatients.AsQueryable);

            foreach (string role in this.decisionConfigurations.DecisionWorkflowRoles)
            {
                this.securityBrokerMock.Setup(broker =>
                broker.IsInRoleAsync(role))
                    .ReturnsAsync(true);
            }

            // when
            ValueTask verifyAndRecordDecisionTask =
               this.decisionOrchestrationService
                   .VerifyAndRecordDecisionAsync(inputDecision);

            DecisionOrchestrationValidationException
                actualDecisionOrchestrationValidationException =
                    await Assert.ThrowsAsync<DecisionOrchestrationValidationException>(
                        testCode: verifyAndRecordDecisionTask.AsTask);

            //then
            actualDecisionOrchestrationValidationException
                .Should().BeEquivalentTo(expectedDecisionOrchestrationValidationException);

            this.patientServiceMock.Verify(service =>
                service.RetrieveAllPatientsAsync(),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.IsInRoleAsync(this.decisionConfigurations.DecisionWorkflowRoles.First()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedDecisionOrchestrationValidationException))),
                       Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.patientServiceMock.VerifyNoOtherCalls();
            this.notificationServiceMock.VerifyNoOtherCalls();
            this.decisionServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldErrorOnVerifyAndRecordDecisionAsyncWithExceededRetryAndNonDecisionWorflowRoleUser()
        {
            // given
            string randomNhsNumber = GenerateRandom10DigitNumber();
            string randomValidationCode = GetRandomStringWithLengthOf(5);
            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();
            Patient randomPatient = GetRandomPatient(randomDateTime, randomNhsNumber, randomValidationCode);
            Patient updatedPatient = randomPatient.DeepClone();
            updatedPatient.RetryCount = 4;
            List<Patient> randomPatients = GetRandomPatients(randomDateTime);
            randomPatients.Add(updatedPatient);
            List<Patient> outputPatients = randomPatients.DeepClone();
            Decision randomDecision = GetRandomDecision(updatedPatient);
            Decision inputDecision = randomDecision.DeepClone();
            Decision outputDecision = inputDecision.DeepClone();

            this.patientServiceMock.Setup(service =>
                service.RetrieveAllPatientsAsync())
                    .ReturnsAsync(outputPatients.AsQueryable);

            foreach (string role in this.decisionConfigurations.DecisionWorkflowRoles)
            {
                this.securityBrokerMock.Setup(broker =>
                broker.IsInRoleAsync(role))
                    .ReturnsAsync(false);
            }

            var exceededMaxRetryCountException =
                new ExceededMaxRetryCountException(
                    $"The maximum retry count of {this.decisionConfigurations.MaxRetryCount} exceeded.");

            var expectedDecisionOrchestrationValidationException =
                new DecisionOrchestrationValidationException(
                    message: "Decision orchestration validation error occurred, please fix the errors and try again.",
                    innerException: exceededMaxRetryCountException);

            ValueTask verifyAndRecordDecisionTask =
               this.decisionOrchestrationService
                   .VerifyAndRecordDecisionAsync(inputDecision);

            DecisionOrchestrationValidationException
                actualDecisionOrchestrationValidationException =
                    await Assert.ThrowsAsync<DecisionOrchestrationValidationException>(
                        testCode: verifyAndRecordDecisionTask.AsTask);

            // then
            actualDecisionOrchestrationValidationException
                .Should().BeEquivalentTo(expectedDecisionOrchestrationValidationException);

            this.patientServiceMock.Verify(service =>
                service.RetrieveAllPatientsAsync(),
                    Times.Once);

            foreach (string role in this.decisionConfigurations.DecisionWorkflowRoles)
            {
                this.securityBrokerMock.Verify(broker =>
                broker.IsInRoleAsync(role),
                    Times.Once);
            }

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedDecisionOrchestrationValidationException))),
                       Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.patientServiceMock.VerifyNoOtherCalls();
            this.notificationServiceMock.VerifyNoOtherCalls();
            this.decisionServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task
            ShouldErrorOnVerifyAndRecordDecisionAsyncWithIncorrectValidationCodeAndNonDecisionWorflowRoleUser()
        {
            // given
            string randomNhsNumber = GenerateRandom10DigitNumber();
            string randomValidationCode = GetRandomStringWithLengthOf(5);
            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();
            Patient randomPatient = GetRandomPatient(randomDateTime, randomNhsNumber, randomValidationCode);
            Patient updatedPatient = randomPatient.DeepClone();
            string invalidValidationCode = GetRandomStringWithLengthOf(5);
            updatedPatient.ValidationCode = invalidValidationCode;
            List<Patient> randomPatients = GetRandomPatients(randomDateTime);
            randomPatients.Add(randomPatient);
            List<Patient> outputPatients = randomPatients.DeepClone();
            Decision randomDecision = GetRandomDecision(updatedPatient);
            Decision inputDecision = randomDecision.DeepClone();
            Decision outputDecision = inputDecision.DeepClone();
            Patient patientToUpdate = randomPatient.DeepClone();
            patientToUpdate.RetryCount += 1;

            this.patientServiceMock.Setup(service =>
                service.RetrieveAllPatientsAsync())
                    .ReturnsAsync(outputPatients.AsQueryable);

            foreach (string role in this.decisionConfigurations.DecisionWorkflowRoles)
            {
                this.securityBrokerMock.Setup(broker =>
                broker.IsInRoleAsync(role))
                    .ReturnsAsync(false);
            }

            var incorrectValidationCodeException =
                new IncorrectValidationCodeException("The validation code provided is incorrect.");

            var expectedDecisionOrchestrationValidationException =
                new DecisionOrchestrationValidationException(
                    message: "Decision orchestration validation error occurred, please fix the errors and try again.",
                    innerException: incorrectValidationCodeException);

            ValueTask verifyAndRecordDecisionTask =
               this.decisionOrchestrationService
                   .VerifyAndRecordDecisionAsync(inputDecision);

            DecisionOrchestrationValidationException
                actualDecisionOrchestrationValidationException =
                    await Assert.ThrowsAsync<DecisionOrchestrationValidationException>(
                        testCode: verifyAndRecordDecisionTask.AsTask);

            // then
            actualDecisionOrchestrationValidationException
                .Should().BeEquivalentTo(expectedDecisionOrchestrationValidationException);

            this.patientServiceMock.Verify(service =>
                service.RetrieveAllPatientsAsync(),
                    Times.Once);

            foreach (string role in this.decisionConfigurations.DecisionWorkflowRoles)
            {
                this.securityBrokerMock.Verify(broker =>
                broker.IsInRoleAsync(role),
                    Times.Once);
            }

            this.patientServiceMock.Verify(service =>
                service.ModifyPatientAsync(It.Is(SamePatientAs(patientToUpdate))),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedDecisionOrchestrationValidationException))),
                       Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.patientServiceMock.VerifyNoOtherCalls();
            this.notificationServiceMock.VerifyNoOtherCalls();
            this.decisionServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task
            ShouldErrorOnVerifyAndRecordDecisionAsyncWithExpiredValidationCodeAndNonDecisionWorflowRoleUser()
        {
            // given
            string randomNhsNumber = GenerateRandom10DigitNumber();
            string randomValidationCode = GetRandomStringWithLengthOf(5);
            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();
            DateTimeOffset expiredDateTimeOffset = randomDateTime.AddDays(-1);
            Patient randomPatient = GetRandomPatient(expiredDateTimeOffset, randomNhsNumber, randomValidationCode);
            List<Patient> randomPatients = GetRandomPatients(randomDateTime);
            randomPatients.Add(randomPatient);
            List<Patient> outputPatients = randomPatients.DeepClone();
            Decision randomDecision = GetRandomDecision(randomPatient);
            Decision inputDecision = randomDecision.DeepClone();
            Decision outputDecision = inputDecision.DeepClone();
            Patient patientToUpdate = randomPatient.DeepClone();
            string randomNewValidationCode = GetRandomStringWithLengthOf(5);

            DateTimeOffset newExpiryDateTimeOffset =
                randomDateTime.AddMinutes(this.decisionConfigurations.PatientValidationCodeExpireAfterMinutes);

            patientToUpdate.RetryCount = 0;
            patientToUpdate.ValidationCode = randomNewValidationCode;
            patientToUpdate.ValidationCodeExpiresOn = newExpiryDateTimeOffset;

            NotificationInfo inputNotificationInfo = new NotificationInfo
            {
                Patient = patientToUpdate,
                Decision = inputDecision
            };

            this.patientServiceMock.Setup(service =>
                service.RetrieveAllPatientsAsync())
                    .ReturnsAsync(outputPatients.AsQueryable);

            foreach (string role in this.decisionConfigurations.DecisionWorkflowRoles)
            {
                this.securityBrokerMock.Setup(broker =>
                broker.IsInRoleAsync(role))
                    .ReturnsAsync(false);
            }

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTime);

            this.patientServiceMock.Setup(service =>
                service.GenerateValidationCodeAsync())
                    .ReturnsAsync(randomNewValidationCode);

            var renewedValidationCodeException =
                new RenewedValidationCodeException("The validation code has expired, but we have issued a new code " +
                $"that will be sent via {inputDecision.Patient.NotificationPreference.ToString()}");

            var expectedDecisionOrchestrationValidationException =
                new DecisionOrchestrationValidationException(
                    message: "Decision orchestration validation error occurred, please fix the errors and try again.",
                    innerException: renewedValidationCodeException);

            ValueTask verifyAndRecordDecisionTask =
              this.decisionOrchestrationService
                  .VerifyAndRecordDecisionAsync(randomDecision);

            DecisionOrchestrationValidationException
                actualDecisionOrchestrationValidationException =
                    await Assert.ThrowsAsync<DecisionOrchestrationValidationException>(
                        testCode: verifyAndRecordDecisionTask.AsTask);

            // then
            actualDecisionOrchestrationValidationException
                .Should().BeEquivalentTo(expectedDecisionOrchestrationValidationException);

            this.patientServiceMock.Verify(service =>
                service.RetrieveAllPatientsAsync(),
                    Times.Once);

            foreach (string role in this.decisionConfigurations.DecisionWorkflowRoles)
            {
                this.securityBrokerMock.Verify(broker =>
                broker.IsInRoleAsync(role),
                    Times.Once);
            }

            this.dateTimeBrokerMock.Verify(service =>
                service.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.patientServiceMock.Verify(service =>
                service.GenerateValidationCodeAsync(),
                    Times.Once);

            this.patientServiceMock.Verify(service =>
                service.ModifyPatientAsync(It.Is(SamePatientAs(patientToUpdate))),
                    Times.Once);

            this.notificationServiceMock.Verify(service =>
                service.SendCodeNotificationAsync(It.Is(SameNotificationInfoAs(inputNotificationInfo))),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedDecisionOrchestrationValidationException))),
                       Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.patientServiceMock.VerifyNoOtherCalls();
            this.notificationServiceMock.VerifyNoOtherCalls();
            this.decisionServiceMock.VerifyNoOtherCalls();
        }
    }
}
