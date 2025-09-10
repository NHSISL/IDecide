// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using LondonDataServices.IDecide.Core.Models.Orchestrations.Patients.Exceptions;
using LondonDataServices.IDecide.Core.Models.Securities;
using LondonDataServices.IDecide.Core.Services.Orchestrations.Patients;
using Moq;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Orchestrations.Patients
{
    public partial class PatientOrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldVerifyPatientCodeAsyncWithNonDecisionWorflowRoleUser()
        {
            // given
            Guid randomGuid = Guid.NewGuid();
            string randomNhsNumber = GenerateRandom10DigitNumber();
            string randomValidationCode = GetRandomStringWithLengthOf(5);
            string randomIpAddress = GetRandomString();
            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();

            Patient randomPatient = GetRandomPatient(
                validationCodeExpiresOn: randomDateTime,
                inputNhsNumber: randomNhsNumber,
                validationCode: randomValidationCode);

            List<Patient> randomPatients = GetRandomPatients(randomDateTime);
            randomPatients.Add(randomPatient);
            List<Patient> outputPatients = randomPatients.DeepClone();
            Patient patientToUpdate = randomPatient.DeepClone();
            patientToUpdate.ValidationCodeMatchedOn = randomDateTime;

            var patientOrchestrationServiceMock = new Mock<PatientOrchestrationService>(
                this.loggingBrokerMock.Object,
                this.securityBrokerMock.Object,
                this.dateTimeBrokerMock.Object,
                this.auditBrokerMock.Object,
                this.identifierBrokerMock.Object,
                this.pdsServiceMock.Object,
                this.patientServiceMock.Object,
                this.notificationServiceMock.Object,
                this.decisionConfigurations)
            { CallBase = true };

            patientOrchestrationServiceMock.Setup(service =>
                service.CheckIfIsAuthenticatedUserWithRequiredRoleAsync())
                    .ReturnsAsync(false);

            this.patientServiceMock.Setup(service =>
                service.RetrieveAllPatientsAsync())
                    .ReturnsAsync(outputPatients.AsQueryable);

            this.identifierBrokerMock.Setup(broker =>
                broker.GetIdentifierAsync())
                    .ReturnsAsync(randomGuid);

            this.securityBrokerMock.Setup(broker =>
                broker.GetIpAddressAsync())
                    .ReturnsAsync(randomIpAddress);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTime);

            // when
            await patientOrchestrationServiceMock.Object.VerifyPatientCodeAsync(randomNhsNumber, randomValidationCode);

            //then
            patientOrchestrationServiceMock.Verify(service =>
                service.CheckIfIsAuthenticatedUserWithRequiredRoleAsync(),
                    Times.Once);

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
                    "Patient Code",
                    "Validating Patient Code",
                    $"Patient with IP address {randomIpAddress} is validating a code for patient {randomNhsNumber}.",
                    null,
                    randomGuid.ToString()),
                        Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Exactly(2));

            this.patientServiceMock.Verify(service =>
                service.ModifyPatientAsync(It.Is(SamePatientAs(patientToUpdate))),
                    Times.Once);

            this.auditBrokerMock.Verify(broker =>
                broker.LogInformationAsync(
                    "Patient Code",
                    "Patient Code Validation Succeeded",
                    "The validation code provided was valid and successfully verified.",
                    null,
                    randomGuid.ToString()),
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

        [Fact]
        public async Task ShouldVerifyPatientCodeAsyncWithDecisionWorflowRoleUser()
        {
            // given
            Guid randomGuid = Guid.NewGuid();
            string randomNhsNumber = GenerateRandom10DigitNumber();
            string randomValidationCode = GetRandomStringWithLengthOf(5);
            string randomIpAddress = GetRandomString();
            User randomUser = CreateRandomUser();
            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();
            this.decisionConfigurations.AgentOverrideCode = randomValidationCode;

            Patient randomPatient = GetRandomPatient(
                validationCodeExpiresOn: randomDateTime,
                inputNhsNumber: randomNhsNumber,
                validationCode: randomValidationCode);

            List<Patient> randomPatients = GetRandomPatients(randomDateTime);
            randomPatients.Add(randomPatient);
            List<Patient> outputPatients = randomPatients.DeepClone();
            Patient patientToUpdate = randomPatient.DeepClone();
            patientToUpdate.ValidationCodeMatchedOn = randomDateTime;

            var patientOrchestrationServiceMock = new Mock<PatientOrchestrationService>(
                this.loggingBrokerMock.Object,
                this.securityBrokerMock.Object,
                this.dateTimeBrokerMock.Object,
                this.auditBrokerMock.Object,
                this.identifierBrokerMock.Object,
                this.pdsServiceMock.Object,
                this.patientServiceMock.Object,
                this.notificationServiceMock.Object,
                this.decisionConfigurations)
            { CallBase = true };

            patientOrchestrationServiceMock.Setup(service =>
                service.CheckIfIsAuthenticatedUserWithRequiredRoleAsync())
                    .ReturnsAsync(true);

            this.patientServiceMock.Setup(service =>
                service.RetrieveAllPatientsAsync())
                    .ReturnsAsync(outputPatients.AsQueryable);

            this.identifierBrokerMock.Setup(broker =>
                broker.GetIdentifierAsync())
                    .ReturnsAsync(randomGuid);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomUser);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTime);

            // when
            await patientOrchestrationServiceMock.Object.VerifyPatientCodeAsync(randomNhsNumber, randomValidationCode);

            //then
            patientOrchestrationServiceMock.Verify(service =>
                service.CheckIfIsAuthenticatedUserWithRequiredRoleAsync(),
                    Times.Once);

            this.patientServiceMock.Verify(service =>
                service.RetrieveAllPatientsAsync(),
                    Times.Once);

            this.identifierBrokerMock.Verify(broker =>
                broker.GetIdentifierAsync(),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.auditBrokerMock.Verify(broker =>
                broker.LogInformationAsync(
                    "Patient Code",
                    "Validating Patient Code",
                    $"User {randomUser.UserId} is validating a code for patient {randomNhsNumber}.",
                    null,
                    randomGuid.ToString()),
                        Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.patientServiceMock.Verify(service =>
                service.ModifyPatientAsync(It.Is(SamePatientAs(patientToUpdate))),
                    Times.Once);

            this.auditBrokerMock.Verify(broker =>
                broker.LogInformationAsync(
                    "Patient Code",
                    "Patient Code Validation Succeeded",
                    "The validation code provided was valid and successfully verified.",
                    null,
                    randomGuid.ToString()),
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

        [Fact]
        public async Task ShouldErrorOnVerifyPatientCodeAsyncWithDecisionWorflowRoleUserAndInvalidCode()
        {
            // given
            Guid randomGuid = Guid.NewGuid();
            string randomNhsNumber = GenerateRandom10DigitNumber();
            string randomValidationCode = GetRandomStringWithLengthOf(5);
            string inputValidationCode = GetRandomStringWithLengthOf(5);
            string randomIpAddress = GetRandomString();
            User randomUser = CreateRandomUser();
            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();
            this.decisionConfigurations.AgentOverrideCode = randomValidationCode;

            Patient randomPatient = GetRandomPatient(
                validationCodeExpiresOn: randomDateTime,
                inputNhsNumber: randomNhsNumber,
                validationCode: randomValidationCode);

            List<Patient> randomPatients = GetRandomPatients(randomDateTime);
            randomPatients.Add(randomPatient);
            List<Patient> outputPatients = randomPatients.DeepClone();

            var patientOrchestrationServiceMock = new Mock<PatientOrchestrationService>(
                this.loggingBrokerMock.Object,
                this.securityBrokerMock.Object,
                this.dateTimeBrokerMock.Object,
                this.auditBrokerMock.Object,
                this.identifierBrokerMock.Object,
                this.pdsServiceMock.Object,
                this.patientServiceMock.Object,
                this.notificationServiceMock.Object,
                this.decisionConfigurations)
            { CallBase = true };

            patientOrchestrationServiceMock.Setup(service =>
                service.CheckIfIsAuthenticatedUserWithRequiredRoleAsync())
                    .ReturnsAsync(true);

            this.patientServiceMock.Setup(service =>
                service.RetrieveAllPatientsAsync())
                    .ReturnsAsync(outputPatients.AsQueryable);

            this.identifierBrokerMock.Setup(broker =>
                broker.GetIdentifierAsync())
                    .ReturnsAsync(randomGuid);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomUser);

            var incorrectValidationCodeException =
               new IncorrectValidationCodeException("The validation code provided is incorrect.");

            var expectedPatientOrchestrationValidationException =
                new PatientOrchestrationValidationException(
                    message: "Patient orchestration validation error occurred, please fix the errors and try again.",
                    innerException: incorrectValidationCodeException);

            // when
            ValueTask verifyPatientCodeTask =
                patientOrchestrationServiceMock.Object.VerifyPatientCodeAsync(randomNhsNumber, inputValidationCode);

            PatientOrchestrationValidationException
                actualPatientOrchestrationValidationException =
                    await Assert.ThrowsAsync<PatientOrchestrationValidationException>(
                        testCode: verifyPatientCodeTask.AsTask);

            //then
            actualPatientOrchestrationValidationException
                .Should().BeEquivalentTo(expectedPatientOrchestrationValidationException);

            patientOrchestrationServiceMock.Verify(service =>
                service.CheckIfIsAuthenticatedUserWithRequiredRoleAsync(),
                    Times.Once);

            this.patientServiceMock.Verify(service =>
                service.RetrieveAllPatientsAsync(),
                    Times.Once);

            this.identifierBrokerMock.Verify(broker =>
                broker.GetIdentifierAsync(),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.auditBrokerMock.Verify(broker =>
                broker.LogInformationAsync(
                    "Patient Code",
                    "Validating Patient Code",
                    $"User {randomUser.UserId} is validating a code for patient {randomNhsNumber}.",
                    null,
                    randomGuid.ToString()),
                        Times.Once);

            this.auditBrokerMock.Verify(broker =>
                broker.LogInformationAsync(
                    "Patient Code",
                    "Patient Code Validation Failed",
                    "The validation code provided was incorrect.",
                    null,
                    randomGuid.ToString()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedPatientOrchestrationValidationException))),
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

        [Fact]
        public async Task ShouldErrorOnVerifyPatientCodeAsyncWithNonDecisionWorflowRoleUserExceedingMaxRetry()
        {
            // given
            Guid randomGuid = Guid.NewGuid();
            string randomNhsNumber = GenerateRandom10DigitNumber();
            string randomValidationCode = GetRandomStringWithLengthOf(5);
            string randomIpAddress = GetRandomString();
            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();
            int exceedMaxRetryCount = this.decisionConfigurations.MaxRetryCount + 1;

            Patient randomPatient = GetRandomPatient(
                validationCodeExpiresOn: randomDateTime,
                inputNhsNumber: randomNhsNumber,
                validationCode: randomValidationCode,
                retryCount: exceedMaxRetryCount);

            List<Patient> randomPatients = GetRandomPatients(randomDateTime);
            randomPatients.Add(randomPatient);
            List<Patient> outputPatients = randomPatients.DeepClone();

            var patientOrchestrationServiceMock = new Mock<PatientOrchestrationService>(
                this.loggingBrokerMock.Object,
                this.securityBrokerMock.Object,
                this.dateTimeBrokerMock.Object,
                this.auditBrokerMock.Object,
                this.identifierBrokerMock.Object,
                this.pdsServiceMock.Object,
                this.patientServiceMock.Object,
                this.notificationServiceMock.Object,
                this.decisionConfigurations)
            { CallBase = true };

            patientOrchestrationServiceMock.Setup(service =>
                service.CheckIfIsAuthenticatedUserWithRequiredRoleAsync())
                    .ReturnsAsync(false);

            this.patientServiceMock.Setup(service =>
                service.RetrieveAllPatientsAsync())
                    .ReturnsAsync(outputPatients.AsQueryable);

            this.identifierBrokerMock.Setup(broker =>
                broker.GetIdentifierAsync())
                    .ReturnsAsync(randomGuid);

            this.securityBrokerMock.Setup(broker =>
                broker.GetIpAddressAsync())
                    .ReturnsAsync(randomIpAddress);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTime);

            var exceededMaxRetryCountException =
               new ExceededMaxRetryCountException(
                   $"The maximum retry count of {this.decisionConfigurations.MaxRetryCount} exceeded.");

            var expectedPatientOrchestrationValidationException =
                new PatientOrchestrationValidationException(
                    message: "Patient orchestration validation error occurred, please fix the errors and try again.",
                    innerException: exceededMaxRetryCountException);

            // when
            ValueTask verifyPatientCodeTask =
                patientOrchestrationServiceMock.Object.VerifyPatientCodeAsync(randomNhsNumber, randomValidationCode);

            PatientOrchestrationValidationException
                actualPatientOrchestrationValidationException =
                    await Assert.ThrowsAsync<PatientOrchestrationValidationException>(
                        testCode: verifyPatientCodeTask.AsTask);

            //then
            actualPatientOrchestrationValidationException
                .Should().BeEquivalentTo(expectedPatientOrchestrationValidationException);

            patientOrchestrationServiceMock.Verify(service =>
                service.CheckIfIsAuthenticatedUserWithRequiredRoleAsync(),
                    Times.Once);

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
                    "Patient Code",
                    "Validating Patient Code",
                    $"Patient with IP address {randomIpAddress} is validating a code for patient {randomNhsNumber}.",
                    null,
                    randomGuid.ToString()),
                        Times.Once);

            this.auditBrokerMock.Verify(broker =>
                broker.LogInformationAsync(
                    "Patient Code",
                    "Patient Code Validation Failed",

                    $"The maximum retry count of {this.decisionConfigurations.MaxRetryCount} exceeded " +
                        $"for patient {randomNhsNumber}",

                    null,
                    randomGuid.ToString()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedPatientOrchestrationValidationException))),
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

        [Fact]
        public async Task ShouldErrorOnVerifyPatientCodeAsyncWithNonDecisionWorflowRoleUserWrongValidationCode()
        {
            // given
            Guid randomGuid = Guid.NewGuid();
            string randomNhsNumber = GenerateRandom10DigitNumber();
            string randomValidationCode = GetRandomStringWithLengthOf(5);
            string inputValidationCode = GetRandomStringWithLengthOf(5);
            string randomIpAddress = GetRandomString();
            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();

            Patient randomPatient = GetRandomPatient(
                validationCodeExpiresOn: randomDateTime,
                inputNhsNumber: randomNhsNumber,
                validationCode: randomValidationCode);

            List<Patient> randomPatients = GetRandomPatients(randomDateTime);
            randomPatients.Add(randomPatient);
            List<Patient> outputPatients = randomPatients.DeepClone();
            Patient patientToUpdate = randomPatient.DeepClone();
            patientToUpdate.RetryCount += 1;

            var patientOrchestrationServiceMock = new Mock<PatientOrchestrationService>(
                this.loggingBrokerMock.Object,
                this.securityBrokerMock.Object,
                this.dateTimeBrokerMock.Object,
                this.auditBrokerMock.Object,
                this.identifierBrokerMock.Object,
                this.pdsServiceMock.Object,
                this.patientServiceMock.Object,
                this.notificationServiceMock.Object,
                this.decisionConfigurations)
            { CallBase = true };

            patientOrchestrationServiceMock.Setup(service =>
                service.CheckIfIsAuthenticatedUserWithRequiredRoleAsync())
                    .ReturnsAsync(false);

            this.patientServiceMock.Setup(service =>
                service.RetrieveAllPatientsAsync())
                    .ReturnsAsync(outputPatients.AsQueryable);

            this.identifierBrokerMock.Setup(broker =>
                broker.GetIdentifierAsync())
                    .ReturnsAsync(randomGuid);

            this.securityBrokerMock.Setup(broker =>
                broker.GetIpAddressAsync())
                    .ReturnsAsync(randomIpAddress);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTime);

            var incorrectValidationCodeException =
               new IncorrectValidationCodeException("The validation code provided is incorrect.");

            var expectedPatientOrchestrationValidationException =
                new PatientOrchestrationValidationException(
                    message: "Patient orchestration validation error occurred, please fix the errors and try again.",
                    innerException: incorrectValidationCodeException);

            // when
            ValueTask verifyPatientCodeTask =
                patientOrchestrationServiceMock.Object.VerifyPatientCodeAsync(randomNhsNumber, inputValidationCode);

            PatientOrchestrationValidationException
                actualPatientOrchestrationValidationException =
                    await Assert.ThrowsAsync<PatientOrchestrationValidationException>(
                        testCode: verifyPatientCodeTask.AsTask);

            //then
            actualPatientOrchestrationValidationException
                .Should().BeEquivalentTo(expectedPatientOrchestrationValidationException);

            patientOrchestrationServiceMock.Verify(service =>
                service.CheckIfIsAuthenticatedUserWithRequiredRoleAsync(),
                    Times.Once);

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
                    "Patient Code",
                    "Validating Patient Code",
                    $"Patient with IP address {randomIpAddress} is validating a code for patient {randomNhsNumber}.",
                    null,
                    randomGuid.ToString()),
                        Times.Once);

            this.patientServiceMock.Verify(service =>
                service.ModifyPatientAsync(It.Is(SamePatientAs(patientToUpdate))),
                    Times.Once);

            this.auditBrokerMock.Verify(broker =>
                broker.LogInformationAsync(
                    "Patient Code",
                    "Patient Code Validation Failed",
                    "The validation code provided was incorrect.",
                    null,
                    randomGuid.ToString()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedPatientOrchestrationValidationException))),
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

        [Fact]
        public async Task ShouldErrorOnVerifyPatientCodeAsyncWithNonDecisionWorflowRoleUserExpiredValidationCode()
        {
            // given
            Guid randomGuid = Guid.NewGuid();
            string randomNhsNumber = GenerateRandom10DigitNumber();
            string randomValidationCode = GetRandomStringWithLengthOf(5);
            string newValidationCode = GetRandomStringWithLengthOf(5);
            string randomIpAddress = GetRandomString();
            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();
            DateTimeOffset expiredDateTime = randomDateTime.AddDays(-1);

            Patient randomPatient = GetRandomPatient(
                validationCodeExpiresOn: expiredDateTime,
                inputNhsNumber: randomNhsNumber,
                validationCode: randomValidationCode);

            List<Patient> randomPatients = GetRandomPatients(randomDateTime);
            randomPatients.Add(randomPatient);
            List<Patient> outputPatients = randomPatients.DeepClone();
            Patient patientToUpdate = randomPatient.DeepClone();
            patientToUpdate.RetryCount = 0;
            patientToUpdate.ValidationCode = newValidationCode;
            patientToUpdate.ValidationCodeMatchedOn = null;

            patientToUpdate.ValidationCodeExpiresOn =
                randomDateTime.AddMinutes(decisionConfigurations.PatientValidationCodeExpireAfterMinutes);

            var patientOrchestrationServiceMock = new Mock<PatientOrchestrationService>(
                this.loggingBrokerMock.Object,
                this.securityBrokerMock.Object,
                this.dateTimeBrokerMock.Object,
                this.auditBrokerMock.Object,
                this.identifierBrokerMock.Object,
                this.pdsServiceMock.Object,
                this.patientServiceMock.Object,
                this.notificationServiceMock.Object,
                this.decisionConfigurations)
            { CallBase = true };

            patientOrchestrationServiceMock.Setup(service =>
                service.CheckIfIsAuthenticatedUserWithRequiredRoleAsync())
                    .ReturnsAsync(false);

            this.patientServiceMock.Setup(service =>
                service.RetrieveAllPatientsAsync())
                    .ReturnsAsync(outputPatients.AsQueryable);

            this.identifierBrokerMock.Setup(broker =>
                broker.GetIdentifierAsync())
                    .ReturnsAsync(randomGuid);

            this.securityBrokerMock.Setup(broker =>
                broker.GetIpAddressAsync())
                    .ReturnsAsync(randomIpAddress);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTime);

            this.patientServiceMock.Setup(service =>
                service.GenerateValidationCodeAsync())
                    .ReturnsAsync(newValidationCode);

            var renewedValidationCodeException =
               new RenewedValidationCodeException(
                   "The validation code has expired, but we have issued a new code that will be sent via " +
                        "your prefered contact method");

            var expectedPatientOrchestrationValidationException =
                new PatientOrchestrationValidationException(
                    message: "Patient orchestration validation error occurred, please fix the errors and try again.",
                    innerException: renewedValidationCodeException);

            // when
            ValueTask verifyPatientCodeTask =
                patientOrchestrationServiceMock.Object.VerifyPatientCodeAsync(randomNhsNumber, randomValidationCode);

            PatientOrchestrationValidationException
                actualPatientOrchestrationValidationException =
                    await Assert.ThrowsAsync<PatientOrchestrationValidationException>(
                        testCode: verifyPatientCodeTask.AsTask);

            //then
            actualPatientOrchestrationValidationException
                .Should().BeEquivalentTo(expectedPatientOrchestrationValidationException);

            patientOrchestrationServiceMock.Verify(service =>
                service.CheckIfIsAuthenticatedUserWithRequiredRoleAsync(),
                    Times.Once);

            this.patientServiceMock.Verify(service =>
                service.RetrieveAllPatientsAsync(),
                    Times.Once);

            this.identifierBrokerMock.Verify(broker =>
                broker.GetIdentifierAsync(),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetIpAddressAsync(),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.auditBrokerMock.Verify(broker =>
                broker.LogInformationAsync(
                    "Patient Code",
                    "Validating Patient Code",
                    $"Patient with IP address {randomIpAddress} is validating a code for patient {randomNhsNumber}.",
                    null,
                    randomGuid.ToString()),
                        Times.Once);

            this.patientServiceMock.Verify(service =>
                service.ModifyPatientAsync(It.Is(SamePatientAs(patientToUpdate))),
                    Times.Once);

            this.auditBrokerMock.Verify(broker =>
                broker.LogInformationAsync(
                    "Patient Code",
                    "New Validation Code Generated",
                    "The validation code was expired so a new code was issued.",
                    null,
                    randomGuid.ToString()),
                        Times.Once);

            this.patientServiceMock.Verify(service =>
                service.GenerateValidationCodeAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedPatientOrchestrationValidationException))),
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
