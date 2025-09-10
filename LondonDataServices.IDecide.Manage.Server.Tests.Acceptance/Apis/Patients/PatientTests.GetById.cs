// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Models.Patients;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Apis.Patients
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
            actualPatient.Should().BeEquivalentTo(expectedPatient, options => options
                .Excluding(property => property.CreatedBy)
                .Excluding(property => property.CreatedDate)
                .Excluding(property => property.UpdatedBy)
                .Excluding(property => property.UpdatedDate));

            await this.apiBroker.DeletePatientByIdAsync(actualPatient.Id);
        }
    }
}