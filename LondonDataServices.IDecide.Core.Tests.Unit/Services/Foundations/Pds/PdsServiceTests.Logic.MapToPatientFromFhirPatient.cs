// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using FluentAssertions;
using Force.DeepCloner;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.Pds
{
    public partial class PdsServiceTests
    {
        [Fact]
        public void ShouldMapToPatientFromFhirPatient()
        {
            // given
            Hl7.Fhir.Model.Patient fhirPatient = CreateRandomPatient();
            Hl7.Fhir.Model.Patient inputFhirPatient = fhirPatient.DeepClone();
            Patient mappedPatient = GeneratePatientFromFhirPatient(inputFhirPatient);
            Patient expectedPatient = mappedPatient.DeepClone();

            // when
            Patient actualPatient = this.pdsService.MapToPatientFromFhirPatient(inputFhirPatient);

            //then
            actualPatient.Should().BeEquivalentTo(expectedPatient);

            this.pdsBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldMapToPatientFromFhirPatientWithWhiteSpace()
        {
            // given
            Hl7.Fhir.Model.Patient fhirPatient = CreateRandomPatient(true);
            Hl7.Fhir.Model.Patient inputFhirPatient = fhirPatient.DeepClone();
            Patient mappedPatient = GeneratePatientFromFhirPatient(inputFhirPatient);
            Patient expectedPatient = mappedPatient.DeepClone();

            // when
            Patient actualPatient = this.pdsService.MapToPatientFromFhirPatient(inputFhirPatient);

            //then
            actualPatient.Should().BeEquivalentTo(expectedPatient);

            this.pdsBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldMapToPatientFromFhirPatientToPartialModelWhenIsSensitivePatient()
        {
            // given
            Hl7.Fhir.Model.Patient fhirSensitivePatient = CreateRandomSensitivePatient();
            Hl7.Fhir.Model.Patient inputSensitiveFhirPatient = fhirSensitivePatient.DeepClone();
            Patient mappedPatient = GeneratePatientFromFhirPatient(inputSensitiveFhirPatient);
            Patient expectedPatient = mappedPatient.DeepClone();

            // when
            Patient actualPatient = this.pdsService.MapToPatientFromFhirPatient(inputSensitiveFhirPatient);

            //then
            actualPatient.Should().BeEquivalentTo(expectedPatient);

            this.pdsBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}