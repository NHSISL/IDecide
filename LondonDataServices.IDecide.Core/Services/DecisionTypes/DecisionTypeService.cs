// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using LondonDataServices.IDecide.Core.Models.Foundations.DecisionTypes;
using System.Linq;
using System.Threading.Tasks;
using System;
using LondonDataServices.IDecide.Core.Brokers.Storages.Sql;

namespace LondonDataServices.IDecide.Core.Services.DecisionTypes
{
    public class DecisionTypeService : IDecisionTypeService
    {
        private readonly IStorageBroker storageBroker;

        public DecisionTypeService(IStorageBroker storageBroker)
        {
            this.storageBroker = storageBroker;
        }

        public async ValueTask<DecisionType> AddDecisionTypeAsync(DecisionType decisionType)
        {
            return await this.storageBroker.InsertDecisionTypeAsync(decisionType);
        }

        public ValueTask<DecisionType> ModifyDecisionTypeAsync(DecisionType decisionType)
        {
            throw new NotImplementedException();
        }

        public ValueTask<DecisionType> RemoveDecisionTypeByIdAsync(Guid decisionTypeId)
        {
            throw new NotImplementedException();
        }

        public ValueTask<IQueryable<DecisionType>> RetrieveAllDecisionTypesAsync()
        {
            throw new NotImplementedException();
        }

        public ValueTask<DecisionType> RetrieveDecisionTypeByIdAsync(Guid decisionTypeId)
        {
            throw new NotImplementedException();
        }
    }
}
