// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading;
using LondonDataServices.IDecide.Core.Models.Foundations.NhsDigitalApis.Exceptions;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.Pds;

namespace LondonDataServices.IDecide.Core.Services.Foundations.NhsDigitalApis
{
    public partial class NhsDigitalApiService
    {
        private static void ValidateSearchCriteriaIsNotNull(SearchCriteria searchCriteria)
        {
            if (searchCriteria is null)
            {
                throw new NullNhsDigitalApiSearchCriteriaException(
                    message: "Search criteria is null.");
            }
        }

        private static void ValidateBuildLoginUrlCancellationToken(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                throw new CancelledNhsDigitalApiBuildLoginUrlException(
                    message: "Build login url cancelled, please try again.");
            }
        }
    }
}
