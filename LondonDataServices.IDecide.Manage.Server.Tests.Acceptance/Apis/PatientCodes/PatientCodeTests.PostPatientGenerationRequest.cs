// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Force.DeepCloner;
using LondonDataServices.IDecide.Manage.Server.Models;
using LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Models.Patients;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Apis.PatientCodes
{
    public partial class PatientCodeTests
    {
        [Fact]
        public async Task ShouldPostPatientGenerationRequestAsync()
        {
            // given
            PatientCodeRequest randomPatientCodeRequest = CreateRandomPatientCodeRequest("1234567890");
            PatientCodeRequest inputPatientCodeRequest = randomPatientCodeRequest.DeepClone();

            // when
            await this.apiBroker.PostPatientGenerationRequestAsync(inputPatientCodeRequest);

            // then
            List<Patient> patients = await apiBroker.GetAllPatientsAsync();

            Patient addedPatient = patients
                .FirstOrDefault(patient => patient.NhsNumber == inputPatientCodeRequest.NhsNumber);

            await this.apiBroker.DeletePatientByIdAsync(addedPatient.Id);
        }
    }
}
