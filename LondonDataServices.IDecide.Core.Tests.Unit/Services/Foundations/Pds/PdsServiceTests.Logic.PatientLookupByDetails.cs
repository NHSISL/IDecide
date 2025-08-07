// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using FluentAssertions;
using Force.DeepCloner;
using Hl7.Fhir.Model;
using ISL.Providers.PDS.Abstractions.Models;
using LondonDataServices.IDecide.Core.Models.Foundations.Pds;
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
            updatedPatientLookup.Patients = new List<Patient> { GetRandomPatient(inputSurname) }; ;
            PatientLookup expectedPatientLookup = updatedPatientLookup.DeepClone();

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

            // when
            PatientLookup actualPatientLookup = await this.pdsService.PatientLookupByDetailsAsync(inputPatientLookup);

            //then
            actualPatientLookup.Should().BeEquivalentTo(expectedPatientLookup);

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