// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Foundations.Decisions;

namespace LondonDataServices.IDecide.Core.Services.Foundations.Decisions
{
    public interface IDecisionService
    {
        ValueTask<Decision> AddDecisionAsync(Decision decision);
        ValueTask<IQueryable<Decision>> RetrieveAllDecisionsAsync();
        ValueTask<Decision> RetrieveDecisionByIdAsync(Guid decisionId);
        ValueTask<Decision> ModifyDecisionAsync(Decision decision);
        ValueTask<Decision> RemoveDecisionByIdAsync(Guid decisionId);
    }
}