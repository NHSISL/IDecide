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
using LondonDataServices.IDecide.Core.Models.Foundations.Decisions;

namespace LondonDataServices.IDecide.Core.Services.Foundations.Decisions
{
    public partial class DecisionService : IDecisionService
    {
        private readonly IStorageBroker storageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ISecurityBroker securityBroker;
        private readonly ISecurityAuditBroker securityAuditBroker;
        private readonly ILoggingBroker loggingBroker;

        public DecisionService(
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

        public ValueTask<Decision> AddDecisionAsync(Decision decision) =>
            TryCatch(async () =>
            {
                decision = await this.securityAuditBroker.ApplyAddAuditValuesAsync(decision);
                await ValidateDecisionOnAdd(decision);

                return await this.storageBroker.InsertDecisionAsync(decision);
            });

        public ValueTask<IQueryable<Decision>> RetrieveAllDecisionsAsync() =>
            TryCatch(async () => await this.storageBroker.SelectAllDecisionsAsync());

        public ValueTask<Decision> RetrieveDecisionByIdAsync(Guid decisionId) =>
            TryCatch(async () =>
            {
                ValidateDecisionId(decisionId);

                Decision maybeDecision = await this.storageBroker
                    .SelectDecisionByIdAsync(decisionId);

                ValidateStorageDecision(maybeDecision, decisionId);

                return maybeDecision;
            });

        public ValueTask<Decision> ModifyDecisionAsync(Decision decision) =>
            TryCatch(async () =>
            {
                decision = await this.securityAuditBroker.ApplyModifyAuditValueAsync(decision);

                await ValidateDecisionOnModify(decision);

                Decision maybeDecision =
                    await this.storageBroker.SelectDecisionByIdAsync(decision.Id);

                ValidateStorageDecision(maybeDecision, decision.Id);

                decision = await this.securityAuditBroker
                    .EnsureAddAuditValuesRemainsUnchangedOnModifyAsync(decision, maybeDecision);

                ValidateAgainstStorageDecisionOnModify(
                    inputDecision: decision,
                    storageDecision: maybeDecision);

                return await this.storageBroker.UpdateDecisionAsync(decision);
            });

        public ValueTask<Decision> RemoveDecisionByIdAsync(Guid decisionId) =>
            TryCatch(async () =>
            {
                ValidateDecisionId(decisionId);

                Decision maybeDecision = await this.storageBroker
                    .SelectDecisionByIdAsync(decisionId);

                ValidateStorageDecision(maybeDecision, decisionId);

                return await this.storageBroker.DeleteDecisionAsync(maybeDecision);
            });
    }
}