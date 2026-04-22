// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using Hl7.Fhir.Serialization;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using LondonDataServices.IDecide.Core.Services.Foundations.Pds;
using Moq;
using NhsDigitalSearchCriteria = NHSDigital.ApiPlatform.Sdk.Models.Foundations.Pds.SearchCriteria;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.Pds
{
    public partial class PdsServiceTests
    {
        [Fact]
        public async Task ShouldPatientLookupByNhsNumberAsync()
        {
            // given
            string randomIdentifier = GenerateRandom10DigitNumber();
            string inputNhsNumber = randomIdentifier.DeepClone();
            Hl7.Fhir.Model.Patient outputFhirPatient = CreateRandomPatientWithNhsNumber(inputNhsNumber);
            var serializer = new FhirJsonSerializer();
            string patientJson = serializer.SerializeToString(outputFhirPatient);
            Patient expectedPatient = GeneratePatientFromFhirPatient(outputFhirPatient);
            List<Patient> mappedPatients = new List<Patient> { expectedPatient };

            var pdsServiceMock = new Mock<PdsService>(
                this.pdsBrokerMock.Object,
                this.nhsDigitalApiBrokerMock.Object,
                this.loggingBrokerMock.Object)
            { CallBase = true };

            pdsServiceMock.Setup(service =>
                service.MapToPatientsFromPatientBundle(patientJson))
                    .Returns(mappedPatients);

            this.nhsDigitalApiBrokerMock.Setup(broker =>
                broker.SearchPatientPDSAsync(
                    It.IsAny<NhsDigitalSearchCriteria>(),
                    CancellationToken.None))
                        .ReturnsAsync(patientJson);

            PdsService pdsService = pdsServiceMock.Object;

            // when
            Patient actualPatient =
                await pdsService.PatientLookupByNhsNumberAsync(inputNhsNumber);

            //then
            actualPatient.Should().BeEquivalentTo(expectedPatient);

            pdsServiceMock.Verify(service =>
                service.MapToPatientsFromPatientBundle(patientJson),
                    Times.Once());

            this.nhsDigitalApiBrokerMock.Verify(broker =>
                broker.SearchPatientPDSAsync(
                    It.IsAny<NhsDigitalSearchCriteria>(),
                    CancellationToken.None),
                        Times.Once);

            this.nhsDigitalApiBrokerMock.VerifyNoOtherCalls();
            this.pdsBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
