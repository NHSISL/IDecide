// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using LondonDataServices.IDecide.Core.Models.Foundations.Pds;

namespace LondonDataServices.IDecide.Portals.Server.Tests.Acceptance.Apis
{
    public partial class PatientSearchApiTests
    {
        [Fact]
        public async Task ShouldPostPatientAsync()
        {
            // given
            string inputSurname = "Smith";
            PatientLookup randomPatientLookup = GetRandomSearchPatientLookup(inputSurname);
            PatientLookup inputPatientLookup = randomPatientLookup.DeepClone();
            Patient expectedPatient = GetPatient(inputSurname);

            // when
            Patient actualPatient =
                await this.apiBroker.PostPatientSearchAsync(inputPatientLookup);

            // then
            actualPatient.Should().BeEquivalentTo(expectedPatient);
        }
    }
}