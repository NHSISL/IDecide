// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Patient = LondonDataServices.IDecide.Core.Models.Foundations.Patients.Patient;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.Pds
{
    public partial class PdsServiceTests
    {
        [Fact]
        public void ShouldMapToPatientsFromBundleJson()
        {
            // given
            Bundle randomBundle = CreateRandomBundle();
            var serializer = new FhirJsonSerializer();
            string bundleJson = serializer.SerializeToString(randomBundle);

            Hl7.Fhir.Model.Patient fhirPatient =
                (Hl7.Fhir.Model.Patient)randomBundle.Entry.First().Resource;

            Patient mappedPatient = GeneratePatientFromFhirPatient(fhirPatient);
            List<Patient> expectedPatients = new List<Patient> { mappedPatient };

            // when
            List<Patient> actualPatients = this.pdsService.MapToPatientsFromBundleJson(bundleJson);

            //then
            actualPatients.Should().BeEquivalentTo(expectedPatients);

            this.pdsBrokerMock.VerifyNoOtherCalls();
            this.nhsDigitalApiBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}