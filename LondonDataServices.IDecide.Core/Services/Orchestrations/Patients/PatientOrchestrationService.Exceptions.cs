// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Foundations.Notifications.Exceptions;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients.Exceptions;
using LondonDataServices.IDecide.Core.Models.Foundations.Pds.Exceptions;
using LondonDataServices.IDecide.Core.Models.Orchestrations.Patients.Exceptions;
using Xeptions;
using NullPatientOrchestrationException = LondonDataServices.IDecide.Core.Models.Orchestrations.Patients.Exceptions.NullPatientOrchestrationException;

namespace LondonDataServices.IDecide.Core.Services.Orchestrations.Patients
{
    public partial class PatientOrchestrationService
    {
        private delegate ValueTask<Patient> ReturningPatientFunction();
        private delegate ValueTask ReturningNothingFunction();

        private async ValueTask<Patient> TryCatch(
            ReturningPatientFunction returningPatientFunction)
        {
            try
            {
                return await returningPatientFunction();
            }
            catch (NullPatientLookupException nullPatientLookupException)
            {
                throw await CreateAndLogValidationExceptionAsync(nullPatientLookupException);
            }
            catch (NoExactPatientFoundException noExactPatientFoundException)
            {
                throw await CreateAndLogValidationExceptionAsync(noExactPatientFoundException);
            }
            catch (InvalidPatientOrchestrationArgumentException invalidPatientOrchestrationArgumentException)
            {
                throw await CreateAndLogValidationExceptionAsync(invalidPatientOrchestrationArgumentException);
            }
            catch (NullPatientOrchestrationException nullPatientOrchestrationException)
            {
                throw await CreateAndLogValidationExceptionAsync(nullPatientOrchestrationException);
            }
            catch (PdsValidationException pdsValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(pdsValidationException);
            }
            catch (PdsDependencyValidationException pdsDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(pdsDependencyValidationException);
            }
            catch (PdsServiceException pdsServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(pdsServiceException);
            }
            catch (PdsDependencyException pdsDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(pdsDependencyException);
            }
            catch (ExternalOptOutPatientOrchestrationException externalOptOutPatientOrchestrationException)
            {
                throw await CreateAndLogServiceExceptionAsync(externalOptOutPatientOrchestrationException);

                //var failedPatientOrchestrationServiceException =
                //    new FailedPatientOrchestrationServiceException(
                //        message: "Failed patient orchestration service error occurred, contact support.",
                //        innerException: externalOptOutPatientOrchestrationException);

                //throw await CreateAndLogServiceExceptionAsync(failedPatientOrchestrationServiceException);
            }
            catch (Exception exception)
            {
                var failedPatientOrchestrationServiceException =
                    new FailedPatientOrchestrationServiceException(
                        message: "Failed patient orchestration service error occurred, contact support.",
                        innerException: exception);

                throw await CreateAndLogServiceExceptionAsync(failedPatientOrchestrationServiceException);
            }
        }

        private async ValueTask TryCatch(
            ReturningNothingFunction returningNothingFunction)
        {
            try
            {
                await returningNothingFunction();
            }
            catch (InvalidCaptchaPatientOrchestrationServiceException invalidCaptchaException)
            {
                throw await CreateAndLogValidationExceptionAsync(invalidCaptchaException);
            }
            catch (UnauthorizedPatientOrchestrationServiceException unauthorizedPatientOrchestrationServiceException)
            {
                throw await CreateAndLogValidationExceptionAsync(unauthorizedPatientOrchestrationServiceException);
            }
            catch (MaxRetryAttemptsExceededException maxRetryAttemptsExceededException)
            {
                throw await CreateAndLogValidationExceptionAsync(maxRetryAttemptsExceededException);
            }
            catch (IncorrectValidationCodeException incorrectValidationCodeException)
            {
                throw await CreateAndLogValidationExceptionAsync(incorrectValidationCodeException);
            }
            catch (ExceededMaxRetryCountException exceededMaxRetryCountException)
            {
                throw await CreateAndLogValidationExceptionAsync(exceededMaxRetryCountException);
            }
            catch (RenewedValidationCodeException renewedValidationCodeException)
            {
                throw await CreateAndLogValidationExceptionAsync(renewedValidationCodeException);
            }
            catch (InvalidPatientOrchestrationArgumentException invalidPatientOrchestrationArgumentException)
            {
                throw await CreateAndLogValidationExceptionAsync(invalidPatientOrchestrationArgumentException);
            }
            catch (PdsValidationException pdsValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(pdsValidationException);
            }
            catch (PdsDependencyValidationException pdsDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(pdsDependencyValidationException);
            }
            catch (PdsServiceException pdsServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(pdsServiceException);
            }
            catch (PdsDependencyException pdsDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(pdsDependencyException);
            }
            catch (PatientValidationException patientValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(patientValidationException);
            }
            catch (PatientDependencyValidationException patientDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(patientDependencyValidationException);
            }
            catch (PatientServiceException patientServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(patientServiceException);
            }
            catch (PatientDependencyException patientDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(patientDependencyException);
            }
            catch (NotificationValidationException notificationValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(notificationValidationException);
            }
            catch (NotificationDependencyValidationException notificationDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(notificationDependencyValidationException);
            }
            catch (NotificationServiceException notificationServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(notificationServiceException);
            }
            catch (NotificationDependencyException notificationDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(notificationDependencyException);
            }
            catch (Exception exception)
            {
                var failedPatientOrchestrationServiceException =
                    new FailedPatientOrchestrationServiceException(
                        message: "Failed patient orchestration service error occurred, contact support.",
                        innerException: exception);

                throw await CreateAndLogServiceExceptionAsync(failedPatientOrchestrationServiceException);
            }
        }

        private async ValueTask<PatientOrchestrationValidationException>
            CreateAndLogValidationExceptionAsync(Xeption exception)
        {
            var patientOrchestrationValidationException =
                new PatientOrchestrationValidationException(
                    message: "Patient orchestration validation error occurred, " +
                        "please fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(patientOrchestrationValidationException);

            return patientOrchestrationValidationException;
        }

        private async ValueTask<PatientOrchestrationDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(Xeption exception)
        {
            var patientOrchestrationDependencyValidationException =
                new PatientOrchestrationDependencyValidationException(
                    message: "Patient orchestration dependency validation error occurred, " +
                        "please fix the errors and try again.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(patientOrchestrationDependencyValidationException);

            return patientOrchestrationDependencyValidationException;
        }

        private async ValueTask<PatientOrchestrationDependencyException>
            CreateAndLogDependencyExceptionAsync(Xeption exception)
        {
            var patientOrchestrationDependencyException =
                new PatientOrchestrationDependencyException(
                    message: "Patient orchestration dependency error occurred, " +
                        "please fix the errors and try again.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(patientOrchestrationDependencyException);

            return patientOrchestrationDependencyException;
        }

        private async ValueTask<PatientOrchestrationServiceException> CreateAndLogServiceExceptionAsync(
           Xeption exception)
        {
            var patientOrchestrationServiceException = new PatientOrchestrationServiceException(
                message: "Patient orchestration service error occurred, contact support.",
                innerException: exception);

            await this.loggingBroker.LogErrorAsync(patientOrchestrationServiceException);

            return patientOrchestrationServiceException;
        }
    }
}
