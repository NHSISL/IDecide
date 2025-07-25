// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using LondonDataServices.IDecide.Core.Models.Foundations.DecisionTypes;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace LondonDataServices.IDecide.Core.Services.Foundations.DecisionTypes
{
    public interface IDecisionTypeService
    {
        ValueTask<DecisionType> AddDecisionTypeAsync(DecisionType decisionType);
        ValueTask<IQueryable<DecisionType>> RetrieveAllDecisionTypesAsync();
        ValueTask<DecisionType> RetrieveDecisionTypeByIdAsync(Guid decisionTypeId);
        ValueTask<DecisionType> ModifyDecisionTypeAsync(DecisionType decisionType);
        ValueTask<DecisionType> RemoveDecisionTypeByIdAsync(Guid decisionTypeId);
    }
}