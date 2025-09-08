// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Core.Models.Foundations.Decisions;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using LondonDataServices.IDecide.Core.Models.Orchestrations.Decisions.Exceptions;
using Moq;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Orchestrations.Decisions
{
    public partial class DecisionOrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnVerifyAndRecordDecisionAsyncWithNullDecision()
        {
            // given
            Decision nullDecision = null;
            var nullDecisionException = new NullDecisionException("Decision is null.");

            var expectedDecisionOrchestrationValidationException =
                new DecisionOrchestrationValidationException(
                    message: "Decision orchestration validation error occurred, please fix the errors and try again.",
                    innerException: nullDecisionException);

            this.patientServiceMock.Setup(service =>
                service.RetrieveAllPatientsAsync())
                    .ThrowsAsync(nullDecisionException);

            // when
            ValueTask verifyAndRecordDecisionTask =
               this.decisionOrchestrationService
                   .VerifyAndRecordDecisionAsync(nullDecision);

            DecisionOrchestrationValidationException
                actualPatientOrchestrationValidationException =
                    await Assert.ThrowsAsync<DecisionOrchestrationValidationException>(
                        testCode: verifyAndRecordDecisionTask.AsTask);

            // then
            actualPatientOrchestrationValidationException
                .Should().BeEquivalentTo(expectedDecisionOrchestrationValidationException);

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
        public async Task ShouldThrowValidationExceptionOnVerifyAndRecordDecisionAsyncWithNullDecisionPatient()
        {
            // given
            Decision randomDecision = GetRandomDecisionWithNullPatient();

            var invalidDecisionOrchestrationArgumentException =
                new InvalidDecisionOrchestrationArgumentException(
                    "Invalid decision orchestration argument. Please correct the errors and try again.");

            invalidDecisionOrchestrationArgumentException.AddData(
                key: "Patient",
                values: "Patient cannot be null.");

            invalidDecisionOrchestrationArgumentException.AddData(
                key: "NhsNumber",
                values: "Text must be exactly 10 digits.");

            invalidDecisionOrchestrationArgumentException.AddData(
                key: "ValidationCode",
                values: "Code must be 5 characters long.");

            var expectedDecisionOrchestrationValidationException =
                new DecisionOrchestrationValidationException(
                    message: "Decision orchestration validation error occurred, please fix the errors and try again.",
                    innerException: invalidDecisionOrchestrationArgumentException);

            this.patientServiceMock.Setup(service =>
                service.RetrieveAllPatientsAsync())
                    .ThrowsAsync(invalidDecisionOrchestrationArgumentException);

            // when
            ValueTask verifyAndRecordDecisionTask =
               this.decisionOrchestrationService
                   .VerifyAndRecordDecisionAsync(randomDecision);

            DecisionOrchestrationValidationException
                actualPatientOrchestrationValidationException =
                    await Assert.ThrowsAsync<DecisionOrchestrationValidationException>(
                        testCode: verifyAndRecordDecisionTask.AsTask);

            // then
            actualPatientOrchestrationValidationException
                .Should().BeEquivalentTo(expectedDecisionOrchestrationValidationException);

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

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("123456789")]
        [InlineData("01234567890")]
        [InlineData("a123456789")]
        public async Task ShouldThrowValidationExceptionOnVerifyAndRecordDecisionAsyncWithInvalidNhsNumber(
            string invalidNhsNumber)
        {
            // given
            string randomValidationCode = GetRandomStringWithLengthOf(5);
            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();
            Patient randomPatient = GetRandomPatient(randomDateTime, invalidNhsNumber, randomValidationCode);
            Decision randomDecision = GetRandomDecision(randomPatient);

            var invalidDecisionOrchestrationArgumentException =
                new InvalidDecisionOrchestrationArgumentException(
                    "Invalid decision orchestration argument. Please correct the errors and try again.");

            invalidDecisionOrchestrationArgumentException.AddData(
                key: "NhsNumber",
                values: "Text must be exactly 10 digits.");

            invalidDecisionOrchestrationArgumentException.AddData(
                key: "PatientNhsNumber",
                values: "Text must be exactly 10 digits.");

            var expectedDecisionOrchestrationValidationException =
                new DecisionOrchestrationValidationException(
                    message: "Decision orchestration validation error occurred, please fix the errors and try again.",
                    innerException: invalidDecisionOrchestrationArgumentException);

            this.patientServiceMock.Setup(service =>
                service.RetrieveAllPatientsAsync())
                    .ThrowsAsync(invalidDecisionOrchestrationArgumentException);

            // when
            ValueTask verifyAndRecordDecisionTask =
               this.decisionOrchestrationService
                   .VerifyAndRecordDecisionAsync(randomDecision);

            DecisionOrchestrationValidationException
                actualPatientOrchestrationValidationException =
                    await Assert.ThrowsAsync<DecisionOrchestrationValidationException>(
                        testCode: verifyAndRecordDecisionTask.AsTask);

            // then
            actualPatientOrchestrationValidationException
                .Should().BeEquivalentTo(expectedDecisionOrchestrationValidationException);

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

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("1234")]
        [InlineData("123456")]
        public async Task ShouldThrowValidationExceptionOnVerifyAndRecordDecisionAsyncWithInvalidValidationCode(
            string invalidValidationCode)
        {
            // given
            string randomNhsNumber = GenerateRandom10DigitNumber();
            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();
            Patient randomPatient = GetRandomPatient(randomDateTime, randomNhsNumber, invalidValidationCode);
            Decision randomDecision = GetRandomDecision(randomPatient);

            var invalidDecisionOrchestrationArgumentException =
                new InvalidDecisionOrchestrationArgumentException(
                    "Invalid decision orchestration argument. Please correct the errors and try again.");

            invalidDecisionOrchestrationArgumentException.AddData(
                key: "ValidationCode",
                values: "Code must be 5 characters long.");

            var expectedDecisionOrchestrationValidationException =
                new DecisionOrchestrationValidationException(
                    message: "Decision orchestration validation error occurred, please fix the errors and try again.",
                    innerException: invalidDecisionOrchestrationArgumentException);

            this.patientServiceMock.Setup(service =>
                service.RetrieveAllPatientsAsync())
                    .ThrowsAsync(invalidDecisionOrchestrationArgumentException);

            // when
            ValueTask verifyAndRecordDecisionTask =
               this.decisionOrchestrationService
                   .VerifyAndRecordDecisionAsync(randomDecision);

            DecisionOrchestrationValidationException
                actualPatientOrchestrationValidationException =
                    await Assert.ThrowsAsync<DecisionOrchestrationValidationException>(
                        testCode: verifyAndRecordDecisionTask.AsTask);

            // then
            actualPatientOrchestrationValidationException
                .Should().BeEquivalentTo(expectedDecisionOrchestrationValidationException);

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
        public async Task ShouldThrowValidationExceptionOnVerifyAndRecordDecisionAsyncWithExceededRetries()
        {
            // given
            string randomNhsNumber = GenerateRandom10DigitNumber();
            string randomValidationCode = GetRandomStringWithLengthOf(5);
            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();
            Patient randomPatient = GetRandomPatient(randomDateTime, randomNhsNumber, randomValidationCode);
            Decision randomDecision = GetRandomDecision(randomPatient);

            var exceededMaxRetryCountException =
                new ExceededMaxRetryCountException(
                    $"The maximum retry count of {this.decisionConfigurations.MaxRetryCount} exceeded.");

            var expectedDecisionOrchestrationValidationException =
                new DecisionOrchestrationValidationException(
                    message: "Decision orchestration validation error occurred, please fix the errors and try again.",
                    innerException: exceededMaxRetryCountException);

            this.patientServiceMock.Setup(service =>
                service.RetrieveAllPatientsAsync())
                    .ThrowsAsync(exceededMaxRetryCountException);

            // when
            ValueTask verifyAndRecordDecisionTask =
               this.decisionOrchestrationService
                   .VerifyAndRecordDecisionAsync(randomDecision);

            DecisionOrchestrationValidationException
                actualPatientOrchestrationValidationException =
                    await Assert.ThrowsAsync<DecisionOrchestrationValidationException>(
                        testCode: verifyAndRecordDecisionTask.AsTask);

            // then
            actualPatientOrchestrationValidationException
                .Should().BeEquivalentTo(expectedDecisionOrchestrationValidationException);

            this.patientServiceMock.Verify(service =>
                service.RetrieveAllPatientsAsync(),
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
        public async Task ShouldThrowValidationExceptionOnVerifyAndRecordDecisionAsyncWithExpiredValidationCode()
        {
            // given
            string randomNhsNumber = GenerateRandom10DigitNumber();
            string randomValidationCode = GetRandomStringWithLengthOf(5);
            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();
            Patient randomPatient = GetRandomPatient(randomDateTime, randomNhsNumber, randomValidationCode);
            Decision randomDecision = GetRandomDecision(randomPatient);

            var renewedValidationCodeException =
                new RenewedValidationCodeException("The validation code has expired, but we have issued a new code " +
                $"that will be sent via {randomDecision.Patient.NotificationPreference.ToString()}");

            var expectedDecisionOrchestrationValidationException =
                new DecisionOrchestrationValidationException(
                    message: "Decision orchestration validation error occurred, please fix the errors and try again.",
                    innerException: renewedValidationCodeException);

            this.patientServiceMock.Setup(service =>
                service.RetrieveAllPatientsAsync())
                    .ThrowsAsync(renewedValidationCodeException);

            // when
            ValueTask verifyAndRecordDecisionTask =
               this.decisionOrchestrationService
                   .VerifyAndRecordDecisionAsync(randomDecision);

            DecisionOrchestrationValidationException
                actualPatientOrchestrationValidationException =
                    await Assert.ThrowsAsync<DecisionOrchestrationValidationException>(
                        testCode: verifyAndRecordDecisionTask.AsTask);

            // then
            actualPatientOrchestrationValidationException
                .Should().BeEquivalentTo(expectedDecisionOrchestrationValidationException);

            this.patientServiceMock.Verify(service =>
                service.RetrieveAllPatientsAsync(),
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
        public async Task ShouldThrowValidationExceptionOnVerifyAndRecordDecisionAsyncWithIncorrectValidationCode()
        {
            // given
            string randomNhsNumber = GenerateRandom10DigitNumber();
            string randomValidationCode = GetRandomStringWithLengthOf(5);
            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();
            Patient randomPatient = GetRandomPatient(randomDateTime, randomNhsNumber, randomValidationCode);
            Decision randomDecision = GetRandomDecision(randomPatient);

            var incorrectValidationCodeException =
                new IncorrectValidationCodeException("The validation code provided is incorrect.");

            var expectedDecisionOrchestrationValidationException =
                new DecisionOrchestrationValidationException(
                    message: "Decision orchestration validation error occurred, please fix the errors and try again.",
                    innerException: incorrectValidationCodeException);

            this.patientServiceMock.Setup(service =>
                service.RetrieveAllPatientsAsync())
                    .ThrowsAsync(incorrectValidationCodeException);

            // when
            ValueTask verifyAndRecordDecisionTask =
               this.decisionOrchestrationService
                   .VerifyAndRecordDecisionAsync(randomDecision);

            DecisionOrchestrationValidationException
                actualPatientOrchestrationValidationException =
                    await Assert.ThrowsAsync<DecisionOrchestrationValidationException>(
                        testCode: verifyAndRecordDecisionTask.AsTask);

            // then
            actualPatientOrchestrationValidationException
                .Should().BeEquivalentTo(expectedDecisionOrchestrationValidationException);

            this.patientServiceMock.Verify(service =>
                service.RetrieveAllPatientsAsync(),
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
