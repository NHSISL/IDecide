// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using LondonDataServices.IDecide.Portal.Server.Tests.Integration.Models.Patient;
using RESTFulSense.Exceptions;

namespace LondonDataServices.IDecide.Portal.Server.Tests.Integration.Apis
{
    public partial class PatientApiTests
    {
        [Fact]
        public async Task ShouldDeletePatientAsync()
        {
            // given
            Patient randomPatient = await PostRandomPatientAsync();

            // when
            await this.apiBroker.DeletePatientByIdAsync(randomPatient.Id);

            // then
            ValueTask<Patient> getPatientByIdTask = 
                this.apiBroker.GetPatientByIdAsync(randomPatient.Id);

            await Assert.ThrowsAsync<HttpResponseNotFoundException>(getPatientByIdTask.AsTask);
        }
    }
}
