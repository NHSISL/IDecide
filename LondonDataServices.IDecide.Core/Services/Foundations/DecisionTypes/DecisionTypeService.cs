// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using LondonDataServices.IDecide.Core.Models.Foundations.DecisionTypes;
using System.Linq;
using System.Threading.Tasks;
using System;
using LondonDataServices.IDecide.Core.Brokers.Storages.Sql;
using LondonDataServices.IDecide.Core.Brokers.DateTimes;
using LondonDataServices.IDecide.Core.Brokers.Securities;
using LondonDataServices.IDecide.Core.Brokers.Loggings;

namespace LondonDataServices.IDecide.Core.Services.Foundations.DecisionTypes
{
    public partial class DecisionTypeService : IDecisionTypeService
    {
        private readonly IStorageBroker storageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ISecurityBroker securityBroker;
        private readonly ILoggingBroker loggingBroker;

        public DecisionTypeService(
            IStorageBroker storageBroker,
            IDateTimeBroker dateTimeBroker,
            ISecurityBroker securityBroker,
            ILoggingBroker loggingBroker)
        {
            this.storageBroker = storageBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.securityBroker = securityBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<DecisionType> AddDecisionTypeAsync(DecisionType decisionType)
        {
            throw new NotImplementedException();
        }

        public ValueTask<DecisionType> ModifyDecisionTypeAsync(DecisionType decisionType) =>
            TryCatch(async () =>
            {
                await ValidateDecisionTypeOnModifyAsync(decisionType);

                DecisionType maybeDecisionType = 
                    await this.storageBroker.SelectDecisionTypeByIdAsync(decisionType.Id);

                ValidateStorageDecisionType(maybeDecisionType, decisionType.Id);

                ValidateAgainstStorageDecisionTypeOnModify(
                    inputDecisionType: decisionType,
                    storageDecisionType: maybeDecisionType);

                return await this.storageBroker.UpdateDecisionTypeAsync(decisionType);
            });

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