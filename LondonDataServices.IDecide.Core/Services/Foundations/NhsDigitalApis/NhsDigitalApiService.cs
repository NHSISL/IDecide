// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Brokers.Loggings;
using LondonDataServices.IDecide.Core.Brokers.NhsDigitalApi;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.Pds;

namespace LondonDataServices.IDecide.Core.Services.Foundations.NhsDigitalApis
{
    public partial class NhsDigitalApiService : INhsDigitalApiService
    {
        private readonly INhsDigitalApiBroker nhsDigitalApiBroker;
        private readonly ILoggingBroker loggingBroker;

        public NhsDigitalApiService(
            INhsDigitalApiBroker nhsDigitalApiBroker,
            ILoggingBroker loggingBroker)
        {
            this.nhsDigitalApiBroker = nhsDigitalApiBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<string> BuildLoginUrlAsync(CancellationToken cancellationToken) =>
            TryCatch(async () =>
            {
                return await this.nhsDigitalApiBroker.BuildLoginUrlAsync(cancellationToken);
            });

        public ValueTask LogoutAsync(CancellationToken cancellationToken) =>
            ValueTask.FromException(new NotImplementedException());

        public ValueTask<string> GetAccessTokenAsync(CancellationToken cancellationToken) =>
            TryCatch(async () =>
            {
                ValidateCancellationTokenIsNotCancelled(cancellationToken);

                return await this.nhsDigitalApiBroker.GetAccessTokenAsync(cancellationToken);
            });

        public ValueTask<string> SearchPatientPDSAsync(
            SearchCriteria searchCriteria,
            CancellationToken cancellationToken) =>
            TryCatch(async () =>
            {
                ValidateSearchCriteriaIsNotNull(searchCriteria);

                return await this.nhsDigitalApiBroker.SearchPatientPDSAsync(
                    searchCriteria,
                    cancellationToken);
            });
    }
}
