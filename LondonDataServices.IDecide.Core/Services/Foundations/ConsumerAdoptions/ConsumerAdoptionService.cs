// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Brokers.DateTimes;
using LondonDataServices.IDecide.Core.Brokers.Loggings;
using LondonDataServices.IDecide.Core.Brokers.Securities;
using LondonDataServices.IDecide.Core.Brokers.Storages.Sql;
using LondonDataServices.IDecide.Core.Models.Foundations.ConsumerAdoptions;

namespace LondonDataServices.IDecide.Core.Services.Foundations.ConsumerAdoptions
{
    public partial class ConsumerAdoptionService : IConsumerAdoptionService
    {
        private readonly IStorageBroker storageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ISecurityAuditBroker securityAuditBroker;
        private readonly ILoggingBroker loggingBroker;

        public ConsumerAdoptionService(
            IStorageBroker storageBroker,
            IDateTimeBroker dateTimeBroker,
            ISecurityAuditBroker securityAuditBroker,
            ILoggingBroker loggingBroker)
        {
            this.storageBroker = storageBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.securityAuditBroker = securityAuditBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<ConsumerAdoption> AddConsumerAdoptionAsync(ConsumerAdoption consumerAdoption) =>
            TryCatch(async () =>
            {
                consumerAdoption = await this.securityAuditBroker.ApplyAddAuditValuesAsync(consumerAdoption);
                await ValidateConsumerAdoptionOnAdd(consumerAdoption);

                return await this.storageBroker.InsertConsumerAdoptionAsync(consumerAdoption);
            });

        public ValueTask<IQueryable<ConsumerAdoption>> RetrieveAllConsumerAdoptionsAsync() =>
            TryCatch(async () => await this.storageBroker.SelectAllConsumerAdoptionsAsync());

        public ValueTask<ConsumerAdoption> RetrieveConsumerAdoptionByIdAsync(Guid consumerAdoptionId) =>
            TryCatch(async () =>
            {
                ValidateConsumerAdoptionId(consumerAdoptionId);

                ConsumerAdoption maybeConsumerAdoption = await this.storageBroker
                    .SelectConsumerAdoptionByIdAsync(consumerAdoptionId);

                ValidateStorageConsumerAdoption(maybeConsumerAdoption, consumerAdoptionId);

                return maybeConsumerAdoption;
            });

        public ValueTask<ConsumerAdoption> ModifyConsumerAdoptionAsync(ConsumerAdoption consumerAdoption) =>
            TryCatch(async () =>
            {
                consumerAdoption = await this.securityAuditBroker.ApplyModifyAuditValuesAsync(consumerAdoption);
                await ValidateConsumerAdoptionOnModify(consumerAdoption);

                ConsumerAdoption maybeConsumerAdoption =
                    await this.storageBroker.SelectConsumerAdoptionByIdAsync(consumerAdoption.Id);

                ValidateStorageConsumerAdoption(maybeConsumerAdoption, consumerAdoption.Id);

                consumerAdoption = await this.securityAuditBroker
                    .EnsureAddAuditValuesRemainsUnchangedOnModifyAsync(
                        consumerAdoption,
                        maybeConsumerAdoption);

                ValidateAgainstStorageConsumerAdoptionOnModify(
                    inputConsumerAdoption: consumerAdoption,
                    storageConsumerAdoption: maybeConsumerAdoption);

                return await this.storageBroker.UpdateConsumerAdoptionAsync(consumerAdoption);
            });

        public ValueTask<ConsumerAdoption> RemoveConsumerAdoptionByIdAsync(Guid consumerAdoptionId) =>
            TryCatch(async () =>
            {
                ValidateConsumerAdoptionId(consumerAdoptionId);

                ConsumerAdoption maybeConsumerAdoption = await this.storageBroker
                    .SelectConsumerAdoptionByIdAsync(consumerAdoptionId);

                ValidateStorageConsumerAdoption(maybeConsumerAdoption, consumerAdoptionId);

                return await this.storageBroker.DeleteConsumerAdoptionAsync(maybeConsumerAdoption);
            });

        public ValueTask BulkAddOrModifyConsumerAdoptionsAsync(
            List<ConsumerAdoption> consumerAdoptions,
            int batchSize = 10000) =>
            TryCatch(async () =>
            {
                ValidateOnBulkAddOrModifyConsumerAdoptions(consumerAdoptions);
                await BulkAddOrModifyBatchAsync(consumerAdoptions, batchSize);
            });

        virtual internal async ValueTask BulkAddOrModifyBatchAsync(
            List<ConsumerAdoption> consumerAdoptions, int batchSize = 10000)
        {
            int totalRecords = consumerAdoptions.Count;
            var exceptions = new List<Exception>();

            for (int i = 0; i < totalRecords; i += batchSize)
            {
                try
                {
                    List<ConsumerAdoption> batch = consumerAdoptions.Skip(i).Take(batchSize).ToList();

                    var batchCompositeKeys = batch
                        .Select(consumerAdoption => new { consumerAdoption.DecisionId, consumerAdoption.ConsumerId })
                        .ToList();

                    IQueryable<ConsumerAdoption> allConsumerAdoptions =
                        await this.storageBroker.SelectAllConsumerAdoptionsAsync();

                    IQueryable<ConsumerAdoption> storageBatchConsumerAdoptions = allConsumerAdoptions
                        .Where(consumerAdoption => batchCompositeKeys.Any(
                            key => key.DecisionId == consumerAdoption.DecisionId &&
                                key.ConsumerId == consumerAdoption.ConsumerId));

                    var existingCompositeKeys = storageBatchConsumerAdoptions
                        .Select(consumerAdoption => new { consumerAdoption.DecisionId, consumerAdoption.ConsumerId })
                        .ToList();

                    List<ConsumerAdoption> newConsumerAdoptions = batch
                        .Where(consumerAdoption => !existingCompositeKeys.Any(
                            key => key.DecisionId == consumerAdoption.DecisionId &&
                                key.ConsumerId == consumerAdoption.ConsumerId))
                        .ToList();

                    List<ConsumerAdoption> existingConsumerAdoptions = batch
                        .Where(consumerAdoption => existingCompositeKeys.Any(
                            key => key.DecisionId == consumerAdoption.DecisionId &&
                                key.ConsumerId == consumerAdoption.ConsumerId))
                        .ToList();

                    try
                    {
                        if (newConsumerAdoptions.Count is not 0)
                        {
                            List<ConsumerAdoption> validatedNewConsumerAdoptions =
                                await ValidateConsumerAdoptionsAndAssignIdAndAuditOnAddAsync(newConsumerAdoptions);

                            await this.storageBroker.BulkInsertConsumerAdoptionsAsync(validatedNewConsumerAdoptions);
                        }
                    }
                    catch (Exception insertException)
                    {
                        exceptions.Add(insertException);
                        await this.loggingBroker.LogErrorAsync(insertException);
                    }

                    try
                    {
                        if (existingConsumerAdoptions.Count is not 0)
                        {
                            List<ConsumerAdoption> validatedExistingConsumerAdoptions =
                                await ValidateConsumerAdoptionsAndAssignAuditOnModifyAsync(
                                    existingConsumerAdoptions);

                            await this.storageBroker.BulkUpdateConsumerAdoptionsAsync(
                                validatedExistingConsumerAdoptions);
                        }
                    }
                    catch (Exception updateException)
                    {
                        exceptions.Add(updateException);
                        await this.loggingBroker.LogErrorAsync(updateException);
                    }
                }
                catch (Exception exception)
                {
                    exceptions.Add(exception);
                    await this.loggingBroker.LogErrorAsync(exception);
                }
            }

            if (exceptions.Any())
            {
                throw new AggregateException(
                    $"Unable to process consumerAdoptions in {exceptions.Count} of the batch(es)",
                    exceptions);
            }
        }
    }
}
