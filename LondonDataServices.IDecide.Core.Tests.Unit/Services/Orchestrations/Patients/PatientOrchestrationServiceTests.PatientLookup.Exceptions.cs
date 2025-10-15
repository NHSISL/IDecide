// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using LondonDataServices.IDecide.Core.Models.Foundations.Pds;
using LondonDataServices.IDecide.Core.Models.Orchestrations.Patients.Exceptions;
using LondonDataServices.IDecide.Core.Services.Orchestrations.Patients;
using Moq;
using Xeptions;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Orchestrations.Patients
{
    public partial class PatientOrchestrationServiceTests
    {
        [Theory]
        [MemberData(nameof(DependencyValidationExceptions))]
        public async Task ShouldThrowDependencyValidationOnPatientLookupWithDetailsAndLogItAsync(
            Xeption dependencyValidationException)
        {
            // given
            string randomString = GetRandomString();
            string inputSurname = randomString.DeepClone();
            PatientLookup randomPatientLookup = GetRandomSearchPatientLookupWithNoNhsNumber(inputSurname);
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

            this.pdsServiceMock.Setup(service =>
                service.PatientLookupByDetailsAsync(inputPatientLookup))
                    .ThrowsAsync(dependencyValidationException);

            var expectedPatientOrchestrationDependencyValidationException =
                new PatientOrchestrationDependencyValidationException(
                    message: "Patient orchestration dependency validation error occurred, " +
                        "please fix the errors and try again.",
                    innerException: dependencyValidationException.InnerException as Xeption);

            // when
            ValueTask<Patient> patientLookupTask =
                patientOrchestrationServiceMock.Object.PatientLookupAsync(inputPatientLookup);

            PatientOrchestrationDependencyValidationException
                actualPatientOrchestrationDependencyValidationException =
                    await Assert.ThrowsAsync<PatientOrchestrationDependencyValidationException>(
                        testCode: patientLookupTask.AsTask);

            // then
            actualPatientOrchestrationDependencyValidationException
                .Should().BeEquivalentTo(expectedPatientOrchestrationDependencyValidationException);

            patientOrchestrationServiceMock.Verify(service =>
                service.CheckIfIsAuthenticatedUserWithRequiredRoleAsync(),
                    Times.Once());

            this.pdsServiceMock.Verify(service =>
                service.PatientLookupByDetailsAsync(inputPatientLookup),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedPatientOrchestrationDependencyValidationException))),
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
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyOnPatientLookupWithDetailsAndLogItAsync(
            Xeption dependencyValidationException)
        {
            // given
            string randomString = GetRandomString();
            string inputSurname = randomString.DeepClone();
            PatientLookup randomPatientLookup = GetRandomSearchPatientLookupWithNoNhsNumber(inputSurname);
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

            this.pdsServiceMock.Setup(service =>
                service.PatientLookupByDetailsAsync(inputPatientLookup))
                    .ThrowsAsync(dependencyValidationException);

            var expectedPatientOrchestrationDependencyException =
                new PatientOrchestrationDependencyException(
                    message: "Patient orchestration dependency error occurred, " +
                        "please fix the errors and try again.",
                    innerException: dependencyValidationException.InnerException as Xeption);

            // when
            ValueTask<Patient> patientLookupTask =
                patientOrchestrationServiceMock.Object.PatientLookupAsync(inputPatientLookup);

            PatientOrchestrationDependencyException
                actualPatientOrchestrationDependencyException =
                    await Assert.ThrowsAsync<PatientOrchestrationDependencyException>(
                        testCode: patientLookupTask.AsTask);

            // then
            actualPatientOrchestrationDependencyException
                .Should().BeEquivalentTo(expectedPatientOrchestrationDependencyException);

            patientOrchestrationServiceMock.Verify(service =>
                service.CheckIfIsAuthenticatedUserWithRequiredRoleAsync(),
                    Times.Once());

            this.pdsServiceMock.Verify(service =>
                service.PatientLookupByDetailsAsync(inputPatientLookup),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedPatientOrchestrationDependencyException))),
                       Times.Once());

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
        public async Task
            ShouldThrowExceptionOnPatientLookupWithDetailsIfExternalOptOutPatientOrchestrationOccurredAndLogItAsync()
        {
            // given
            string randomString = GetRandomString();
            string inputSurname = randomString.DeepClone();
            PatientLookup randomPatientLookup = GetRandomSearchPatientLookupWithNoNhsNumber(inputSurname);
            PatientLookup inputPatientLookup = randomPatientLookup.DeepClone();
            PatientLookup updatedPatientLookup = randomPatientLookup.DeepClone();
            updatedPatientLookup.Patients = new List<Patient> { CreateRandomSensitivePatient(inputSurname) };
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

            this.pdsServiceMock.Setup(service =>
                service.PatientLookupByDetailsAsync(inputPatientLookup))
                    .ReturnsAsync(outputPatientLookup);

            var externalOptOutPatientOrchestrationException =
                new ExternalOptOutPatientOrchestrationException("The patient is marked as sensitive.");

            var expectedPatientOrchestrationValidationException =
                new PatientOrchestrationValidationException(
                    message: "Patient orchestration validation error occurred, please fix the errors and try again.",
                    innerException: externalOptOutPatientOrchestrationException);

            // when
            ValueTask<Patient> patientLookupTask =
                patientOrchestrationServiceMock.Object.PatientLookupAsync(inputPatientLookup);

            PatientOrchestrationValidationException
                actualPatientOrchestrationValidationException =
                    await Assert.ThrowsAsync<PatientOrchestrationValidationException>(
                        testCode: patientLookupTask.AsTask);

            //then
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
        public async Task ShouldThrowServiceExceptionOnPatientLookupWithDetailsIfServiceErrorOccurredAndLogItAsync()
        {
            // given
            string randomString = GetRandomString();
            string inputSurname = randomString.DeepClone();
            PatientLookup randomPatientLookup = GetRandomSearchPatientLookupWithNoNhsNumber(inputSurname);
            PatientLookup inputPatientLookup = randomPatientLookup.DeepClone();
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

            patientOrchestrationServiceMock.Setup(service =>
                service.CheckIfIsAuthenticatedUserWithRequiredRoleAsync())
                    .ReturnsAsync(false);

            this.pdsServiceMock.Setup(service =>
                service.PatientLookupByDetailsAsync(inputPatientLookup))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<Patient> patientLookupTask =
               patientOrchestrationServiceMock.Object.PatientLookupAsync(inputPatientLookup);

            PatientOrchestrationServiceException
                actualPatientOrchestrationValidationException =
                    await Assert.ThrowsAsync<PatientOrchestrationServiceException>(
                        testCode: patientLookupTask.AsTask);

            // then
            actualPatientOrchestrationValidationException.Should().BeEquivalentTo(
                expectedPatientOrchestrationServiceException);

            patientOrchestrationServiceMock.Verify(service =>
                service.CheckIfIsAuthenticatedUserWithRequiredRoleAsync(),
                    Times.Once);

            this.pdsServiceMock.Verify(service =>
               service.PatientLookupByDetailsAsync(inputPatientLookup),
                   Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedPatientOrchestrationServiceException))),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedPatientOrchestrationServiceException))),
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
        [MemberData(nameof(DependencyValidationExceptions))]
        public async Task ShouldThrowDependencyValidationOnPatientLookupWithNhsNumberAndLogItAsync(
            Xeption dependencyValidationException)
        {
            // given
            string randomNhsNumber = GenerateRandom10DigitNumber();
            string inputNhsNumber = randomNhsNumber.DeepClone();
            PatientLookup randomPatientLookup = GetRandomSearchPatientLookupWithNhsNumber(inputNhsNumber);
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

            this.pdsServiceMock.Setup(service =>
                service.PatientLookupByNhsNumberAsync(inputNhsNumber))
                    .ThrowsAsync(dependencyValidationException);

            var expectedPatientOrchestrationDependencyValidationException =
                new PatientOrchestrationDependencyValidationException(
                    message: "Patient orchestration dependency validation error occurred, " +
                        "please fix the errors and try again.",
                    innerException: dependencyValidationException.InnerException as Xeption);

            // when
            ValueTask<Patient> patientLookupTask =
                patientOrchestrationServiceMock.Object.PatientLookupAsync(inputPatientLookup);

            PatientOrchestrationDependencyValidationException
                actualPatientOrchestrationDependencyValidationException =
                    await Assert.ThrowsAsync<PatientOrchestrationDependencyValidationException>(
                        testCode: patientLookupTask.AsTask);

            // then
            actualPatientOrchestrationDependencyValidationException
                .Should().BeEquivalentTo(expectedPatientOrchestrationDependencyValidationException);

            patientOrchestrationServiceMock.Verify(service =>
                service.CheckIfIsAuthenticatedUserWithRequiredRoleAsync(),
                    Times.Once);

            this.pdsServiceMock.Verify(service =>
                service.PatientLookupByNhsNumberAsync(inputNhsNumber),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedPatientOrchestrationDependencyValidationException))),
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
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyOnPatientLookupWithNhsNumberAndLogItAsync(
            Xeption dependencyValidationException)
        {
            // given
            string randomNhsNumber = GenerateRandom10DigitNumber();
            string inputNhsNumber = randomNhsNumber.DeepClone();
            PatientLookup randomPatientLookup = GetRandomSearchPatientLookupWithNhsNumber(inputNhsNumber);
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

            this.pdsServiceMock.Setup(service =>
                service.PatientLookupByNhsNumberAsync(inputNhsNumber))
                    .ThrowsAsync(dependencyValidationException);

            var expectedPatientOrchestrationDependencyException =
                new PatientOrchestrationDependencyException(
                    message: "Patient orchestration dependency error occurred, " +
                        "please fix the errors and try again.",
                    innerException: dependencyValidationException.InnerException as Xeption);

            // when
            ValueTask<Patient> patientLookupTask =
                patientOrchestrationServiceMock.Object.PatientLookupAsync(inputPatientLookup);

            PatientOrchestrationDependencyException
                actualPatientOrchestrationDependencyException =
                    await Assert.ThrowsAsync<PatientOrchestrationDependencyException>(
                        testCode: patientLookupTask.AsTask);

            // then
            actualPatientOrchestrationDependencyException
                .Should().BeEquivalentTo(expectedPatientOrchestrationDependencyException);

            patientOrchestrationServiceMock.Verify(service =>
                service.CheckIfIsAuthenticatedUserWithRequiredRoleAsync(),
                    Times.Once);

            this.pdsServiceMock.Verify(service =>
                service.PatientLookupByNhsNumberAsync(inputNhsNumber),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedPatientOrchestrationDependencyException))),
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
        public async Task ShouldThrowExceptionOnPatientLookupWithNhsNumberIfExternalOptOutPatientOrchestrationOccurredAndLogItAsync()
        {
            // given
            string randomNhsNumber = GenerateRandom10DigitNumber();
            string inputNhsNumber = randomNhsNumber.DeepClone();
            PatientLookup randomPatientLookup = GetRandomSearchPatientLookupWithNhsNumber(inputNhsNumber);
            PatientLookup inputPatientLookup = randomPatientLookup.DeepClone();
            Patient randomPatient = GetRandomSensitivePatient();

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

            this.pdsServiceMock.Setup(service =>
                service.PatientLookupByNhsNumberAsync(inputNhsNumber))
                    .ReturnsAsync(randomPatient);

            var externalOptOutPatientOrchestrationException =
                new ExternalOptOutPatientOrchestrationException("The patient is marked as sensitive.");

            var expectedPatientOrchestrationValidationException =
                new PatientOrchestrationValidationException(
                    message: "Patient orchestration validation error occurred, please fix the errors and try again.",
                    innerException: externalOptOutPatientOrchestrationException);

            // when
            ValueTask<Patient> patientLookupTask =
                patientOrchestrationServiceMock.Object.PatientLookupAsync(inputPatientLookup);

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

        [Fact]
        public async Task ShouldThrowServiceExceptionOnPatientLookupWithNhsNumberIfServiceErrorOccurredAndLogItAsync()
        {
            // given
            string randomNhsNumber = GenerateRandom10DigitNumber();
            string inputNhsNumber = randomNhsNumber.DeepClone();
            PatientLookup randomPatientLookup = GetRandomSearchPatientLookupWithNhsNumber(inputNhsNumber);
            PatientLookup inputPatientLookup = randomPatientLookup.DeepClone();

            var serviceException = new Exception();

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

            var failedServicePatientOrchestrationException =
                new FailedPatientOrchestrationServiceException(
                    message: "Failed patient orchestration service error occurred, contact support.",
                    innerException: serviceException);

            var expectedPatientOrchestrationServiceException =
                new PatientOrchestrationServiceException(
                    message: "Patient orchestration service error occurred, contact support.",
                    innerException: failedServicePatientOrchestrationException.InnerException as Xeption);

            this.pdsServiceMock.Setup(service =>
                service.PatientLookupByNhsNumberAsync(inputNhsNumber))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<Patient> patientLookupTask =
               patientOrchestrationServiceMock.Object.PatientLookupAsync(inputPatientLookup);

            PatientOrchestrationServiceException
                actualPatientOrchestrationValidationException =
                    await Assert.ThrowsAsync<PatientOrchestrationServiceException>(
                        testCode: patientLookupTask.AsTask);

            // then
            actualPatientOrchestrationValidationException.Should().BeEquivalentTo(
                expectedPatientOrchestrationServiceException);

            patientOrchestrationServiceMock.Verify(service =>
                service.CheckIfIsAuthenticatedUserWithRequiredRoleAsync(),
                    Times.Once);

            this.pdsServiceMock.Verify(service =>
               service.PatientLookupByNhsNumberAsync(inputNhsNumber),
                   Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedPatientOrchestrationServiceException))),
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
