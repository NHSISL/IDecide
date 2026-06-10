// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Foundations.Users;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.Pds;

namespace LondonDataServices.IDecide.Core.Services.Orchestrations.NhsDigitalApis
{
    public interface INhsDigitalApiOrchestrationService
    {
        ValueTask<User> ProcessCallbackAsync(
            string code,
            string state,
            CancellationToken cancellationToken);

        ValueTask LogoutAsync(CancellationToken cancellationToken);
        ValueTask<string> BuildLoginUrlAsync(CancellationToken cancellationToken);
        ValueTask<string> GetAccessTokenAsync(CancellationToken cancellationToken);

        ValueTask<string> SearchPatientPDSAsync(
            SearchCriteria searchCriteria,
            CancellationToken cancellationToken);
    }
}
