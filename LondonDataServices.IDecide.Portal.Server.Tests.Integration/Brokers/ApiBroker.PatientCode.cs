// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using LondonDataServices.IDecide.Portal.Server.Models;

namespace LondonDataServices.IDecide.Portal.Server.Tests.Integration.Brokers
{
    public partial class ApiBroker
    {
        private const string patientCodeRelativeUrl = "api/patientcode";

        public async ValueTask PostPatientGenerationRequestAsync(PatientCodeRequest patientCodeRequest)
        {
            await authenticatedApiFactoryClient.PostContentAsync(
                $"{patientCodeRelativeUrl}/PatientGenerationRequest", patientCodeRequest);
        }

        public async ValueTask VerifyPatientCodeAsync(PatientCodeRequest patientCodeRequest)
        {
            await authenticatedApiFactoryClient.PostContentAsync(
                $"{patientCodeRelativeUrl}/VerifyPatientCode", patientCodeRequest);
        }
    }
}
