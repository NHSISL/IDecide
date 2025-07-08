// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using LondonDataServices.IDecide.Manage.Server.Tests.Integration.ReIdentification.Brokers;
using LondonDataServices.IDecide.Manage.Server.Tests.Integration.Brokers;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Integration.Apis
{
    [Collection(nameof(ApiTestCollection))]
    public partial class FrontendConfigurationsApiTests
    {
        private readonly ApiBroker apiBroker;

        public FrontendConfigurationsApiTests(ApiBroker apiBroker) =>
            this.apiBroker = apiBroker;
    }
}
