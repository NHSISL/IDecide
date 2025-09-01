// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using LondonDataServices.IDecide.Portal.Server.Tests.Acceptance.Brokers;

namespace LondonDataServices.IDecide.Portal.Server.Tests.Acceptance.Apis
{
    [Collection(nameof(ApiTestCollection))]
    public partial class PatientDecisionsApiTests
    {
        private readonly ApiBroker apiBroker;

        public PatientDecisionsApiTests(ApiBroker apiBroker) =>
            this.apiBroker = apiBroker;
    }
}
