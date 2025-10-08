// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using LondonDataServices.IDecide.Core.Models.Foundations.Pds;
using LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Models.Patients;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Apis.PatientSearches
{
    public partial class PatientSearchTests
    {
        [Fact]
        public async Task ShouldPostPatientSearch()
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
