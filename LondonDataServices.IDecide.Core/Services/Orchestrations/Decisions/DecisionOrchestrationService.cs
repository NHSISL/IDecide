// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Brokers.DateTimes;
using LondonDataServices.IDecide.Core.Brokers.Loggings;
using LondonDataServices.IDecide.Core.Models.Foundations.Decisions;
using LondonDataServices.IDecide.Core.Models.Foundations.Notifications;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using LondonDataServices.IDecide.Core.Models.Orchestrations.Decisions;
using LondonDataServices.IDecide.Core.Models.Orchestrations.Decisions.Exceptions;
using LondonDataServices.IDecide.Core.Services.Foundations.Decisions;
using LondonDataServices.IDecide.Core.Services.Foundations.Notifications;
using LondonDataServices.IDecide.Core.Services.Foundations.Patients;

namespace LondonDataServices.IDecide.Core.Services.Orchestrations.Decisions
{
    public partial class DecisionOrchestrationService : IDecisionOrchestrationService
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

        public ValueTask VerifyAndRecordDecisionAsync(Decision decision) =>
            TryCatch(async () =>
            {
                ValidateDecisionIsNotNull(decision);
                ValidateDecision(decision);
                string maybeNhsNumber = decision.PatientNhsNumber;
                IQueryable<Patient> patients = await this.patientService.RetrieveAllPatientsAsync();
                Patient maybeMatchingPatient = patients.FirstOrDefault(patient => patient.NhsNumber == maybeNhsNumber);
                ValidatePatientExists(maybeMatchingPatient);

                if (maybeMatchingPatient.RetryCount > this.decisionOrchestrationConfiguration.MaxRetryCount)
                {
                    throw new ExceededMaxRetryCountException(
                        $"The maximum retry count of {this.decisionOrchestrationConfiguration.MaxRetryCount} exceeded.");
                }

                DateTimeOffset currentDateTime = await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync();

                if (maybeMatchingPatient.ValidationCodeExpiresOn < currentDateTime)
                {
                    Patient patientToUpdate = maybeMatchingPatient;
                    patientToUpdate.RetryCount += 1;
                    await this.patientService.ModifyPatientAsync(patientToUpdate);

                    throw new ExpiredValidationCodeException("The validation code has expired.");
                }

                if (maybeMatchingPatient.ValidationCode != decision.Patient.ValidationCode)
                {
                    Patient patientToUpdate = maybeMatchingPatient;
                    patientToUpdate.RetryCount += 1;
                    await this.patientService.ModifyPatientAsync(patientToUpdate);

                    throw new IncorrectValidationCodeException("The validation code provided is incorrect.");
                }

                Decision addedDecision = await this.decisionService.AddDecisionAsync(decision);

                NotificationInfo notificationInfo = new NotificationInfo
                {
                    Patient = maybeMatchingPatient,
                    Decision = addedDecision
                };

                await this.notificationService.SendSubmissionSuccessNotificationAsync(notificationInfo);
            });
    }
}
