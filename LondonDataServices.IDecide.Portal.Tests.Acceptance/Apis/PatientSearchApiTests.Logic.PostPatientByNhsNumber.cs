// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Portal.Server.Models.PatientSearches;

namespace LondonDataServices.IDecide.Portals.Server.Tests.Acceptance.Apis
{
    public partial class PatientSearchApiTests
    {
        [Fact]
        public async Task ShouldPostPatientByNhsNumberAsync()
        {
            // given
            string inputNhsNumber = "1234567890";
            Patient expectedPatient = GetPatient();

            // when
            Patient actualPatient =
                await this.apiBroker.PostPatientByNhsNumberAsync(inputNhsNumber);

            // then
            actualPatient.Should().BeEquivalentTo(expectedPatient);
        }
    }
}