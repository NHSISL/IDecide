// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

namespace LondonDataServices.IDecide.Manage.Server.Tests.Integration.ReIdentification.Brokers
{
    public partial class ApiBroker
    {
        private const string featuresRelativeUrl = "api/Features";

        public async ValueTask<string[]> GetFeaturesAsync() =>
            await this.apiFactoryClient.GetContentAsync<string[]>(featuresRelativeUrl);
    }
}