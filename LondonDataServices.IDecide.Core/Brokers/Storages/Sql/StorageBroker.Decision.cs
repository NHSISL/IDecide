// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Foundations.Decisions;
using Microsoft.EntityFrameworkCore;

namespace LondonDataServices.IDecide.Core.Brokers.Storages.Sql
{
    public partial class StorageBroker
    {
        public DbSet<Decision> Decisions { get; set; }

        public async ValueTask<Decision> InsertDecisionAsync(Decision decision) =>
            await InsertAsync(decision);

        public async ValueTask<IQueryable<Decision>> SelectAllDecisionsAsync() =>
            await SelectAllAsync<Decision>();

        public async ValueTask<Decision> SelectDecisionByIdAsync(Guid decisionId) =>
            await SelectAsync<Decision>(decisionId);

        public async ValueTask<Decision> UpdateDecisionAsync(Decision decision) =>
            await UpdateAsync(decision);

        public async ValueTask<Decision> DeleteDecisionAsync(Decision decision) =>
            await DeleteAsync(decision);
    }
}
