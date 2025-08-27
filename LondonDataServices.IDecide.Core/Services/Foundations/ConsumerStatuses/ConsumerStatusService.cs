// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Brokers.DateTimes;
using LondonDataServices.IDecide.Core.Brokers.Loggings;
using LondonDataServices.IDecide.Core.Brokers.Securities;
using LondonDataServices.IDecide.Core.Brokers.Storages.Sql;
using LondonDataServices.IDecide.Core.Models.Foundations.ConsumerStatuses;

namespace LondonDataServices.IDecide.Core.Services.Foundations.ConsumerStatuses
{
    public partial class ConsumerStatusService : IConsumerStatusService
    {
        private readonly IStorageBroker storageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ISecurityBroker securityBroker;
        private readonly ISecurityAuditBroker securityAuditBroker;
        private readonly ILoggingBroker loggingBroker;

        public ConsumerStatusService(
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

        public ValueTask<ConsumerStatus> AddConsumerStatusAsync(ConsumerStatus consumerStatus) =>
            TryCatch(async () =>
            {
                consumerStatus = await this.securityAuditBroker.ApplyAddAuditValuesAsync(consumerStatus);
                await ValidateConsumerStatusOnAdd(consumerStatus);

                return await this.storageBroker.InsertConsumerStatusAsync(consumerStatus);
            });

        public ValueTask<ConsumerStatus> ModifyConsumerStatusAsync(ConsumerStatus consumerStatus)
        {
            throw new System.NotImplementedException();
        }
    }
}
