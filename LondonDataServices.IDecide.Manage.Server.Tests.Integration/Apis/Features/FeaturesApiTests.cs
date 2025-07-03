// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using LondonDataServices.IDecide.Manage.Server.Tests.Integration.Brokers;
using LondonDataServices.IDecide.Manage.Server.Tests.Integration.ReIdentification.Brokers;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Integration.Apis
{
    [Collection(nameof(ApiTestCollection))]
    public partial class FeaturesApiTests
    {
        private readonly ApiBroker apiBroker;

        public FeaturesApiTests(ApiBroker apiBroker) =>
            this.apiBroker = apiBroker;
    }
}