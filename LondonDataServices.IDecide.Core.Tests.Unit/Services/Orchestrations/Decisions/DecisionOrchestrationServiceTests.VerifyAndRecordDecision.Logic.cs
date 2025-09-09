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
using LondonDataServices.IDecide.Core.Models.Securities;
using LondonDataServices.IDecide.Core.Services.Orchestrations.Decisions;
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
            Guid randomGuid = Guid.NewGuid();
            string randomIpAddress = GetRandomString();
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

            var decisionOrchestrationServiceMock = new Mock<DecisionOrchestrationService>(
                this.loggingBrokerMock.Object,
                this.dateTimeBrokerMock.Object,
                this.securityBrokerMock.Object,
                this.auditBrokerMock.Object,
                this.identifierBrokerMock.Object,
                this.patientServiceMock.Object,
                this.decisionServiceMock.Object,
                this.notificationServiceMock.Object,
                this.decisionConfigurations)
            { CallBase = true };

            this.patientServiceMock.Setup(service =>
                service.RetrieveAllPatientsAsync())
                    .ReturnsAsync(outputPatients.AsQueryable);

            this.identifierBrokerMock.Setup(broker =>
                broker.GetIdentifierAsync())
                    .ReturnsAsync(randomGuid);

            decisionOrchestrationServiceMock.Setup(service =>
                service.CheckIfIsAuthenticatedUserWithRequiredRoleAsync())
                    .ReturnsAsync(false);

            this.securityBrokerMock.Setup(broker =>
                broker.GetIpAddressAsync())
                    .ReturnsAsync(randomIpAddress);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTime);

            this.decisionServiceMock.Setup(service =>
                service.AddDecisionAsync(It.Is(SameDecisionAs(inputDecision))))
                    .ReturnsAsync(outputDecision);

            // when
            await decisionOrchestrationServiceMock.Object.VerifyAndRecordDecisionAsync(inputDecision);

            //then
            this.patientServiceMock.Verify(service =>
                service.RetrieveAllPatientsAsync(),
                    Times.Once);

            this.identifierBrokerMock.Verify(broker =>
                broker.GetIdentifierAsync(),
                    Times.Once);

            decisionOrchestrationServiceMock.Verify(service =>
                service.CheckIfIsAuthenticatedUserWithRequiredRoleAsync(),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetIpAddressAsync(),
                    Times.Once);

            this.auditBrokerMock.Verify(broker =>
                broker.LogInformationAsync(
                    "Decision",
                    "Verifying Decision",
                    $"Patient with IP address {randomIpAddress} is validating a code for patient {randomNhsNumber}.",
                    null,
                    randomGuid.ToString()),
                        Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once());

            this.decisionServiceMock.Verify(service =>
                service.AddDecisionAsync(It.Is(SameDecisionAs(inputDecision))),
                    Times.Once);

            this.notificationServiceMock.Verify(service =>
                service.SendSubmissionSuccessNotificationAsync(It.Is(SameNotificationInfoAs(inputNotificationInfo))),
                    Times.Once);

            this.auditBrokerMock.Verify(broker =>
                broker.LogInformationAsync(
                    "Decision",
                    "Decision Submitted",
                    "The patients decision has been succesfully submitted",
                    null,
                    randomGuid.ToString()),
                        Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.auditBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
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
            Guid randomGuid = Guid.NewGuid();
            User randomUser = CreateRandomUser();
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

            var decisionOrchestrationServiceMock = new Mock<DecisionOrchestrationService>(
                this.loggingBrokerMock.Object,
                this.dateTimeBrokerMock.Object,
                this.securityBrokerMock.Object,
                this.auditBrokerMock.Object,
                this.identifierBrokerMock.Object,
                this.patientServiceMock.Object,
                this.decisionServiceMock.Object,
                this.notificationServiceMock.Object,
                this.decisionConfigurations)
            { CallBase = true };

            this.patientServiceMock.Setup(service =>
                service.RetrieveAllPatientsAsync())
                    .ReturnsAsync(outputPatients.AsQueryable);

            this.identifierBrokerMock.Setup(broker =>
                broker.GetIdentifierAsync())
                    .ReturnsAsync(randomGuid);

            decisionOrchestrationServiceMock.Setup(service =>
                service.CheckIfIsAuthenticatedUserWithRequiredRoleAsync())
                    .ReturnsAsync(true);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomUser);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTime);

            this.decisionServiceMock.Setup(service =>
                service.AddDecisionAsync(It.Is(SameDecisionAs(inputDecision))))
                    .ReturnsAsync(outputDecision);

            // when
            await decisionOrchestrationServiceMock.Object.VerifyAndRecordDecisionAsync(inputDecision);

            //then
            this.patientServiceMock.Verify(service =>
                service.RetrieveAllPatientsAsync(),
                    Times.Once);

            this.identifierBrokerMock.Verify(broker =>
                broker.GetIdentifierAsync(),
                    Times.Once);

            decisionOrchestrationServiceMock.Verify(service =>
                service.CheckIfIsAuthenticatedUserWithRequiredRoleAsync(),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.auditBrokerMock.Verify(broker =>
                broker.LogInformationAsync(
                    "Decision",
                    "Verifying Decision",
                    $"User {randomUser.UserId} is verifying the decision for patient {randomNhsNumber}.",
                    null,
                    randomGuid.ToString()),
                        Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once());

            this.decisionServiceMock.Verify(service =>
                service.AddDecisionAsync(It.Is(SameDecisionAs(inputDecision))),
                    Times.Once);

            this.notificationServiceMock.Verify(service =>
                service.SendSubmissionSuccessNotificationAsync(It.Is(SameNotificationInfoAs(inputNotificationInfo))),
                    Times.Once);

            this.auditBrokerMock.Verify(broker =>
                broker.LogInformationAsync(
                    "Decision",
                    "Decision Submitted",
                    "The patients decision has been succesfully submitted",
                    null,
                    randomGuid.ToString()),
                        Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.auditBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
            this.patientServiceMock.VerifyNoOtherCalls();
            this.notificationServiceMock.VerifyNoOtherCalls();
            this.decisionServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldErrorOnVerifyAndRecordDecisionAsyncWithDecisionWorflowRoleUserAndNoMatchedOnDate()
        {
            // given
            string randomNhsNumber = GenerateRandom10DigitNumber();
            string randomValidationCode = GetRandomStringWithLengthOf(5);
            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();
            Guid randomGuid = Guid.NewGuid();
            User randomUser = CreateRandomUser();
            Patient randomPatient = GetRandomPatient(randomDateTime, randomNhsNumber, randomValidationCode);
            randomPatient.ValidationCodeMatchedOn = null;
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

            var decisionOrchestrationServiceMock = new Mock<DecisionOrchestrationService>(
                this.loggingBrokerMock.Object,
                this.dateTimeBrokerMock.Object,
                this.securityBrokerMock.Object,
                this.auditBrokerMock.Object,
                this.identifierBrokerMock.Object,
                this.patientServiceMock.Object,
                this.decisionServiceMock.Object,
                this.notificationServiceMock.Object,
                this.decisionConfigurations)
            { CallBase = true };

            this.patientServiceMock.Setup(service =>
                service.RetrieveAllPatientsAsync())
                    .ReturnsAsync(outputPatients.AsQueryable);

            this.identifierBrokerMock.Setup(broker =>
                broker.GetIdentifierAsync())
                    .ReturnsAsync(randomGuid);

            decisionOrchestrationServiceMock.Setup(service =>
                service.CheckIfIsAuthenticatedUserWithRequiredRoleAsync())
                    .ReturnsAsync(true);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomUser);

            var validationCodeNotMatchedException =
               new ValidationCodeNotMatchedException(
                   "The validation code for this patient has not been succesfully matched");

            var expectedDecisionOrchestrationValidationException =
                new DecisionOrchestrationValidationException(
                    message: "Decision orchestration validation error occurred, please fix the errors and try again.",
                    innerException: validationCodeNotMatchedException);

            // when
            ValueTask verifyAndRecordDecisionTask =
                decisionOrchestrationServiceMock.Object.VerifyAndRecordDecisionAsync(inputDecision);

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

            this.identifierBrokerMock.Verify(broker =>
                broker.GetIdentifierAsync(),
                    Times.Once);

            decisionOrchestrationServiceMock.Verify(service =>
                service.CheckIfIsAuthenticatedUserWithRequiredRoleAsync(),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.auditBrokerMock.Verify(broker =>
                broker.LogInformationAsync(
                    "Decision",
                    "Verifying Decision",
                    $"User {randomUser.UserId} is verifying the decision for patient {randomNhsNumber}.",
                    null,
                    randomGuid.ToString()),
                        Times.Once);

            this.auditBrokerMock.Verify(broker =>
                broker.LogInformationAsync(
                    "Decision",
                    "Decision Submission Failed",
                    "There was no matched validation code found for this patient.",
                    null,
                    randomGuid.ToString()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedDecisionOrchestrationValidationException))),
                       Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.auditBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
            this.patientServiceMock.VerifyNoOtherCalls();
            this.notificationServiceMock.VerifyNoOtherCalls();
            this.decisionServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldErrorOnVerifyAndRecordDecisionAsyncWithDecisionWorflowRoleUserAndMatchedOnDateExpired()
        {
            // given
            string randomNhsNumber = GenerateRandom10DigitNumber();
            string randomValidationCode = GetRandomStringWithLengthOf(5);
            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();

            DateTimeOffset expiredMatchedOn =
                randomDateTime.AddMinutes((-1 * this.decisionConfigurations.ValidatedCodeValidForMinutes) - 1);

            Guid randomGuid = Guid.NewGuid();
            User randomUser = CreateRandomUser();
            Patient randomPatient = GetRandomPatient(randomDateTime, randomNhsNumber, randomValidationCode);
            randomPatient.ValidationCodeMatchedOn = expiredMatchedOn;
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

            var decisionOrchestrationServiceMock = new Mock<DecisionOrchestrationService>(
                this.loggingBrokerMock.Object,
                this.dateTimeBrokerMock.Object,
                this.securityBrokerMock.Object,
                this.auditBrokerMock.Object,
                this.identifierBrokerMock.Object,
                this.patientServiceMock.Object,
                this.decisionServiceMock.Object,
                this.notificationServiceMock.Object,
                this.decisionConfigurations)
            { CallBase = true };

            this.patientServiceMock.Setup(service =>
                service.RetrieveAllPatientsAsync())
                    .ReturnsAsync(outputPatients.AsQueryable);

            this.identifierBrokerMock.Setup(broker =>
                broker.GetIdentifierAsync())
                    .ReturnsAsync(randomGuid);

            decisionOrchestrationServiceMock.Setup(service =>
                service.CheckIfIsAuthenticatedUserWithRequiredRoleAsync())
                    .ReturnsAsync(true);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomUser);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTime);

            var validationCodeMatchExpiredException =
               new ValidationCodeMatchExpiredException(
                   "The validation code for this patient has been matched but the matching period has now expired");

            var expectedDecisionOrchestrationValidationException =
                new DecisionOrchestrationValidationException(
                    message: "Decision orchestration validation error occurred, please fix the errors and try again.",
                    innerException: validationCodeMatchExpiredException);

            // when
            ValueTask verifyAndRecordDecisionTask =
                decisionOrchestrationServiceMock.Object.VerifyAndRecordDecisionAsync(inputDecision);

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

            this.identifierBrokerMock.Verify(broker =>
                broker.GetIdentifierAsync(),
                    Times.Once);

            decisionOrchestrationServiceMock.Verify(service =>
                service.CheckIfIsAuthenticatedUserWithRequiredRoleAsync(),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once());

            this.auditBrokerMock.Verify(broker =>
                broker.LogInformationAsync(
                    "Decision",
                    "Verifying Decision",
                    $"User {randomUser.UserId} is verifying the decision for patient {randomNhsNumber}.",
                    null,
                    randomGuid.ToString()),
                        Times.Once);

            this.auditBrokerMock.Verify(broker =>
                broker.LogInformationAsync(
                    "Decision",
                    "Decision Submission Failed",
                    "There was a matched validation code found but the matching period has now expired.",
                    null,
                    randomGuid.ToString()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedDecisionOrchestrationValidationException))),
                       Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.auditBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
            this.patientServiceMock.VerifyNoOtherCalls();
            this.notificationServiceMock.VerifyNoOtherCalls();
            this.decisionServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldErrorOnVerifyAndRecordDecisionAsyncWithNonDecisionWorflowRoleUserAndNoMatchedOnDate()
        {
            // given
            string randomNhsNumber = GenerateRandom10DigitNumber();
            string randomValidationCode = GetRandomStringWithLengthOf(5);
            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();
            Guid randomGuid = Guid.NewGuid();
            string randomIpAddress = GetRandomString();
            Patient randomPatient = GetRandomPatient(randomDateTime, randomNhsNumber, randomValidationCode);
            randomPatient.ValidationCodeMatchedOn = null;
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

            var decisionOrchestrationServiceMock = new Mock<DecisionOrchestrationService>(
                this.loggingBrokerMock.Object,
                this.dateTimeBrokerMock.Object,
                this.securityBrokerMock.Object,
                this.auditBrokerMock.Object,
                this.identifierBrokerMock.Object,
                this.patientServiceMock.Object,
                this.decisionServiceMock.Object,
                this.notificationServiceMock.Object,
                this.decisionConfigurations)
            { CallBase = true };

            this.patientServiceMock.Setup(service =>
                service.RetrieveAllPatientsAsync())
                    .ReturnsAsync(outputPatients.AsQueryable);

            this.identifierBrokerMock.Setup(broker =>
                broker.GetIdentifierAsync())
                    .ReturnsAsync(randomGuid);

            decisionOrchestrationServiceMock.Setup(service =>
                service.CheckIfIsAuthenticatedUserWithRequiredRoleAsync())
                    .ReturnsAsync(false);

            this.securityBrokerMock.Setup(broker =>
                broker.GetIpAddressAsync())
                    .ReturnsAsync(randomIpAddress);

            var validationCodeNotMatchedException =
               new ValidationCodeNotMatchedException(
                   "The validation code for this patient has not been succesfully matched");

            var expectedDecisionOrchestrationValidationException =
                new DecisionOrchestrationValidationException(
                    message: "Decision orchestration validation error occurred, please fix the errors and try again.",
                    innerException: validationCodeNotMatchedException);

            // when
            ValueTask verifyAndRecordDecisionTask =
                decisionOrchestrationServiceMock.Object.VerifyAndRecordDecisionAsync(inputDecision);

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

            this.identifierBrokerMock.Verify(broker =>
                broker.GetIdentifierAsync(),
                    Times.Once);

            decisionOrchestrationServiceMock.Verify(service =>
                service.CheckIfIsAuthenticatedUserWithRequiredRoleAsync(),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetIpAddressAsync(),
                    Times.Once);

            this.auditBrokerMock.Verify(broker =>
                broker.LogInformationAsync(
                    "Decision",
                    "Verifying Decision",
                    $"Patient with IP address {randomIpAddress} is validating a code for patient {randomNhsNumber}.",
                    null,
                    randomGuid.ToString()),
                        Times.Once);

            this.auditBrokerMock.Verify(broker =>
                broker.LogInformationAsync(
                    "Decision",
                    "Decision Submission Failed",
                    "There was no matched validation code found for this patient.",
                    null,
                    randomGuid.ToString()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedDecisionOrchestrationValidationException))),
                       Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.auditBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
            this.patientServiceMock.VerifyNoOtherCalls();
            this.notificationServiceMock.VerifyNoOtherCalls();
            this.decisionServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task
            ShouldErrorOnVerifyAndRecordDecisionAsyncWithNonDecisionWorflowRoleUserAndMatchedOnDateExpired()
        {
            // given
            string randomNhsNumber = GenerateRandom10DigitNumber();
            string randomValidationCode = GetRandomStringWithLengthOf(5);
            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();

            DateTimeOffset expiredMatchedOn =
                randomDateTime.AddMinutes((-1 * this.decisionConfigurations.ValidatedCodeValidForMinutes) - 1);

            Guid randomGuid = Guid.NewGuid();
            string randomIpAddress = GetRandomString();
            Patient randomPatient = GetRandomPatient(randomDateTime, randomNhsNumber, randomValidationCode);
            randomPatient.ValidationCodeMatchedOn = expiredMatchedOn;
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

            var decisionOrchestrationServiceMock = new Mock<DecisionOrchestrationService>(
                this.loggingBrokerMock.Object,
                this.dateTimeBrokerMock.Object,
                this.securityBrokerMock.Object,
                this.auditBrokerMock.Object,
                this.identifierBrokerMock.Object,
                this.patientServiceMock.Object,
                this.decisionServiceMock.Object,
                this.notificationServiceMock.Object,
                this.decisionConfigurations)
            { CallBase = true };

            this.patientServiceMock.Setup(service =>
                service.RetrieveAllPatientsAsync())
                    .ReturnsAsync(outputPatients.AsQueryable);

            this.identifierBrokerMock.Setup(broker =>
                broker.GetIdentifierAsync())
                    .ReturnsAsync(randomGuid);

            decisionOrchestrationServiceMock.Setup(service =>
                service.CheckIfIsAuthenticatedUserWithRequiredRoleAsync())
                    .ReturnsAsync(false);

            this.securityBrokerMock.Setup(broker =>
                broker.GetIpAddressAsync())
                    .ReturnsAsync(randomIpAddress);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTime);

            var validationCodeMatchExpiredException =
               new ValidationCodeMatchExpiredException(
                   "The validation code for this patient has been matched but the matching period has now expired");

            var expectedDecisionOrchestrationValidationException =
                new DecisionOrchestrationValidationException(
                    message: "Decision orchestration validation error occurred, please fix the errors and try again.",
                    innerException: validationCodeMatchExpiredException);

            // when
            ValueTask verifyAndRecordDecisionTask =
                decisionOrchestrationServiceMock.Object.VerifyAndRecordDecisionAsync(inputDecision);

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

            this.identifierBrokerMock.Verify(broker =>
                broker.GetIdentifierAsync(),
                    Times.Once);

            decisionOrchestrationServiceMock.Verify(service =>
                service.CheckIfIsAuthenticatedUserWithRequiredRoleAsync(),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetIpAddressAsync(),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once());

            this.auditBrokerMock.Verify(broker =>
                broker.LogInformationAsync(
                    "Decision",
                    "Verifying Decision",
                    $"Patient with IP address {randomIpAddress} is validating a code for patient {randomNhsNumber}.",
                    null,
                    randomGuid.ToString()),
                        Times.Once);

            this.auditBrokerMock.Verify(broker =>
                broker.LogInformationAsync(
                    "Decision",
                    "Decision Submission Failed",
                    "There was a matched validation code found but the matching period has now expired.",
                    null,
                    randomGuid.ToString()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedDecisionOrchestrationValidationException))),
                       Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.auditBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
            this.patientServiceMock.VerifyNoOtherCalls();
            this.notificationServiceMock.VerifyNoOtherCalls();
            this.decisionServiceMock.VerifyNoOtherCalls();
        }
    }
}
