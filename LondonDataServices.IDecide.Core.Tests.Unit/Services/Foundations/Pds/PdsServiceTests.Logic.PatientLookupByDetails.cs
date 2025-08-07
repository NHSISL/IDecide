// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Force.DeepCloner;
using Hl7.Fhir.Model;
using ISL.Providers.PDS.Abstractions.Models;
using LondonDataServices.IDecide.Core.Models.Foundations.Pds;
using LondonDataServices.IDecide.Core.Services.Foundations.Pds;
using Moq;
using Patient = LondonDataServices.IDecide.Core.Models.Foundations.Patients.Patient;
using Task = System.Threading.Tasks.Task;

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
            PatientBundle outputPatientBundle = CreateRandomPatientBundle(randomBundle);
            PatientLookup updatedPatientLookup = randomPatientLookup.DeepClone();
            Patient mappedPatient = GetRandomPatient(inputSurname);
            List<Patient> mappedPatients = new List<Patient> { mappedPatient };
            updatedPatientLookup.Patients = mappedPatients;
            PatientLookup expectedPatientLookup = updatedPatientLookup.DeepClone();

            var pdsServiceMock = new Mock<PdsService>(
                this.pdsBrokerMock.Object,
                this.loggingBrokerMock.Object)
            { CallBase = true };

            pdsServiceMock.Setup(service => 
                service.MapToPatientsFromPatientBundle(outputPatientBundle))
                    .Returns(mappedPatients);

            this.pdsBrokerMock.Setup(broker =>
                broker.PatientLookupByDetailsAsync(
                    string.Empty,
                    inputPatientLookup.SearchCriteria.Surname,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty))
                        .ReturnsAsync(outputPatientBundle);

            PdsService pdsService = pdsServiceMock.Object;

            // when
            PatientLookup actualPatientLookup = await pdsService.PatientLookupByDetailsAsync(inputPatientLookup);

            //then
            actualPatientLookup.Should().BeEquivalentTo(expectedPatientLookup);

            pdsServiceMock.Verify(service =>
                service.MapToPatientsFromPatientBundle(outputPatientBundle),
                    Times.Once());

            this.pdsBrokerMock.Verify(broker =>
                broker.PatientLookupByDetailsAsync(
                    string.Empty,
                    inputPatientLookup.SearchCriteria.Surname,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty),
                        Times.Once);

            this.pdsBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}