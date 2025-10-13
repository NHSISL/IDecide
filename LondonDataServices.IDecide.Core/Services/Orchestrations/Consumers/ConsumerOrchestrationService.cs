// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Brokers.DateTimes;
using LondonDataServices.IDecide.Core.Brokers.Identifiers;
using LondonDataServices.IDecide.Core.Brokers.Loggings;
using LondonDataServices.IDecide.Core.Brokers.Securities;
using LondonDataServices.IDecide.Core.Models.Foundations.ConsumerAdoptions;
using LondonDataServices.IDecide.Core.Models.Foundations.Consumers;
using LondonDataServices.IDecide.Core.Models.Foundations.Decisions;
using LondonDataServices.IDecide.Core.Models.Foundations.Notifications;
using LondonDataServices.IDecide.Core.Models.Orchestrations.Consumers.Exceptions;
using LondonDataServices.IDecide.Core.Models.Securities;
using LondonDataServices.IDecide.Core.Services.Foundations.ConsumerAdoptions;
using LondonDataServices.IDecide.Core.Services.Foundations.Consumers;
using LondonDataServices.IDecide.Core.Services.Foundations.Notifications;
using LondonDataServices.IDecide.Core.Services.Foundations.Patients;

namespace LondonDataServices.IDecide.Core.Services.Orchestrations.Consumers
{
    public partial class ConsumerOrchestrationService : IConsumerOrchestrationService
    {
        private readonly ILoggingBroker loggingBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ISecurityBroker securityBroker;
        private readonly IIdentifierBroker identifierBroker;
        private readonly IConsumerService consumerService;
        private readonly IConsumerAdoptionService consumerAdoptionService;
        private readonly IPatientService patientService;
        private readonly INotificationService notificationService;
        public ConsumerOrchestrationService(
            ILoggingBroker loggingBroker,
            IDateTimeBroker dateTimeBroker,
            ISecurityBroker securityBroker,
            IIdentifierBroker identifierBroker,
            IConsumerService consumerService,
            IConsumerAdoptionService consumerAdoptionService,
            IPatientService patientService,
            INotificationService notificationService)
        {
            this.loggingBroker = loggingBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.securityBroker = securityBroker;
            this.identifierBroker = identifierBroker;
            this.consumerService = consumerService;
            this.consumerAdoptionService = consumerAdoptionService;
            this.patientService = patientService;
            this.notificationService = notificationService;
        }

        public ValueTask AdoptPatientDecisions(List<Decision> decisions) =>
            TryCatch(async () =>
            {
                ValidateDecisions(decisions);
                var user = await this.securityBroker.GetCurrentUserAsync();
                IQueryable<Consumer> consumers = await this.consumerService.RetrieveAllConsumersAsync();
                Consumer consumer = consumers.FirstOrDefault(c => c.EntraId == user.UserId);
                var consumerAdoptions = new List<ConsumerAdoption>();
                DateTimeOffset adoptionDate = await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync();

                foreach (var decision in decisions)
                {
                    try
                    {
                        var consumerAdoption = new ConsumerAdoption
                        {
                            Id = await this.identifierBroker.GetIdentifierAsync(),
                            ConsumerId = consumer.Id,
                            DecisionId = decision.Id,
                            AdoptionDate = adoptionDate
                        };

                        consumerAdoptions.Add(consumerAdoption);
                    }
                    catch (Exception ex)
                    {
                        await this.loggingBroker.LogErrorAsync(ex);
                    }
                }

                await this.consumerAdoptionService.BulkAddOrModifyConsumerAdoptionsAsync(consumerAdoptions);

                foreach (var decision in decisions)
                {
                    try
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
                    catch (Exception ex)
                    {
                        await this.loggingBroker.LogErrorAsync(ex);
                    }
                }
            });

        public async ValueTask RecordConsumerAdoption(List<Guid> decisionIds)
        {
            bool currentUserIsAuthenticated = await this.securityBroker.IsCurrentUserAuthenticatedAsync();

            if (!currentUserIsAuthenticated)
            {
                throw new UnauthorizedConsumerOrchestrationServiceException(
                    message: "The current user is not authorized to perform this operation.");
            }

            User currentUser = await this.securityBroker.GetCurrentUserAsync();
            IQueryable<Consumer> consumers = await this.consumerService.RetrieveAllConsumersAsync();
            Consumer maybeConsumer = consumers.FirstOrDefault(consumer => consumer.EntraId == currentUser.UserId);

            if (maybeConsumer is null)
            {
                throw new UnauthorizedConsumerOrchestrationServiceException(
                    message: "The current user is not authorized to perform this operation.");
            }

            Guid consumerId = maybeConsumer.Id;
            List<ConsumerAdoption> consumerAdoptions = new List<ConsumerAdoption>();
            DateTimeOffset adoptionDate = await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync();

            foreach (var decisionId in decisionIds)
            {
                try
                {
                    var consumerAdoption = new ConsumerAdoption
                    {
                        Id = await this.identifierBroker.GetIdentifierAsync(),
                        ConsumerId = consumerId,
                        DecisionId = decisionId,
                        AdoptionDate = adoptionDate
                    };

                    consumerAdoptions.Add(consumerAdoption);
                }
                catch (Exception ex)
                {
                    await this.loggingBroker.LogErrorAsync(ex);
                }
            }

            await this.consumerAdoptionService.BulkAddOrModifyConsumerAdoptionsAsync(consumerAdoptions);
        }
    }
}
