// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.Pds;

namespace LondonDataServices.IDecide.Core.Brokers.NhsDigitalApi
{
    public interface INhsDigitalApiBroker
    {
        ValueTask<string> SearchPatientPDSAsync(
            SearchCriteria searchCriteria,
            CancellationToken cancellationToken);

        ValueTask<string> BuildLoginUrlAsync(CancellationToken cancellationToken);
        ValueTask<string> GetAccessTokenAsync(CancellationToken cancellationToken);
        ValueTask LogoutAsync(CancellationToken cancellationToken);

        ValueTask<string> GetUserInfoAsync(
            string code,
            string state,
            CancellationToken cancellationToken);
    }
}