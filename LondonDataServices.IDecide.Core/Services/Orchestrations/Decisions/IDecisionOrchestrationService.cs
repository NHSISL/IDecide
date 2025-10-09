// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Foundations.Decisions;

namespace LondonDataServices.IDecide.Core.Services.Orchestrations.Decisions
{
    public interface IDecisionOrchestrationService
    {
        ValueTask VerifyAndRecordDecisionAsync(Decision decision);

        ValueTask<List<Decision>> RetrieveAllPendingAdoptionDecisionsForConsumer(
            DateTimeOffset changesSinceDate, string decisionType);
    }
}
