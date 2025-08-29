// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using FluentAssertions;
using LondonDataServices.IDecide.Portal.Server.Tests.Integration.Models.Patient;

namespace LondonDataServices.IDecide.Portal.Server.Tests.Integration.Apis
{
    public partial class PatientApiTests
    {
        [Fact]
        public async Task ShouldGetPatientByIdAsync()
        {
            // given
            Patient randomPatient = await PostRandomPatientAsync();
            Patient expectedPatient = randomPatient;

            // when
            Patient actualPatient = 
                await this.apiBroker.GetPatientByIdAsync(randomPatient.Id);

            // then
            actualPatient.Should().BeEquivalentTo(expectedPatient);
            await this.apiBroker.DeletePatientByIdAsync(actualPatient.Id);
        }
    }
}
