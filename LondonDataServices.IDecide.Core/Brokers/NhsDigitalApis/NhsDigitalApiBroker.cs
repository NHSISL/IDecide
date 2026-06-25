// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Text.Json;
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

        public async ValueTask<string> BuildLoginUrlAsync(CancellationToken cancellationToken)
        {
            return await this.apiPlatformClient
                .CareIdentityServiceClient
                .BuildLoginUrlAsync(cancellationToken);
        }

        public async ValueTask<string> GetAccessTokenAsync(CancellationToken cancellationToken)
        {
            return await this.apiPlatformClient
                .CareIdentityServiceClient
                .GetAccessTokenAsync(cancellationToken);
        }

        public async ValueTask LogoutAsync(CancellationToken cancellationToken)
        {
            await this.apiPlatformClient
                .CareIdentityServiceClient
                .LogoutAsync(cancellationToken);
        }

        public async ValueTask<string> GetUserInfoAsync(
            string code,
            string state,
            CancellationToken cancellationToken)
        {
            var userInfo = await this.apiPlatformClient
                .CareIdentityServiceClient
                .GetUserInfoAsync(code, state, cancellationToken);

            return userInfo is not null
                ? JsonSerializer.Serialize(userInfo)
                : string.Empty;
        }
    }
}