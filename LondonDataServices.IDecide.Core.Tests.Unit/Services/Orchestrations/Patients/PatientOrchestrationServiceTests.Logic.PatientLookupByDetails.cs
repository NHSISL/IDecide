// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Hl7.Fhir.Model;
using ISL.Providers.PDS.Abstractions.Models;
using LondonDataServices.IDecide.Core.Models.Foundations.Pds;
using Moq;
using FluentAssertions;
using Force.DeepCloner;
using System.Linq;
using Patient = LondonDataServices.IDecide.Core.Models.Foundations.Pds.Patient;
using Task = System.Threading.Tasks.Task;
using LondonDataServices.IDecide.Core.Extensions.Patients;

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
            Patient redactedPatient = patientToRedact.GetRedactedPatient();
            Patient expectedPatient = redactedPatient.DeepClone();

            this.pdsServiceMock.Setup(service =>
                service.PatientLookupByDetailsAsync(inputPatientLookup))
                        .ReturnsAsync(outputPatientLookup);

            // when
            Patient actualPatient = 
                await this.patientOrchestrationService.PatientLookupByDetailsAsync(inputPatientLookup);

            //then
            actualPatient.Should().BeEquivalentTo(expectedPatient);

            this.pdsServiceMock.Verify(service =>
                service.PatientLookupByDetailsAsync(inputPatientLookup),
                        Times.Once);

            this.pdsServiceMock.VerifyNoOtherCalls();
        }
    }
}
