// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.Pds;

namespace LondonDataServices.IDecide.Core.Services.Foundations.NhsDigitalApis
{
    public interface INhsDigitalApiService
    {
        ValueTask<string> SearchPatientPDSAsync(
            SearchCriteria searchCriteria,
            CancellationToken cancellationToken);

        ValueTask<string> BuildLoginUrlAsync(CancellationToken cancellationToken);
    }
}
