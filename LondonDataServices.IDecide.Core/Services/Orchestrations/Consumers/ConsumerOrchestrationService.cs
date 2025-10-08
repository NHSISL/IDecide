// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Brokers.DateTimes;
using LondonDataServices.IDecide.Core.Brokers.Loggings;
using LondonDataServices.IDecide.Core.Brokers.Securities;
using LondonDataServices.IDecide.Core.Models.Foundations.ConsumerAdoptions;
using LondonDataServices.IDecide.Core.Models.Foundations.Decisions;
using LondonDataServices.IDecide.Core.Models.Foundations.Notifications;
using LondonDataServices.IDecide.Core.Services.Foundations.ConsumerAdoptions;
using LondonDataServices.IDecide.Core.Services.Foundations.Consumers;
using LondonDataServices.IDecide.Core.Services.Foundations.Notifications;
using LondonDataServices.IDecide.Core.Services.Foundations.Patients;

namespace LondonDataServices.IDecide.Core.Services.Orchestrations.Consumers
{
    public class ConsumerOrchestrationService : IConsumerOrchestrationService
    {
        private readonly ILoggingBroker loggingBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ISecurityBroker securityBroker;
        private readonly IConsumerService consumerService;
        private readonly IConsumerAdoptionService consumerAdoptionService;
        private readonly INotificationService notificationService;
        private readonly IPatientService patientService;

        public ConsumerOrchestrationService(
            ILoggingBroker loggingBroker,
            IDateTimeBroker dateTimeBroker,
            ISecurityBroker securityBroker,
            IConsumerService consumerService,
            IConsumerAdoptionService consumerAdoptionService,
            INotificationService notificationService,
            IPatientService patientService)
        {
            this.loggingBroker = loggingBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.securityBroker = securityBroker;
            this.consumerService = consumerService;
            this.consumerAdoptionService = consumerAdoptionService;
            this.notificationService = notificationService;
            this.patientService = patientService;
        }

        public async ValueTask AdoptPatientDecisions(List<Decision> decisions)
        {
            var user = await this.securityBroker.GetCurrentUserAsync();
            var consumerId = Guid.Parse(user.UserId);
            var consumer = await this.consumerService.RetrieveConsumerByIdAsync(consumerId);

            var consumerAdoptions = new List<ConsumerAdoption>();

            foreach (var decision in decisions)
            {
                var consumerAdoption = new ConsumerAdoption
                {
                    ConsumerId = consumer.Id,
                    DecisionId = decision.Id,
                    AdoptionDate = await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync()
                };

                consumerAdoptions.Add(consumerAdoption);
            }

            await this.consumerAdoptionService.BulkAddOrModifyConsumerAdoptionsAsync(consumerAdoptions);

            foreach (var decision in decisions)
            {
                var patient =
                    decision.Patient ?? await this.patientService.RetrievePatientByIdAsync(decision.PatientId);

                var notificationInfo = new NotificationInfo
                {
                    Decision = decision,
                    Patient = patient
                };

                await this.notificationService.SendSubscriberUsageNotificationAsync(notificationInfo);
            }
        }
    }
}
