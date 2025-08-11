// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using LondonDataServices.IDecide.Portal.Server.Models.PatientSearches;

namespace LondonDataServices.IDecide.Portals.Server.Tests.Acceptance.Apis
{
    public partial class PatientSearchApiTests
    {
        [Fact]
        public async Task ShouldPostPatientByDetailsAsync()
        {
            // given
            string inputSurname = "Smith";
            PatientLookup randomPatientLookup = GetRandomSearchPatientLookup(inputSurname);
            PatientLookup inputPatientLookup = randomPatientLookup.DeepClone();
            Patient expectedPatient = GetPatient(inputSurname);

            // when
            Patient actualPatient =
                await this.apiBroker.PostPatientByDetailsAsync(inputPatientLookup);

            // then
            actualPatient.Should().BeEquivalentTo(expectedPatient);
        }
    }
}