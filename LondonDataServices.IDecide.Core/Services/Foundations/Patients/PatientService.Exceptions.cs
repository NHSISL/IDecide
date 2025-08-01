// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using EFxceptions.Models.Exceptions;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xeptions;

namespace LondonDataServices.IDecide.Core.Services.Foundations.Patients
{
    public partial class PatientService
    {
        private delegate ValueTask<Patient> ReturningPatientFunction();
        private delegate ValueTask<IQueryable<Patient>> ReturningPatientsFunction();

        private async ValueTask<Patient> TryCatch(ReturningPatientFunction returningPatientFunction)
        {
            try
            {
                return await returningPatientFunction();
            }
            catch (NullPatientException nullPatientException)
            {
                throw await CreateAndLogValidationExceptionAsync(nullPatientException);
            }
            catch (InvalidPatientException invalidPatientException)
            {
                throw await CreateAndLogValidationExceptionAsync(invalidPatientException);
            }
            catch (SqlException sqlException)
            {
                var failedPatientStorageException =
                    new FailedPatientStorageException(
                        message: "Failed patient storage error occurred, contact support.",
                        innerException: sqlException);

                throw await CreateAndLogCriticalDependencyExceptionAsync(failedPatientStorageException);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsPatientException =
                    new AlreadyExistsPatientException(
                        message: "Patient with the same Id already exists.",
                        innerException: duplicateKeyException,
                        data: duplicateKeyException.Data);

                throw await CreateAndLogDependencyValidationExceptionAsync(alreadyExistsPatientException);
            }
            catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
            {
                var lockedPatientException =
                    new LockedPatientException(
                        message: "Locked patient record exception, please fix errors and try again.",
                        innerException: dbUpdateConcurrencyException);

                throw await CreateAndLogDependencyValidationExceptionAsync(lockedPatientException);
            }
            catch (DbUpdateException databaseUpdateException)
            {
                var failedPatientStorageException =
                    new FailedPatientStorageException(
                        message: "Failed patient storage error occurred, contact support.",
                        innerException: databaseUpdateException);

                throw await CreateAndLogDependencyExceptionAsync(failedPatientStorageException);
            }
            catch (Exception exception)
            {
                var failedPatientServiceException =
                    new FailedPatientServiceException(
                        message: "Failed patient service occurred, please contact support",
                        innerException: exception);

                throw await CreateAndLogServiceExceptionAsync(failedPatientServiceException);
            }
        }

        private async ValueTask<IQueryable<Patient>> TryCatch(
            ReturningPatientsFunction returningPatientsFunction)
        {
            try
            {
                return await returningPatientsFunction();
            }
            catch (SqlException sqlException)
            {
                var failedPatientStorageException =
                    new FailedPatientStorageException(
                        message: "Failed patient storage error occurred, contact support.",
                        innerException: sqlException);

                throw await CreateAndLogCriticalDependencyExceptionAsync(failedPatientStorageException);
            }
            catch (Exception exception)
            {
                var failedPatientServiceException =
                    new FailedPatientServiceException(
                        message: "Failed patient service occurred, please contact support",
                        innerException: exception);

                throw await CreateAndLogServiceExceptionAsync(failedPatientServiceException);
            }
        }

        private async ValueTask<PatientValidationException> CreateAndLogValidationExceptionAsync(
            Xeption exception)
        {
            var patientValidationException =
                new PatientValidationException(
                    message: "Patient validation error occurred, please fix errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(patientValidationException);

            return patientValidationException;
        }

        private async ValueTask<PatientDependencyException> CreateAndLogCriticalDependencyExceptionAsync(
            Xeption exception)
        {
            var patientDependencyException =
                new PatientDependencyException(
                    message: "Patient dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogCriticalAsync(patientDependencyException);

            return patientDependencyException;
        }

        private async ValueTask<PatientDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(Xeption exception)
        {
            var patientDependencyValidationException =
                new PatientDependencyValidationException(
                    message: "Patient dependency validation occurred, please fix errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(patientDependencyValidationException);

            return patientDependencyValidationException;
        }

        private async ValueTask<PatientDependencyException> CreateAndLogDependencyExceptionAsync(
            Xeption exception)
        {
            var patientDependencyException =
                new PatientDependencyException(
                    message: "Patient dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(patientDependencyException);

            return patientDependencyException;
        }

        private async ValueTask<PatientServiceException> CreateAndLogServiceExceptionAsync(
            Xeption exception)
        {
            var patientServiceException =
                new PatientServiceException(
                    message: "Patient service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(patientServiceException);

            return patientServiceException;
        }
    }
}
