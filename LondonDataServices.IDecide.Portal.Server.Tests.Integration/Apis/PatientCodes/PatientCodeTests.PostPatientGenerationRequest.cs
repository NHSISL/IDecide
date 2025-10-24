// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Force.DeepCloner;
using LondonDataServices.IDecide.Portal.Server.Models;
using LondonDataServices.IDecide.Portal.Server.Tests.Integration.Models.Patients;

namespace LondonDataServices.IDecide.Portal.Server.Tests.Integration.Apis.PatientCodes
{
    public partial class PatientCodeTests
    {
        [Fact]
        public async Task ShouldPostPatientGenerationRequestAsync()
        {
            // given
            string nhsNumber = "9449304424";
            Patient randomPatient = await PostRandomPatientAsync(nhsNumber);
            PatientCodeRequest randomPatientCodeRequest = CreateRandomPatientCodeRequest(randomPatient);
            PatientCodeRequest inputPatientCodeRequest = randomPatientCodeRequest.DeepClone();

            // when
            await this.apiBroker.PostPatientGenerationRequestAsync(inputPatientCodeRequest);

            // then
            List<Patient> patients = await this.apiBroker.GetAllPatientsAsync();

            Patient addedPatient = patients
                .First(patient => patient.NhsNumber == inputPatientCodeRequest.NhsNumber);

            await this.apiBroker.DeletePatientByIdAsync(addedPatient.Id);
        }
    }
}
