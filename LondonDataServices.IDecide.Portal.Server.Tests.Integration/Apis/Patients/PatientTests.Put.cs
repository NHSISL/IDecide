// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Portal.Server.Tests.Integration.Models.Patients;

namespace LondonDataServices.IDecide.Portal.Server.Tests.Integration.Apis
{
    public partial class PatientApiTests
    {
        [Fact]
        public async Task ShouldPutPatientAsync()
        {
            // given
            Patient randomPatient = await PostRandomPatientAsync();
            Patient modifiedPatient = UpdatePatientWithRandomValues(randomPatient);

            // when
            await this.apiBroker.PutPatientAsync(modifiedPatient);
            Patient actualPatient = await this.apiBroker.GetPatientByIdAsync(randomPatient.Id);

            // then
            actualPatient.Should().BeEquivalentTo(
                modifiedPatient,
                options => options
                    .Excluding(patient => patient.CreatedBy)
                    .Excluding(patient => patient.CreatedDate)
                    .Excluding(patient => patient.UpdatedBy)
                    .Excluding(patient => patient.UpdatedDate));

            await this.apiBroker.DeletePatientByIdAsync(actualPatient.Id);
        }
    }
}
