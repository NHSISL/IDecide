// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ISL.Providers.Captcha.Abstractions.Models;
using LondonDataServices.IDecide.Core.Brokers.Audits;
using LondonDataServices.IDecide.Core.Brokers.DateTimes;
using LondonDataServices.IDecide.Core.Brokers.Identifiers;
using LondonDataServices.IDecide.Core.Brokers.Loggings;
using LondonDataServices.IDecide.Core.Brokers.Securities;
using LondonDataServices.IDecide.Core.Models.Brokers.Securities;
using LondonDataServices.IDecide.Core.Models.Foundations.Consumers;
using LondonDataServices.IDecide.Core.Models.Foundations.Decisions;
using LondonDataServices.IDecide.Core.Models.Foundations.Notifications;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using LondonDataServices.IDecide.Core.Models.Orchestrations.Decisions;
using LondonDataServices.IDecide.Core.Models.Orchestrations.Decisions.Exceptions;
using LondonDataServices.IDecide.Core.Services.Foundations.Consumers;
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
        private readonly IAuditBroker auditBroker;
        private readonly IIdentifierBroker identifierBroker;
        private readonly IPatientService patientService;
        private readonly IDecisionService decisionService;
        private readonly INotificationService notificationService;
        private readonly IConsumerService consumerService;
        private readonly DecisionConfigurations decisionConfigurations;
        private readonly SecurityBrokerConfigurations securityBrokerConfigurations;

        public DecisionOrchestrationService(
            ILoggingBroker loggingBroker,
            IDateTimeBroker dateTimeBroker,
            ISecurityBroker securityBroker,
            IAuditBroker auditBroker,
            IIdentifierBroker identifierBroker,
            IPatientService patientService,
            IDecisionService decisionService,
            INotificationService notificationService,
            IConsumerService consumerService,
            DecisionConfigurations decisionConfigurations,
            SecurityBrokerConfigurations securityBrokerConfigurations)
        {
            this.loggingBroker = loggingBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.securityBroker = securityBroker;
            this.auditBroker = auditBroker;
            this.identifierBroker = identifierBroker;
            this.patientService = patientService;
            this.decisionService = decisionService;
            this.notificationService = notificationService;
            this.consumerService = consumerService;
            this.decisionConfigurations = decisionConfigurations;
            this.securityBrokerConfigurations = securityBrokerConfigurations;
        }

        public ValueTask VerifyAndRecordDecisionAsync(Decision decision) =>
            TryCatch(async () =>
            {
                ValidateVerifyAndRecordDecisionArguments(decision);
                string maybeNhsNumber = decision.Patient.NhsNumber;
                IQueryable<Patient> patients = await this.patientService.RetrieveAllPatientsAsync();
                Patient maybeMatchingPatient = patients.FirstOrDefault(patient => patient.NhsNumber == maybeNhsNumber);
                ValidatePatientExists(maybeMatchingPatient);
                decision.PatientId = maybeMatchingPatient.Id;
                Guid correlationId = await this.identifierBroker.GetIdentifierAsync();
                bool isAuthenticatedUserWithRole = await CheckIfIsAuthenticatedUserWithRequiredRoleAsync();
                string verifyingDecisionAuditMessage;

                if (isAuthenticatedUserWithRole)
                {
                    var currentUser = await this.securityBroker.GetCurrentUserAsync();
                    verifyingDecisionAuditMessage = $"User {currentUser.UserId} is verifying the decision for " +
                        $"patient Nhs Number: {maybeMatchingPatient.NhsNumber}, " +
                        $"with PatientId {maybeMatchingPatient.Id}";
                }
                else
                {
                    string ipAddress = await this.securityBroker.GetIpAddressAsync();

                    verifyingDecisionAuditMessage = $"Patient with IP address {ipAddress} is validating a code for " +
                        $"patient Nhs Number: {maybeMatchingPatient.NhsNumber}, " +
                        $"with PatientId {maybeMatchingPatient.Id}";
                }

                await this.auditBroker.LogInformationAsync(
                    auditType: "Decision",
                    title: "Verifying Decision",
                    message: verifyingDecisionAuditMessage,
                    fileName: null,
                    correlationId: correlationId.ToString());

                if (maybeMatchingPatient.ValidationCodeMatchedOn is null)
                {
                    await this.auditBroker.LogInformationAsync(
                    auditType: "Decision",
                    title: "Decision Submission Failed",

                    message: "There was no matched validation code found for this patient " +
                        $"Nhs Number: {maybeMatchingPatient.NhsNumber}, " +
                        $"with PatientId {maybeMatchingPatient.Id}",

                    fileName: null,
                    correlationId: correlationId.ToString());

                    throw new ValidationCodeNotMatchedException(
                        "The validation code for this patient has not been successfully matched");
                }

                DateTimeOffset now = await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync();
                DateTimeOffset validationCodeMatchedOn = maybeMatchingPatient.ValidationCodeMatchedOn.Value;

                DateTimeOffset validatedCodeValidUntil =
                    validationCodeMatchedOn.AddMinutes(this.decisionConfigurations.ValidatedCodeValidForMinutes);

                if (now > validatedCodeValidUntil)
                {
                    await this.auditBroker.LogInformationAsync(
                    auditType: "Decision",
                    title: "Decision Submission Failed",

                    message: $"There was a matched validation code found but the matching " +
                        $"period has now expired for patientId {maybeMatchingPatient.Id.ToString()}.",

                    fileName: null,
                    correlationId: correlationId.ToString());

                    throw new ValidationCodeMatchExpiredException(
                        "The validation code for this patient is no longer active. " +
                        "Please complete validation process again.");
                }

                Patient updatedPatient = maybeMatchingPatient;
                updatedPatient.NotificationPreference = decision.Patient.NotificationPreference;
                Patient modifiedPatient = await this.patientService.ModifyPatientAsync(updatedPatient);
                Decision addedDecision = await this.decisionService.AddDecisionAsync(decision);

                NotificationInfo notificationInfo = new NotificationInfo
                {
                    Patient = modifiedPatient,
                    Decision = addedDecision
                };

                await this.notificationService.SendSubmissionSuccessNotificationAsync(notificationInfo);

                await this.auditBroker.LogInformationAsync(
                    auditType: "Decision",
                    title: "Decision Submitted",

                    message: $"The patient's decision has been successfully submitted for " +
                        $"decisionId {addedDecision.Id}, " +
                        $"patient Nhs Number: {maybeMatchingPatient.NhsNumber}, with " +
                        $"PatientId {maybeMatchingPatient.Id}",

                    fileName: null,
                    correlationId: correlationId.ToString());
            });

        public async ValueTask<List<Decision>> RetrieveAllPendingAdoptionDecisionsForConsumer(
            DateTimeOffset changesSinceDate,
            string decisionType)
        {
            throw new NotImplementedException();
        }

        virtual internal async ValueTask<bool> CheckIfIsAuthenticatedUserWithRequiredRoleAsync()
        {
            var currentUserIsAuthenticated = await this.securityBroker.IsCurrentUserAuthenticatedAsync();

            if (currentUserIsAuthenticated)
            {
                bool userIsInWorkflowRole = false;

                foreach (string role in this.decisionConfigurations.DecisionWorkflowRoles)
                {
                    if (await this.securityBroker.IsInRoleAsync(role))
                    {
                        userIsInWorkflowRole = true;
                        break;
                    }
                }

                if (userIsInWorkflowRole is false)
                {
                    throw new UnauthorizedDecisionOrchestrationServiceException(
                        message: "The current user is not authorized to perform this operation.");
                }
                else
                {
                    return true;
                }
            }
            else
            {
                CaptchaResult captchaResult = await this.securityBroker.ValidateCaptchaAsync();

                if (captchaResult.Success is false)
                {
                    throw new InvalidCaptchaDecisionOrchestrationServiceException(
                        message: "The provided captcha token is invalid.");
                }
                else if (captchaResult.Success is true
                    && captchaResult.Score < securityBrokerConfigurations.ReCaptchaScoreThreshold)
                {
                    throw new ReCaptchaLowConfidenceException(
                        message: "The captcha score is below the configured threshold.");
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
