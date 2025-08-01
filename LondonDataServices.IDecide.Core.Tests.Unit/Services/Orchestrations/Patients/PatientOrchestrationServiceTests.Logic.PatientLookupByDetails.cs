// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Hl7.Fhir.Model;
using ISL.Providers.PDS.Abstractions.Models;
using LondonDataServices.IDecide.Core.Models.Foundations.Pds;
using LondonDataServices.IDecide.Core.Services.Orchestrations.Patients;
using Moq;
using FluentAssertions;
using Force.DeepCloner;
using System.Linq;
using Patient = LondonDataServices.IDecide.Core.Models.Foundations.Pds.Patient;
using Task = System.Threading.Tasks.Task;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Orchestrations.Patients
{
    public partial class PatientOrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldPatientLookupByDetailsAsync()
        {
            // given
            string randomString = GetRandomString();
            string inputSurname = randomString.DeepClone();
            PatientLookup randomPatientLookup = GetRandomSearchPatientLookup(inputSurname);
            PatientLookup inputPatientLookup = randomPatientLookup.DeepClone();
            Bundle randomBundle = CreateRandomBundle(inputSurname);
            PatientBundle outputPatientBundle = CreateRandomPatientBundle(randomBundle);
            PatientLookup updatedPatientLookup = randomPatientLookup.DeepClone();
            updatedPatientLookup.Patients = outputPatientBundle;
            PatientLookup outputPatientLookup = updatedPatientLookup.DeepClone();
            Hl7.Fhir.Model.Patient fhirPatient = outputPatientLookup.Patients.Patients.FirstOrDefault();
            Patient patientToRedact = GetPatientFromFhirPatient(fhirPatient);
            Patient redactedPatient = GetRedactedPatient(patientToRedact);
            Patient expectedPatient = redactedPatient.DeepClone();

            var patientOrchestrationServiceMock = new Mock<PatientOrchestrationService>(
                this.pdsServiceMock.Object)
            { CallBase = true };

            this.pdsServiceMock.Setup(service =>
                service.PatientLookupByDetailsAsync(inputPatientLookup))
                        .ReturnsAsync(outputPatientLookup);

            patientOrchestrationServiceMock.Setup(service =>
                service.RedactPatientDetails(patientToRedact))
                    .Returns(redactedPatient);

            // when
            Patient actualPatient = 
                await patientOrchestrationServiceMock.Object.PatientLookupByDetailsAsync(inputPatientLookup);

            //then
            actualPatient.Should().BeEquivalentTo(expectedPatient);

            this.pdsServiceMock.Verify(service =>
                service.PatientLookupByDetailsAsync(inputPatientLookup),
                        Times.Once);

            patientOrchestrationServiceMock.Verify(service =>
                service.RedactPatientDetails(patientToRedact), 
                    Times.Once);

            this.pdsServiceMock.VerifyNoOtherCalls();
        }
    }
}
