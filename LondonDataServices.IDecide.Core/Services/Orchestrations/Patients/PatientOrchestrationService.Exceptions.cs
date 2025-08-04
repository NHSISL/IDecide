// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Foundations.Pds;
using LondonDataServices.IDecide.Core.Models.Foundations.Pds.Exceptions;
using LondonDataServices.IDecide.Core.Models.Orchestrations.Patients.Exceptions;
using Xeptions;

namespace LondonDataServices.IDecide.Core.Services.Orchestrations.Patients
{
    public partial class PatientOrchestrationService
    {
        private delegate ValueTask<Patient> ReturningPatientFunction();

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
                throw await CreateAndLogValidationExceptionAsync(
                    noExactPatientFoundException);
            }
            catch (PdsValidationException pdsValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    pdsValidationException);
            }
            catch (PdsDependencyValidationException pdsDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    pdsDependencyValidationException);
            }
            catch (PdsServiceException pdsServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    pdsServiceException);
            }
            catch (PdsDependencyException pdsDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    pdsDependencyException);
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
                        "fix the errors and try again.",
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
                        "fix the errors and try again.",
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
                        "fix the errors and try again.",
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
