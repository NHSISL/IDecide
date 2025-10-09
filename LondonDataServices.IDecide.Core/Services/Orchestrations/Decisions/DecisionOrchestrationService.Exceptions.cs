// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Foundations.Consumers.Exceptions;
using LondonDataServices.IDecide.Core.Models.Foundations.Decisions;
using LondonDataServices.IDecide.Core.Models.Foundations.Decisions.Exceptions;
using LondonDataServices.IDecide.Core.Models.Foundations.Notifications.Exceptions;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients.Exceptions;
using LondonDataServices.IDecide.Core.Models.Orchestrations.Decisions.Exceptions;
using Xeptions;

using NotFoundPatientException =
    LondonDataServices.IDecide.Core.Models.Orchestrations.Decisions.Exceptions.NotFoundPatientException;

using NullDecisionException =
    LondonDataServices.IDecide.Core.Models.Orchestrations.Decisions.Exceptions.NullDecisionException;

namespace LondonDataServices.IDecide.Core.Services.Orchestrations.Decisions
{
    public partial class DecisionOrchestrationService
    {
        private delegate ValueTask ReturningNothingFunction();
        private delegate ValueTask<List<Decision>> ReturningDecisionsFunction();

        private async ValueTask TryCatch(ReturningNothingFunction returningNothingFunction)
        {
            try
            {
                await returningNothingFunction();
            }
            catch (NullDecisionException nullDecisionException)
            {
                throw await CreateAndLogValidationExceptionAsync(nullDecisionException);
            }
            catch (InvalidDecisionOrchestrationArgumentException invalidDecisionOrchestrationArgumentException)
            {
                throw await CreateAndLogValidationExceptionAsync(invalidDecisionOrchestrationArgumentException);
            }
            catch (NotFoundPatientException notFoundPatientException)
            {
                throw await CreateAndLogValidationExceptionAsync(notFoundPatientException);
            }
            catch (ValidationCodeNotMatchedException validationCodeNotMatchedException)
            {
                throw await CreateAndLogValidationExceptionAsync(validationCodeNotMatchedException);
            }
            catch (ValidationCodeMatchExpiredException validationCodeMatchExpiredException)
            {
                throw await CreateAndLogValidationExceptionAsync(validationCodeMatchExpiredException);
            }
            catch (PatientValidationException patientValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    patientValidationException);
            }
            catch (PatientDependencyValidationException patientDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    patientDependencyValidationException);
            }
            catch (PatientServiceException patientServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    patientServiceException);
            }
            catch (PatientDependencyException patientDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    patientDependencyException);
            }
            catch (DecisionValidationException decisionValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    decisionValidationException);
            }
            catch (DecisionDependencyValidationException decisionDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    decisionDependencyValidationException);
            }
            catch (DecisionServiceException decisionServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    decisionServiceException);
            }
            catch (DecisionDependencyException decisionDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    decisionDependencyException);
            }
            catch (NotificationValidationException notificationValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    notificationValidationException);
            }
            catch (NotificationDependencyValidationException notificationDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    notificationDependencyValidationException);
            }
            catch (NotificationServiceException notificationServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    notificationServiceException);
            }
            catch (NotificationDependencyException notificationDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    notificationDependencyException);
            }
            catch (Exception exception)
            {
                var failedDecisionOrchestrationServiceException =
                    new FailedDecisionOrchestrationServiceException(
                        message: "Failed decision orchestration service error occurred, contact support.",
                        innerException: exception);

                throw await CreateAndLogServiceExceptionAsync(failedDecisionOrchestrationServiceException);
            }
        }

        private async ValueTask<List<Decision>> TryCatch(ReturningDecisionsFunction returningDecisionsFunction)
        {
            try
            {
                return await returningDecisionsFunction();
            }
            catch (ConsumerValidationException consumerValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    consumerValidationException);
            }
            catch (ConsumerDependencyValidationException consumerDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    consumerDependencyValidationException);
            }
            catch (DecisionValidationException decisionValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    decisionValidationException);
            }
            catch (DecisionDependencyValidationException decisionDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    decisionDependencyValidationException);
            }
            catch (Exception exception)
            {
                var failedDecisionOrchestrationServiceException =
                    new FailedDecisionOrchestrationServiceException(
                        message: "Failed decision orchestration service error occurred, contact support.",
                        innerException: exception);

                throw await CreateAndLogServiceExceptionAsync(failedDecisionOrchestrationServiceException);
            }
        }

        private async ValueTask<DecisionOrchestrationValidationException>
            CreateAndLogValidationExceptionAsync(Xeption exception)
        {
            var decisionOrchestrationValidationException =
                new DecisionOrchestrationValidationException(
                    message: "Decision orchestration validation error occurred, " +
                        "please fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(decisionOrchestrationValidationException);

            return decisionOrchestrationValidationException;
        }

        private async ValueTask<DecisionOrchestrationDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(Xeption exception)
        {
            var decisionOrchestrationDependencyValidationException =
                new DecisionOrchestrationDependencyValidationException(
                    message: "Decision orchestration dependency validation error occurred, " +
                        "please fix the errors and try again.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(decisionOrchestrationDependencyValidationException);

            return decisionOrchestrationDependencyValidationException;
        }

        private async ValueTask<DecisionOrchestrationDependencyException>
            CreateAndLogDependencyExceptionAsync(Xeption exception)
        {
            var decisionOrchestrationDependencyException =
                new DecisionOrchestrationDependencyException(
                    message: "Decision orchestration dependency error occurred, " +
                        "please fix the errors and try again.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(decisionOrchestrationDependencyException);

            return decisionOrchestrationDependencyException;
        }

        private async ValueTask<DecisionOrchestrationServiceException> CreateAndLogServiceExceptionAsync(
           Xeption exception)
        {
            var decisionOrchestrationServiceException = new DecisionOrchestrationServiceException(
                message: "Decision orchestration service error occurred, contact support.",
                innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(decisionOrchestrationServiceException);

            return decisionOrchestrationServiceException;
        }
    }
}
