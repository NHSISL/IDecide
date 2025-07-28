// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Foundations.DecisionTypes;
using Microsoft.EntityFrameworkCore;

namespace LondonDataServices.IDecide.Core.Brokers.Storages.Sql
{
    public partial class StorageBroker
    {
        public DbSet<DecisionType> DecisionTypes { get; set; }

        public async ValueTask<DecisionType> InsertDecisionTypeAsync(DecisionType decisionType) =>
            await InsertAsync(decisionType);

        public async ValueTask<IQueryable<DecisionType>> SelectAllDecisionTypesAsync() =>
            await SelectAllAsync<DecisionType>();

        public async ValueTask<DecisionType> SelectDecisionTypeByIdAsync(Guid decisionTypeId) =>
            await SelectAsync<DecisionType>(decisionTypeId);

        public async ValueTask<DecisionType> UpdateDecisionTypeAsync(DecisionType decisionType) =>
            await UpdateAsync(decisionType);

        public async ValueTask<DecisionType> DeleteDecisionTypeAsync(DecisionType decisionType) =>
            await DeleteAsync(decisionType);
    }
}
