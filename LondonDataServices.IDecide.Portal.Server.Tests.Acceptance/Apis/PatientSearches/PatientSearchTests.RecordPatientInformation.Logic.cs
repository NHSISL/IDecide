// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Foundations.Pds;

namespace LondonDataServices.IDecide.Portal.Server.Tests.Acceptance.Apis.PatientSearches
{
    public partial class PatientSearchTests
    {
        [Fact]
        public async Task ShouldRecordPatientInformationAsync()
        {
            // given
            string inputSurname = "Smith";
            PatientLookup randomPatientLookup = GetRandomSearchPatientLookup(inputSurname);
            PatientLookup inputPatientLookup = randomPatientLookup.DeepClone();
            Patient expectedPatient = GetPatient(inputSurname);

            // when
            Patient actualPatient =
                await this.apiBroker.(inputPatientLookup);

            // then
            actualPatient.Should().BeEquivalentTo(expectedPatient);
        }
    }
}
