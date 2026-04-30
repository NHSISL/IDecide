// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;

namespace LondonDataServices.IDecide.Core.Services.Orchestrations.NhsDigitalApis
{
    public interface INhsDigitalApiOrchestrationService
    {
        ValueTask ProcessCallbackAsync(string code, string state, CancellationToken cancellationToken);
    }
}
