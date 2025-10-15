// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Core.Models.Orchestrations.Patients.Exceptions;
using LondonDataServices.IDecide.Core.Services.Orchestrations.Patients;
using Moq;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Orchestrations.Patients
{
    public partial class PatientOrchestrationServiceTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("123456789")]
        [InlineData("01234567890")]
        [InlineData("a123456789")]
        public async Task ShouldThrowValidationExceptionOnVerifyPatientCodeWithInvalidNhsNumber(
            string invalidNhsNumber)
        {
            // given
            string randomValidationCode = GetRandomStringWithLengthOf(5);

            var patientOrchestrationServiceMock = new Mock<PatientOrchestrationService>(
               this.loggingBrokerMock.Object,
               this.securityBrokerMock.Object,
               this.dateTimeBrokerMock.Object,
               this.auditBrokerMock.Object,
               this.identifierBrokerMock.Object,
               this.pdsServiceMock.Object,
               this.patientServiceMock.Object,
               this.notificationServiceMock.Object,
               this.decisionConfigurations,
               this.securityBrokerConfigurations)
            { CallBase = true };

            var invalidPatientOrchestrationArgumentException =
                new InvalidPatientOrchestrationArgumentException(
                    "Invalid patient orchestration argument. Please correct the errors and try again.");

            invalidPatientOrchestrationArgumentException.AddData(
                key: "nhsNumber",
                values: "Text must be exactly 10 digits.");

            var expectedPatientOrchestrationValidationException =
               new PatientOrchestrationValidationException(
                   message: "Patient orchestration validation error occurred, please fix the errors and try again.",
                   innerException: invalidPatientOrchestrationArgumentException);

            patientOrchestrationServiceMock.Setup(service =>
                service.CheckIfIsAuthenticatedUserWithRequiredRoleAsync())
                    .ThrowsAsync(expectedPatientOrchestrationValidationException);

            // when
            ValueTask verifyPatientCodeTask =
                patientOrchestrationServiceMock.Object.VerifyPatientCodeAsync(invalidNhsNumber, randomValidationCode);

            PatientOrchestrationValidationException
                actualPatientOrchestrationValidationException =
                    await Assert.ThrowsAsync<PatientOrchestrationValidationException>(
                        testCode: verifyPatientCodeTask.AsTask);

            // then
            actualPatientOrchestrationValidationException
                .Should().BeEquivalentTo(expectedPatientOrchestrationValidationException);

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

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("1234")]
        [InlineData("123456")]
        public async Task ShouldThrowValidationExceptionOnVerifyPatientCodeWithInvalidValidationCode(
            string invalidValidationCode)
        {
            // given
            string randomNhsNumber = GenerateRandom10DigitNumber();

            var patientOrchestrationServiceMock = new Mock<PatientOrchestrationService>(
                this.loggingBrokerMock.Object,
                this.securityBrokerMock.Object,
                this.dateTimeBrokerMock.Object,
                this.auditBrokerMock.Object,
                this.identifierBrokerMock.Object,
                this.pdsServiceMock.Object,
                this.patientServiceMock.Object,
                this.notificationServiceMock.Object,
                this.decisionConfigurations,
                this.securityBrokerConfigurations)
            { CallBase = true };

            var invalidPatientOrchestrationArgumentException =
                new InvalidPatientOrchestrationArgumentException(
                    "Invalid patient orchestration argument. Please correct the errors and try again.");

            invalidPatientOrchestrationArgumentException.AddData(
                key: "verificationCode",
                values: "Code must be 5 characters long.");

            var expectedPatientOrchestrationValidationException =
               new PatientOrchestrationValidationException(
                   message: "Patient orchestration validation error occurred, please fix the errors and try again.",
                   innerException: invalidPatientOrchestrationArgumentException);

            patientOrchestrationServiceMock.Setup(service =>
                service.CheckIfIsAuthenticatedUserWithRequiredRoleAsync())
                    .ThrowsAsync(expectedPatientOrchestrationValidationException);

            // when
            ValueTask verifyPatientCodeTask =
                patientOrchestrationServiceMock.Object.VerifyPatientCodeAsync(randomNhsNumber, invalidValidationCode);

            PatientOrchestrationValidationException
                actualPatientOrchestrationValidationException =
                    await Assert.ThrowsAsync<PatientOrchestrationValidationException>(
                        testCode: verifyPatientCodeTask.AsTask);

            // then
            actualPatientOrchestrationValidationException
                .Should().BeEquivalentTo(expectedPatientOrchestrationValidationException);

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
