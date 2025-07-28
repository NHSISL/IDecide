// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Foundations.Decisions;

namespace LondonDataServices.IDecide.Core.Brokers.Storages.Sql
{
    public partial interface IStorageBroker
    {
        ValueTask<Decision> InsertDecisionAsync(Decision decision);
        ValueTask<IQueryable<Decision>> SelectAllDecisionsAsync();
        ValueTask<Decision> SelectDecisionByIdAsync(Guid decisionId);
        ValueTask<Decision> UpdateDecisionAsync(Decision decision);
        ValueTask<Decision> DeleteDecisionAsync(Decision decision);
    }
}
