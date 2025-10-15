// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Foundations.Decisions;

namespace LondonDataServices.IDecide.Core.Services.Orchestrations.Consumers
{
    public interface IConsumerOrchestrationService
    {
        ValueTask AdoptPatientDecisionsAsync(List<Decision> decisions);
        ValueTask RecordConsumerAdoptionAsync(List<Guid> decisionIds);
    }
}
