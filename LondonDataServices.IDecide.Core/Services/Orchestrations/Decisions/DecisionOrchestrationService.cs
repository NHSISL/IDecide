// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Brokers.DateTimes;
using LondonDataServices.IDecide.Core.Brokers.Loggings;
using LondonDataServices.IDecide.Core.Models.Foundations.Decisions;
using LondonDataServices.IDecide.Core.Models.Orchestrations.Decisions;
using LondonDataServices.IDecide.Core.Services.Foundations.Decisions;
using LondonDataServices.IDecide.Core.Services.Foundations.Notifications;
using LondonDataServices.IDecide.Core.Services.Foundations.Patients;

namespace LondonDataServices.IDecide.Core.Services.Orchestrations.Decisions
{
    public class DecisionOrchestrationService : IDecisionOrchestrationService
    {
        private readonly ILoggingBroker loggingBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly IPatientService patientService;
        private readonly IDecisionService decisionService;
        private readonly INotificationService notificationService;
        private readonly DecisionOrchestrationConfigurations decisionOrchestrationConfiguration;

        public DecisionOrchestrationService(
            ILoggingBroker loggingBroker,
            IDateTimeBroker dateTimeBroker,
            IPatientService patientService,
            IDecisionService decisionService,
            INotificationService notificationService,
            DecisionOrchestrationConfigurations decisionOrchestrationConfigurations)
        {
            this.loggingBroker = loggingBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.patientService = patientService;
            this.decisionService = decisionService;
            this.notificationService = notificationService;
            this.decisionOrchestrationConfiguration = decisionOrchestrationConfigurations;
        }

        public ValueTask VerifyAndRecordDecisionAsync(Decision decision)
        {
            throw new System.NotImplementedException();
        }
    }
}
