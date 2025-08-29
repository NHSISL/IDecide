// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using LondonDataServices.IDecide.Portal.Server.Tests.Integration.ReIdentification.Brokers;

namespace LondonDataServices.IDecide.Portal.Server.Tests.Integration.Brokers
{
    [CollectionDefinition(nameof(ApiTestCollection))]
    public class ApiTestCollection : ICollectionFixture<ApiBroker>
    { }
}
