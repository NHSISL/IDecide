// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using LondonDataServices.IDecide.Portals.Server.Tests.Acceptance.Brokers;
using LondonDataServices.IDecide.Portal.Tests.Acceptance.Brokers;

namespace LondonDataServices.IDecide.Portals.Server.Tests.Acceptance.Apis
{
    [Collection(nameof(ApiTestCollection))]
    public partial class FeaturesApiTests
    {
        private readonly ApiBroker apiBroker;

        public FeaturesApiTests(ApiBroker apiBroker) =>
            this.apiBroker = apiBroker;
    }
}