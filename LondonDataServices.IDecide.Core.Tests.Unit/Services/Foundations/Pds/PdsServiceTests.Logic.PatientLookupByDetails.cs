// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using FluentAssertions;
using Force.DeepCloner;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using LondonDataServices.IDecide.Core.Models.Foundations.Pds;
using LondonDataServices.IDecide.Core.Services.Foundations.Pds;
using Moq;
using Patient = LondonDataServices.IDecide.Core.Models.Foundations.Patients.Patient;
using Task = System.Threading.Tasks.Task;
using NhsDigitalSearchCriteria = NHSDigital.ApiPlatform.Sdk.Models.Foundations.Pds.SearchCriteria;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.Pds
{
    public partial class PdsServiceTests
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
            var serializer = new FhirJsonSerializer();
            string bundleJson = serializer.SerializeToString(randomBundle);
            PatientLookup updatedPatientLookup = randomPatientLookup.DeepClone();
            Patient mappedPatient = GetRandomPatient(inputSurname);
            List<Patient> mappedPatients = new List<Patient> { mappedPatient };
            updatedPatientLookup.Patients = mappedPatients;
            PatientLookup expectedPatientLookup = updatedPatientLookup.DeepClone();

            var pdsServiceMock = new Mock<PdsService>(
                this.pdsBrokerMock.Object,
                this.nhsDigitalApiBrokerMock.Object,
                this.loggingBrokerMock.Object)
            { CallBase = true };

            pdsServiceMock.Setup(service =>
                service.MapToPatientsFromBundleJson(bundleJson))
                    .Returns(mappedPatients);

            this.nhsDigitalApiBrokerMock.Setup(broker =>
                broker.SearchPatientPDSAsync(
                    It.IsAny<NhsDigitalSearchCriteria>(),
                    CancellationToken.None))
                        .ReturnsAsync(bundleJson);

            PdsService pdsService = pdsServiceMock.Object;

            // when
            PatientLookup actualPatientLookup = await pdsService.PatientLookupByDetailsAsync(inputPatientLookup);

            //then
            actualPatientLookup.Should().BeEquivalentTo(expectedPatientLookup);

            pdsServiceMock.Verify(service =>
                service.MapToPatientsFromBundleJson(bundleJson),
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