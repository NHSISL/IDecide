// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using LondonDataServices.IDecide.Core.Models.Foundations.Pds;
using LondonDataServices.IDecide.Portal.Server.Tests.Integration.Models.Patients;

namespace LondonDataServices.IDecide.Portal.Server.Tests.Integration.Apis.PatientSearches
{
    public partial class PatientSearchTests
    {
        [Fact]
        public async Task ShouldPostPatientSearch()
        {
            // given
            string inputNhsNumber = "9449304424";
            PatientLookup randomPatientLookup = GetRandomSearchPatientLookup(inputNhsNumber);
            PatientLookup inputPatientLookup = randomPatientLookup.DeepClone();
            Patient expectedPatient = GetExpectedPatient();

            // when
            Patient actualPatient =
                await this.apiBroker.PostPatientSearchAsync(inputPatientLookup);

            // then
            actualPatient.Should().BeEquivalentTo(expectedPatient);
        }
    }
}
