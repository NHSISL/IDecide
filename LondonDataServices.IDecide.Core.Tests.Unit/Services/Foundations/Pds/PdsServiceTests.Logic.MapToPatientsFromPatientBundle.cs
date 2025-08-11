// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Force.DeepCloner;
using Hl7.Fhir.Model;
using ISL.Providers.PDS.Abstractions.Models;
using Patient = LondonDataServices.IDecide.Core.Models.Foundations.Patients.Patient;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.Pds
{
    public partial class PdsServiceTests
    {
        [Fact]
        public void ShouldMapToPatientsFromPatientBundle()
        {
            // given
            Bundle randomBundle = CreateRandomBundle();
            PatientBundle patientBundle = CreateRandomPatientBundle(randomBundle);
            PatientBundle inputPatientBundle = patientBundle.DeepClone();
            Patient mappedPatient = GeneratePatientFromFhirPatient(inputPatientBundle.Patients.First());
            List<Patient> expectedPatients = new List<Patient> { mappedPatient };

            // when
            List<Patient> actualPatients = this.pdsService.MapToPatientsFromPatientBundle(inputPatientBundle);

            //then
            actualPatients.Should().BeEquivalentTo(expectedPatients);

            this.pdsBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}