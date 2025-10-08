// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using LondonDataServices.IDecide.Core.Models.Orchestrations.Patients.Exceptions;
using LondonDataServices.IDecide.Core.Services.Orchestrations.Patients;
using Moq;
using Xeptions;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Orchestrations.Patients
{
    public partial class PatientOrchestrationServiceTests
    {
        [Theory]
        [MemberData(nameof(VerifyPatientCodeDependencyValidationExceptions))]
        public async Task ShouldThrowDependencyValidationOnVerifyPatientCodeAndLogItAsync(
            Xeption dependencyValidationException)
        {
            // given
            string randomNhsNumber = GenerateRandom10DigitNumber();
            string inputNhsNumber = randomNhsNumber.DeepClone();
            string randomValidationCode = GetRandomStringWithLengthOf(5);
            string inputValidationCode = randomValidationCode.DeepClone();

            var expectedPatientOrchestrationDependencyValidationException =
                new PatientOrchestrationDependencyValidationException(
                    message: "Patient orchestration dependency validation error occurred, " +
                        "please fix the errors and try again.",
                    innerException: dependencyValidationException.InnerException as Xeption);

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

            patientOrchestrationServiceMock.Setup(broker =>
                broker.CheckIfIsAuthenticatedUserWithRequiredRoleAsync())
                    .ThrowsAsync(dependencyValidationException);

            // when
            ValueTask verifyPatientCodeTask =
                 patientOrchestrationServiceMock.Object.VerifyPatientCodeAsync(
                    inputNhsNumber,
                    inputValidationCode);

            PatientOrchestrationDependencyValidationException
                actualPatientOrchestrationDependencyValidationException =
                    await Assert.ThrowsAsync<PatientOrchestrationDependencyValidationException>(
                        testCode: verifyPatientCodeTask.AsTask);

            // then
            actualPatientOrchestrationDependencyValidationException
                .Should().BeEquivalentTo(expectedPatientOrchestrationDependencyValidationException);

            patientOrchestrationServiceMock.Verify(broker =>
                broker.CheckIfIsAuthenticatedUserWithRequiredRoleAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedPatientOrchestrationDependencyValidationException))),
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
        [MemberData(nameof(VerifyPatientCodeDependencyExceptions))]
        public async Task ShouldThrowDependencyOnVerifyPatientCodeAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            string randomNhsNumber = GenerateRandom10DigitNumber();
            string inputNhsNumber = randomNhsNumber.DeepClone();
            string randomValidationCode = GetRandomStringWithLengthOf(5);
            string inputValidationCode = randomValidationCode.DeepClone();

            var expectedPatientOrchestrationDependencyException =
                 new PatientOrchestrationDependencyException(
                    message: "Patient orchestration dependency error occurred, " +
                        "please fix the errors and try again.",
                    innerException: dependencyException.InnerException as Xeption);

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

            patientOrchestrationServiceMock.Setup(broker =>
                broker.CheckIfIsAuthenticatedUserWithRequiredRoleAsync())
                    .ThrowsAsync(dependencyException);

            // when
            ValueTask verifyPatientCodeTask =
                 patientOrchestrationServiceMock.Object.VerifyPatientCodeAsync(
                    inputNhsNumber,
                    inputValidationCode);

            PatientOrchestrationDependencyException
                actualPatientOrchestrationDependencyException =
                    await Assert.ThrowsAsync<PatientOrchestrationDependencyException>(
                        testCode: verifyPatientCodeTask.AsTask);

            // then
            actualPatientOrchestrationDependencyException
                .Should().BeEquivalentTo(expectedPatientOrchestrationDependencyException);

            patientOrchestrationServiceMock.Verify(broker =>
                broker.CheckIfIsAuthenticatedUserWithRequiredRoleAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedPatientOrchestrationDependencyException))),
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
        public async Task ShouldThrowServiceExceptionOnVerifyPatientCodeIfServiceErrorOccurredAndLogItAsync()
        {
            // given
            string randomNhsNumber = GenerateRandom10DigitNumber();
            string inputNhsNumber = randomNhsNumber.DeepClone();
            string randomValidationCode = GetRandomStringWithLengthOf(5);
            string inputValidationCode = randomValidationCode.DeepClone();
            var serviceException = new Exception();

            var failedServicePatientOrchestrationException =
                new FailedPatientOrchestrationServiceException(
                    message: "Failed patient orchestration service error occurred, contact support.",
                    innerException: serviceException);

            var expectedPatientOrchestrationServiceException =
                new PatientOrchestrationServiceException(
                    message: "Patient orchestration service error occurred, contact support.",
                    innerException: failedServicePatientOrchestrationException.InnerException as Xeption);

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

            patientOrchestrationServiceMock.Setup(broker =>
                broker.CheckIfIsAuthenticatedUserWithRequiredRoleAsync())
                    .ThrowsAsync(serviceException);

            // when
            ValueTask verifyPatientCodeTask =
                 patientOrchestrationServiceMock.Object.VerifyPatientCodeAsync(
                    inputNhsNumber,
                    inputValidationCode);

            PatientOrchestrationServiceException
                actualPatientOrchestrationValidationException =
                    await Assert.ThrowsAsync<PatientOrchestrationServiceException>(
                        testCode: verifyPatientCodeTask.AsTask);

            // then
            actualPatientOrchestrationValidationException.Should().BeEquivalentTo(
                expectedPatientOrchestrationServiceException);

            patientOrchestrationServiceMock.Verify(broker =>
                 broker.CheckIfIsAuthenticatedUserWithRequiredRoleAsync(),
                     Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedPatientOrchestrationServiceException))),
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
