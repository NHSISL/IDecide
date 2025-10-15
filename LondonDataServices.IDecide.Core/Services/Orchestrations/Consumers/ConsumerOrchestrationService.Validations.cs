// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using LondonDataServices.IDecide.Core.Models.Foundations.Decisions;
using LondonDataServices.IDecide.Core.Models.Orchestrations.Consumers.Exceptions;

namespace LondonDataServices.IDecide.Core.Services.Orchestrations.Consumers
{
    public partial class ConsumerOrchestrationService
    {
        private void ValidateDecisions(List<Decision> decisions)
        {
            if (decisions is null || !decisions.Any())
            {
                throw new InvalidDecisionsException("Decisions required.");
            }
        }

        private void ValidateDecisionIds(List<Guid> decisionIds)
        {
            if (decisionIds is null || !decisionIds.Any())
            {
                throw new InvalidDecisionIdsException("Decision Ids required.");
            }
        }
    }
}
