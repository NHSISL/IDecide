// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Brokers
{
    public partial class ApiBroker
    {
        private const string featuresRelativeUrl = "api/Features";

        public async ValueTask<string[]> GetFeaturesAsync() =>
            await this.apiFactoryClient.GetContentAsync<string[]>(featuresRelativeUrl);
    }
}
