// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Brokers.DateTimes;
using LondonDataServices.IDecide.Core.Brokers.Loggings;
using LondonDataServices.IDecide.Core.Brokers.Securities;
using LondonDataServices.IDecide.Core.Brokers.Storages.Sql;
using LondonDataServices.IDecide.Core.Models.Foundations.DecisionTypes;

namespace LondonDataServices.IDecide.Core.Services.Foundations.DecisionTypes
{
    public partial class DecisionTypeService : IDecisionTypeService
    {
        private readonly IStorageBroker storageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ISecurityBroker securityBroker;
        private readonly ISecurityAuditBroker securityAuditBroker;
        private readonly ILoggingBroker loggingBroker;

        public DecisionTypeService(
            IStorageBroker storageBroker,
            IDateTimeBroker dateTimeBroker,
            ISecurityBroker securityBroker,
            ISecurityAuditBroker securityAuditBroker,
            ILoggingBroker loggingBroker)
        {
            this.storageBroker = storageBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.securityBroker = securityBroker;
            this.securityAuditBroker = securityAuditBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<DecisionType> AddDecisionTypeAsync(DecisionType decisionType) =>
            TryCatch(async () =>
            {
                decisionType = await this.securityAuditBroker.ApplyAddAuditAsync(decisionType);
                await ValidateDecisionTypeOnAdd(decisionType);

                return await this.storageBroker.InsertDecisionTypeAsync(decisionType);
            });

        public ValueTask<IQueryable<DecisionType>> RetrieveAllDecisionTypesAsync() =>
            TryCatch(async () => await this.storageBroker.SelectAllDecisionTypesAsync());

        public ValueTask<DecisionType> RetrieveDecisionTypeByIdAsync(Guid decisionTypeId) =>
            TryCatch(async () =>
            {
                ValidateDecisionTypeId(decisionTypeId);

                DecisionType maybeDecisionType = await this.storageBroker
                    .SelectDecisionTypeByIdAsync(decisionTypeId);

                ValidateStorageDecisionType(maybeDecisionType, decisionTypeId);

                return maybeDecisionType;
            });

        public ValueTask<DecisionType> ModifyDecisionTypeAsync(DecisionType decisionType) =>
            TryCatch(async () =>
            {
                decisionType = await this.securityAuditBroker.ApplyModifyAuditAsync(decisionType);

                await ValidateDecisionTypeOnModify(decisionType);

                DecisionType maybeDecisionType =
                    await this.storageBroker.SelectDecisionTypeByIdAsync(decisionType.Id);

                ValidateStorageDecisionType(maybeDecisionType, decisionType.Id);

                decisionType = await this.securityAuditBroker
                    .EnsureAddAuditValuesRemainsUnchangedOnModifyAsync(decisionType, maybeDecisionType);

                ValidateAgainstStorageDecisionTypeOnModify(
                    inputDecisionType: decisionType,
                    storageDecisionType: maybeDecisionType);

                return await this.storageBroker.UpdateDecisionTypeAsync(decisionType);
            });

        public ValueTask<DecisionType> RemoveDecisionTypeByIdAsync(Guid decisionTypeId) =>
            TryCatch(async () =>
            {
                ValidateDecisionTypeId(decisionTypeId);

                DecisionType maybeDecisionType = await this.storageBroker
                    .SelectDecisionTypeByIdAsync(decisionTypeId);

                ValidateStorageDecisionType(maybeDecisionType, decisionTypeId);

                return await this.storageBroker.DeleteDecisionTypeAsync(maybeDecisionType);
            });
    }
}