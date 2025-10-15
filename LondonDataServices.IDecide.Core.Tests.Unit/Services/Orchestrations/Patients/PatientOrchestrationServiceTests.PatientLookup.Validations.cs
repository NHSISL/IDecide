// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using LondonDataServices.IDecide.Core.Models.Foundations.Pds;
using LondonDataServices.IDecide.Core.Models.Orchestrations.Patients.Exceptions;
using LondonDataServices.IDecide.Core.Services.Orchestrations.Patients;
using Moq;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Orchestrations.Patients
{
    public partial class PatientOrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnPatientLookupByDetailsWhenNullAndLogItAsync()
        {
            // given
            PatientLookup nullPatientLookup = null;

            var nullPatientLookupException =
                new NullPatientLookupException(message: "Patient lookup is null.");

            var expectedPatientOrchestrationValidationException =
                new PatientOrchestrationValidationException(
                    message: "Patient orchestration validation error occurred, " +
                        "please fix the errors and try again.",
                    innerException: nullPatientLookupException);

            this.pdsServiceMock.Setup(service =>
                service.PatientLookupByDetailsAsync(nullPatientLookup))
                    .ThrowsAsync(nullPatientLookupException);

            // when
            ValueTask<Patient> patientLookupTask =
                this.patientOrchestrationService
                    .PatientLookupAsync(nullPatientLookup);

            PatientOrchestrationValidationException
                actualPatientOrchestrationValidationException =
                    await Assert.ThrowsAsync<PatientOrchestrationValidationException>(
                        testCode: patientLookupTask.AsTask);

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

        [Fact]
        public async Task ShouldThrowValidationExceptionOnPatientLookupByDetailsWhenNoPatientsFoundAndLogItAsync()
        {
            // given
            string randomString = GetRandomString();
            string inputSurname = randomString.DeepClone();
            PatientLookup randomPatientLookup = GetRandomSearchPatientLookupWithNoNhsNumber(inputSurname);
            PatientLookup inputPatientLookup = randomPatientLookup.DeepClone();
            PatientLookup updatedPatientLookup = randomPatientLookup.DeepClone();
            updatedPatientLookup.Patients = new List<Patient>();
            PatientLookup outputPatientLookup = updatedPatientLookup.DeepClone();

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

            patientOrchestrationServiceMock.Setup(service =>
                service.CheckIfIsAuthenticatedUserWithRequiredRoleAsync())
                    .ReturnsAsync(false);

            var nullPatientLookupException =
                new NoExactPatientFoundException(message: "No matching patient found.");

            var expectedPatientOrchestrationValidationException =
                new PatientOrchestrationValidationException(
                    message: "Patient orchestration validation error occurred, " +
                        "please fix the errors and try again.",
                    innerException: nullPatientLookupException);

            this.pdsServiceMock.Setup(service =>
                service.PatientLookupByDetailsAsync(inputPatientLookup))
                    .ReturnsAsync(outputPatientLookup);

            // when
            ValueTask<Patient> patientLookupTask =
                patientOrchestrationServiceMock.Object
                    .PatientLookupAsync(inputPatientLookup);

            PatientOrchestrationValidationException
                actualPatientOrchestrationValidationException =
                    await Assert.ThrowsAsync<PatientOrchestrationValidationException>(
                        testCode: patientLookupTask.AsTask);

            // then
            actualPatientOrchestrationValidationException
                .Should().BeEquivalentTo(expectedPatientOrchestrationValidationException);

            patientOrchestrationServiceMock.Verify(service =>
                service.CheckIfIsAuthenticatedUserWithRequiredRoleAsync(),
                    Times.Once);

            this.pdsServiceMock.Verify(service =>
                service.PatientLookupByDetailsAsync(inputPatientLookup),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedPatientOrchestrationValidationException))),
                       Times.Once);

            patientOrchestrationServiceMock.VerifyNoOtherCalls();
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
        public async Task ShouldThrowValidationExceptionOnPatientLookupByDetailsWhenMultiplePatientsFoundAndLogItAsync()
        {
            // given
            string randomString = GetRandomString();
            string inputSurname = randomString.DeepClone();
            PatientLookup randomPatientLookup = GetRandomSearchPatientLookupWithNoNhsNumber(inputSurname);
            PatientLookup inputPatientLookup = randomPatientLookup.DeepClone();
            PatientLookup updatedPatientLookup = randomPatientLookup.DeepClone();
            updatedPatientLookup.Patients = GetRandomPatients();
            PatientLookup outputPatientLookup = updatedPatientLookup.DeepClone();

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

            patientOrchestrationServiceMock.Setup(service =>
                service.CheckIfIsAuthenticatedUserWithRequiredRoleAsync())
                    .ReturnsAsync(false);

            var nullPatientLookupException =
                new NoExactPatientFoundException(message: "Multiple matching patients found.");

            var expectedPatientOrchestrationValidationException =
                new PatientOrchestrationValidationException(
                    message: "Patient orchestration validation error occurred, " +
                        "please fix the errors and try again.",
                    innerException: nullPatientLookupException);

            this.pdsServiceMock.Setup(service =>
                service.PatientLookupByDetailsAsync(inputPatientLookup))
                    .ReturnsAsync(outputPatientLookup);

            // when
            ValueTask<Patient> patientLookupTask =
                patientOrchestrationServiceMock.Object
                    .PatientLookupAsync(inputPatientLookup);

            PatientOrchestrationValidationException
                actualPatientOrchestrationValidationException =
                    await Assert.ThrowsAsync<PatientOrchestrationValidationException>(
                        testCode: patientLookupTask.AsTask);

            // then
            actualPatientOrchestrationValidationException
                .Should().BeEquivalentTo(expectedPatientOrchestrationValidationException);

            patientOrchestrationServiceMock.Verify(service =>
                service.CheckIfIsAuthenticatedUserWithRequiredRoleAsync(),
                    Times.Once);

            this.pdsServiceMock.Verify(service =>
                service.PatientLookupByDetailsAsync(inputPatientLookup),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedPatientOrchestrationValidationException))),
                       Times.Once);

            patientOrchestrationServiceMock.VerifyNoOtherCalls();
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
        [InlineData("123456789")]
        [InlineData("01234567890")]
        public async Task ShouldThrowValidationExceptionOnPatientLookupByNhsNumberAsync(string invalidNhsNumber)
        {
            // given
            PatientLookup randomPatientLookup = GetRandomSearchPatientLookupWithNhsNumber(invalidNhsNumber);
            PatientLookup inputPatientLookup = randomPatientLookup.DeepClone();

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

            patientOrchestrationServiceMock.Setup(service =>
                service.CheckIfIsAuthenticatedUserWithRequiredRoleAsync())
                    .ReturnsAsync(false);

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

            // when
            ValueTask<Patient> patientLookupByNhsNumberAction =
                patientOrchestrationServiceMock.Object.PatientLookupAsync(inputPatientLookup);

            PatientOrchestrationValidationException actualException =
                await Assert.ThrowsAsync<PatientOrchestrationValidationException>(
                    patientLookupByNhsNumberAction.AsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedPatientOrchestrationValidationException);

            patientOrchestrationServiceMock.Verify(service =>
                service.CheckIfIsAuthenticatedUserWithRequiredRoleAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedPatientOrchestrationValidationException))),
                       Times.Once);

            patientOrchestrationServiceMock.VerifyNoOtherCalls();
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
        public async Task ShouldThrowValidationExceptionOnPatientLookupByNhsNumberWhenNoPatientFoundAndLogItAsync()
        {
            // given
            string randomNhsNumber = GenerateRandom10DigitNumber();
            string inputNhsNumber = randomNhsNumber.DeepClone();
            PatientLookup randomPatientLookup = GetRandomSearchPatientLookupWithNhsNumber(inputNhsNumber);
            PatientLookup inputPatientLookup = randomPatientLookup.DeepClone();
            Patient nullPatient = null;

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

            patientOrchestrationServiceMock.Setup(service =>
                service.CheckIfIsAuthenticatedUserWithRequiredRoleAsync())
                    .ReturnsAsync(false);

            var nullPatientException =
                new NullPatientOrchestrationException(message: "Patient is null.");

            var expectedPatientOrchestrationValidationException =
                new PatientOrchestrationValidationException(
                    message: "Patient orchestration validation error occurred, " +
                        "please fix the errors and try again.",
                    innerException: nullPatientException);

            this.pdsServiceMock.Setup(service =>
                service.PatientLookupByNhsNumberAsync(inputNhsNumber))
                    .ReturnsAsync(nullPatient);

            // when
            ValueTask<Patient> patientLookupTask =
                patientOrchestrationServiceMock.Object
                    .PatientLookupAsync(inputPatientLookup);

            PatientOrchestrationValidationException
                actualPatientOrchestrationValidationException =
                    await Assert.ThrowsAsync<PatientOrchestrationValidationException>(
                        testCode: patientLookupTask.AsTask);

            // then
            actualPatientOrchestrationValidationException
                .Should().BeEquivalentTo(expectedPatientOrchestrationValidationException);

            patientOrchestrationServiceMock.Verify(service =>
                service.CheckIfIsAuthenticatedUserWithRequiredRoleAsync(),
                    Times.Once);

            this.pdsServiceMock.Verify(service =>
                service.PatientLookupByNhsNumberAsync(inputNhsNumber),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedPatientOrchestrationValidationException))),
                       Times.Once);

            patientOrchestrationServiceMock.VerifyNoOtherCalls();
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
