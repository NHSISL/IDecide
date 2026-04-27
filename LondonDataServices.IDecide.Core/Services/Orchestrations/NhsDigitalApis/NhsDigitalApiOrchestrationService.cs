// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Brokers.Loggings;
using LondonDataServices.IDecide.Core.Services.Foundations.NhsDigitalApis;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.Pds;

namespace LondonDataServices.IDecide.Core.Services.Orchestrations.NhsDigitalApis
{
    public partial class NhsDigitalApiOrchestrationService : INhsDigitalApiOrchestrationService
    {
        private readonly ILoggingBroker loggingBroker;
        private readonly INhsDigitalApiService nhsDigitalApiService;

        public NhsDigitalApiOrchestrationService(
            ILoggingBroker loggingBroker,
            INhsDigitalApiService nhsDigitalApiService)
        {
            this.loggingBroker = loggingBroker;
            this.nhsDigitalApiService = nhsDigitalApiService;
        }

        public ValueTask<string> SearchPatientPDSAsync(
            SearchCriteria searchCriteria,
            CancellationToken cancellationToken) =>
                this.nhsDigitalApiService.SearchPatientPDSAsync(
                    searchCriteria,
                    cancellationToken);
    }
}
