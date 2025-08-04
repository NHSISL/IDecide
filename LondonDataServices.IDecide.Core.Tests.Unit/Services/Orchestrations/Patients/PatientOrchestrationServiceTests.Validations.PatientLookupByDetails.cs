// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using LondonDataServices.IDecide.Core.Models.Foundations.Pds;
using Moq;
using FluentAssertions;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Orchestrations.Patients.Exceptions;
using Force.DeepCloner;
using Hl7.Fhir.Model;
using ISL.Providers.PDS.Abstractions.Models;
using Task = System.Threading.Tasks.Task;
using Patient = LondonDataServices.IDecide.Core.Models.Foundations.Pds.Patient;

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
                        "fix the errors and try again.",
                    innerException: nullPatientLookupException);

            this.pdsServiceMock.Setup(service =>
                service.PatientLookupByDetailsAsync(nullPatientLookup))
                    .ThrowsAsync(nullPatientLookupException);

            // when
            ValueTask<Patient> patientLookupTask =
                this.patientOrchestrationService
                    .PatientLookupByDetailsAsync(nullPatientLookup);

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

            this.pdsServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnPatientLookupByDetailsWhenNoPatientsFoundAndLogItAsync()
        {
            // given
            string randomString = GetRandomString();
            string inputSurname = randomString.DeepClone();
            PatientLookup randomPatientLookup = GetRandomSearchPatientLookup(inputSurname);
            PatientLookup inputPatientLookup = randomPatientLookup.DeepClone();
            Bundle randomEmptyBundle = CreateRandomEmptyBundle();
            PatientBundle outputPatientBundle = CreateRandomPatientBundle(randomEmptyBundle);
            PatientLookup updatedPatientLookup = randomPatientLookup.DeepClone();
            updatedPatientLookup.Patients = outputPatientBundle;
            PatientLookup outputPatientLookup = updatedPatientLookup.DeepClone();

            var nullPatientLookupException =
                new NoExactPatientFoundException(message: "No matching patient found.");

            var expectedPatientOrchestrationValidationException =
                new PatientOrchestrationValidationException(
                    message: "Patient orchestration validation error occurred, " +
                        "fix the errors and try again.",
                    innerException: nullPatientLookupException);

            this.pdsServiceMock.Setup(service =>
                service.PatientLookupByDetailsAsync(inputPatientLookup))
                    .ReturnsAsync(outputPatientLookup);

            // when
            ValueTask<Patient> patientLookupTask =
                this.patientOrchestrationService
                    .PatientLookupByDetailsAsync(inputPatientLookup);

            PatientOrchestrationValidationException
                actualPatientOrchestrationValidationException =
                    await Assert.ThrowsAsync<PatientOrchestrationValidationException>(
                        testCode: patientLookupTask.AsTask);

            // then
            actualPatientOrchestrationValidationException
                .Should().BeEquivalentTo(expectedPatientOrchestrationValidationException);

            this.pdsServiceMock.Verify(service =>
                service.PatientLookupByDetailsAsync(inputPatientLookup),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedPatientOrchestrationValidationException))),
                       Times.Once);

            this.pdsServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnPatientLookupByDetailsWhenMultiplePatientsFoundAndLogItAsync()
        {
            // given
            string randomString = GetRandomString();
            string inputSurname = randomString.DeepClone();
            PatientLookup randomPatientLookup = GetRandomSearchPatientLookup(inputSurname);
            PatientLookup inputPatientLookup = randomPatientLookup.DeepClone();
            Bundle randomEmptyBundle = CreateRandomMultiplePatientBundle();
            PatientBundle outputPatientBundle = CreateRandomPatientBundle(randomEmptyBundle);
            PatientLookup updatedPatientLookup = randomPatientLookup.DeepClone();
            updatedPatientLookup.Patients = outputPatientBundle;
            PatientLookup outputPatientLookup = updatedPatientLookup.DeepClone();

            var nullPatientLookupException =
                new NoExactPatientFoundException(message: "Multiple matching patients found.");

            var expectedPatientOrchestrationValidationException =
                new PatientOrchestrationValidationException(
                    message: "Patient orchestration validation error occurred, " +
                        "fix the errors and try again.",
                    innerException: nullPatientLookupException);

            this.pdsServiceMock.Setup(service =>
                service.PatientLookupByDetailsAsync(inputPatientLookup))
                    .ReturnsAsync(outputPatientLookup);

            // when
            ValueTask<Patient> patientLookupTask =
                this.patientOrchestrationService
                    .PatientLookupByDetailsAsync(inputPatientLookup);

            PatientOrchestrationValidationException
                actualPatientOrchestrationValidationException =
                    await Assert.ThrowsAsync<PatientOrchestrationValidationException>(
                        testCode: patientLookupTask.AsTask);

            // then
            actualPatientOrchestrationValidationException
                .Should().BeEquivalentTo(expectedPatientOrchestrationValidationException);

            this.pdsServiceMock.Verify(service =>
                service.PatientLookupByDetailsAsync(inputPatientLookup),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedPatientOrchestrationValidationException))),
                       Times.Once);

            this.pdsServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
