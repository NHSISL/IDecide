// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using Force.DeepCloner;
using LondonDataServices.IDecide.Manage.Server.Models;
using LondonDataServices.IDecide.Manage.Server.Tests.Integration.Models.Patients;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Integration.Apis.PatientCodes
{
    public partial class PatientCodeTests
    {
        [Fact]
        public async Task ShouldVerifyPatientCodeAsync()
        {
            // given
            Patient randomPatient = await PostRandomPatientAsync();
            PatientCodeRequest randomPatientCodeRequest = CreateRandomPatientCodeRequest(randomPatient);
            PatientCodeRequest inputPatientCodeRequest = randomPatientCodeRequest.DeepClone();

            // when
            await this.apiBroker.VerifyPatientCodeAsync(inputPatientCodeRequest);

            // then
            await this.apiBroker.DeletePatientByIdAsync(randomPatient.Id);
        }
    }
}
