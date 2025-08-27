// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Brokers.DateTimes;
using LondonDataServices.IDecide.Core.Brokers.Loggings;
using LondonDataServices.IDecide.Core.Brokers.Securities;
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
        private readonly ISecurityBroker securityBroker;
        private readonly IPatientService patientService;
        private readonly IDecisionService decisionService;
        private readonly INotificationService notificationService;
        private readonly DecisionConfigurations decisionConfiguration;

        public DecisionOrchestrationService(
            ILoggingBroker loggingBroker,
            IDateTimeBroker dateTimeBroker,
            ISecurityBroker securityBroker,
            IPatientService patientService,
            IDecisionService decisionService,
            INotificationService notificationService,
            DecisionConfigurations decisionConfigurations)
        {
            this.loggingBroker = loggingBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.securityBroker = securityBroker;
            this.patientService = patientService;
            this.decisionService = decisionService;
            this.notificationService = notificationService;
            this.decisionConfiguration = decisionConfigurations;
        }

        public ValueTask VerifyAndRecordDecisionAsync(Decision decision) =>
            TryCatch(async () =>
            {
                ValidateVerifyAndRecordDecisionArguments(decision);
                string maybeNhsNumber = decision.PatientNhsNumber;
                IQueryable<Patient> patients = await this.patientService.RetrieveAllPatientsAsync();
                Patient maybeMatchingPatient = patients.FirstOrDefault(patient => patient.NhsNumber == maybeNhsNumber);
                ValidatePatientExists(maybeMatchingPatient);

                bool userIsInWorkflowRole = false;

                foreach (string role in this.decisionConfiguration.DecisionWorkflowRoles)
                {
                    if (await this.securityBroker.IsInRoleAsync(role))
                    {
                        userIsInWorkflowRole = true;
                        break;
                    }
                }

                if (userIsInWorkflowRole)
                {
                    if (maybeMatchingPatient.ValidationCode != decision.Patient.ValidationCode)
                    {
                        throw new IncorrectValidationCodeException("The validation code provided is incorrect.");
                    }
                }
                else
                {
                    if (maybeMatchingPatient.RetryCount > this.decisionConfiguration.MaxRetryCount)
                    {
                        throw new ExceededMaxRetryCountException(
                            $"The maximum retry count of {this.decisionConfiguration.MaxRetryCount} exceeded.");
                    }

                    if (maybeMatchingPatient.ValidationCode != decision.Patient.ValidationCode)
                    {
                        Patient patientToUpdate = maybeMatchingPatient;
                        patientToUpdate.RetryCount += 1;
                        await this.patientService.ModifyPatientAsync(patientToUpdate);

                        throw new IncorrectValidationCodeException("The validation code provided is incorrect.");
                    }

                    DateTimeOffset currentDateTime = await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync();

                    if (maybeMatchingPatient.ValidationCodeExpiresOn < currentDateTime)
                    {
                        throw new ExpiredValidationCodeException("The validation code has expired.");
                    }
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
