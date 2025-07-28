// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Foundations.DecisionTypes;

namespace LondonDataServices.IDecide.Core.Brokers.Storages.Sql
{
    public partial interface IStorageBroker
    {
        ValueTask<DecisionType> InsertDecisionTypeAsync(DecisionType decisionType);
        ValueTask<IQueryable<DecisionType>> SelectAllDecisionTypesAsync();
        ValueTask<DecisionType> SelectDecisionTypeByIdAsync(Guid decisionTypeId);
        ValueTask<DecisionType> UpdateDecisionTypeAsync(DecisionType decisionType);
        ValueTask<DecisionType> DeleteDecisionTypeAsync(DecisionType decisionType);
    }
}
