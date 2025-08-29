// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------



using System;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Portal.Tests.Integration.Models.OdsDatas;

namespace LondonDataServices.IDecide.Portals.Server.Tests.Integration.ReIdentification.Brokers
{
    public partial class ApiBroker
    {
        private const string odsDataRelativeUrl = "api/odsData";

        public async ValueTask<OdsData> PostOdsDataAsync(OdsData odsData) =>
            await this.apiFactoryClient.PostContentAsync(odsDataRelativeUrl, odsData);

        public async ValueTask<OdsData> DeleteOdsDataByIdAsync(Guid odsDataId) =>
            await this.apiFactoryClient.DeleteContentAsync<OdsData>($"{odsDataRelativeUrl}/{odsDataId}");
    }
}
