// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;

namespace LondonDataServices.IDecide.Core.Brokers.Identifiers
{
    public class IdentifierBroker : IIdentifierBroker
    {
        public async ValueTask<Guid> GetIdentifierAsync() =>
            Guid.NewGuid();
    }
}
