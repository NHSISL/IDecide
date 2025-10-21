// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using LondonDataServices.IDecide.Manage.Server.Models;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Brokers
{
    public partial class ApiBroker
    {
        private const string patientCodeRelativeUrl = "api/patientcode";

        public async ValueTask PostPatientGenerationRequestAsync(PatientCodeRequest patientCodeRequest)
        {
            await this.apiFactoryClient.PostContentAsync<PatientCodeRequest>(
                $"{patientCodeRelativeUrl}/PatientGenerationRequest", patientCodeRequest);
        }

        public async ValueTask VerifyPatientCodeAsync(PatientCodeRequest patientCodeRequest)
        {
            await this.apiFactoryClient.PostContentAsync<PatientCodeRequest>(
                $"{patientCodeRelativeUrl}/VerifyPatientCode", patientCodeRequest);
        }
    }
}
