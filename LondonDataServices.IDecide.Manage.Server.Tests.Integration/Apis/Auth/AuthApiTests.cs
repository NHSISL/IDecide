// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using LondonDataServices.IDecide.Manage.Server.Tests.Integration.Brokers;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Integration.Apis.Auth
{
    [Collection(nameof(ApiTestCollection))]
    public partial class AuthApiTests
    {
        private readonly ApiBroker apiBroker;

        public AuthApiTests(ApiBroker apiBroker) =>
            this.apiBroker = apiBroker;
    }
}
