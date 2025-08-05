// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using LondonDataServices.IDecide.Core.Models.Foundations.Pds;
using Moq;
using FluentAssertions;
using Force.DeepCloner;
using System.Linq;
using LondonDataServices.IDecide.Core.Extensions.Patients;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            PatientLookup updatedPatientLookup = randomPatientLookup.DeepClone();
            updatedPatientLookup.Patients = new List<Patient> { GetRandomPatient(inputSurname) };
            PatientLookup outputPatientLookup = updatedPatientLookup.DeepClone();
            Patient patient = outputPatientLookup.Patients.FirstOrDefault();
            Patient patientToRedact = patient.DeepClone();
            Patient redactedPatient = patientToRedact.Redact();
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
