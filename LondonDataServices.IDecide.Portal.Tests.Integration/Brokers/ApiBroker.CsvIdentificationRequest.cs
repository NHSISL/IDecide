// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Portal.Tests.Integration.Models.CsvIdentificationRequests;

namespace LondonDataServices.IDecide.Portals.Server.Tests.Integration.ReIdentification.Brokers
{
    public partial class ApiBroker
    {
        private const string csvIdentificationRequestRelativeUrl = "api/csvidentificationrequests";

        public async ValueTask<CsvIdentificationRequest> PostCsvIdentificationRequestAsync(
            CsvIdentificationRequest csvIdentificationRequest) =>
            await this.apiFactoryClient.PostContentAsync(csvIdentificationRequestRelativeUrl, csvIdentificationRequest);

        public async ValueTask<CsvIdentificationRequest> DeleteCsvIdentificationRequestByIdAsync(
            Guid csvIdentificationRequestId) =>
            await this.apiFactoryClient.DeleteContentAsync<CsvIdentificationRequest>(
                $"{csvIdentificationRequestRelativeUrl}/{csvIdentificationRequestId}");
    }
}
