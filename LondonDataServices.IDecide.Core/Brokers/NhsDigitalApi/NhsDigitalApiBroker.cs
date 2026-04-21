// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using NHSDigital.ApiPlatform.Sdk.Clients.ApiPlatforms;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.Pds;

namespace LondonDataServices.IDecide.Core.Brokers.NhsDigitalApi
{
    public class NhsDigitalApiBroker : INhsDigitalApiBroker
    {
        private readonly IApiPlatformClient apiPlatformClient;

        public NhsDigitalApiBroker(
            IApiPlatformClient apiPlatformClient)
        {
            this.apiPlatformClient = apiPlatformClient;
        }

        public async ValueTask<string> SearchPatientPDSAsync(
            SearchCriteria searchCriteria,
            CancellationToken cancellationToken)
        {
            string jsonResponse = await this.apiPlatformClient
                .PersonalDemographicsServiceClient
                .SearchPatientsAsync(
                    searchCriteria,
                    cancellationToken: cancellationToken);

            return jsonResponse;
        }
    }
}