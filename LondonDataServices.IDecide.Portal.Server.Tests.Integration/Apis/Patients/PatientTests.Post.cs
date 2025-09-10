// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Portal.Server.Tests.Integration.Models.Patients;

namespace LondonDataServices.IDecide.Portal.Server.Tests.Integration.Apis.Patients
{
    public partial class PatientApiTests
    {
        [Fact]
        public async Task ShouldPostPatientAsync()
        {
            // given
            Patient randomPatient = CreateRandomPatient();
            Patient expectedPatient = randomPatient;

            // when 
            await this.apiBroker.PostPatientAsync(randomPatient);

            Patient actualPatient =
                await this.apiBroker.GetPatientByIdAsync(randomPatient.Id);

            // then
            actualPatient.Should().BeEquivalentTo(
                expectedPatient,
                options => options
                    .Excluding(patient => patient.CreatedBy)
                    .Excluding(patient => patient.CreatedDate)
                    .Excluding(patient => patient.UpdatedBy)
                    .Excluding(patient => patient.UpdatedDate));

            await this.apiBroker.DeletePatientByIdAsync(actualPatient.Id);
        }
    }
}
