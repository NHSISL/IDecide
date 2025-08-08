// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
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
                throw await CreateAndLogValidationException(nullPatientException);
            }
            catch (InvalidPatientException invalidPatientException)
            {
                throw await CreateAndLogValidationException(invalidPatientException);
            }
            catch (SqlException sqlException)
            {
                var failedPatientStorageException =
                    new FailedPatientStorageException(
                        message: "Failed patient storage error occurred, contact support.",
                        innerException: sqlException);

                throw await CreateAndLogCriticalDependencyException(failedPatientStorageException);
            }
            catch (NotFoundPatientException notFoundPatientException)
            {
                throw await CreateAndLogValidationException(notFoundPatientException);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsPatientException =
                    new AlreadyExistsPatientException(
                        message: "Patient with the same Id already exists.",
                        innerException: duplicateKeyException);

                throw await CreateAndLogDependencyValidationException(alreadyExistsPatientException);
            }
            catch (ForeignKeyConstraintConflictException foreignKeyConstraintConflictException)
            {
                var invalidPatientReferenceException =
                    new InvalidPatientReferenceException(
                        message: "Invalid patient reference error occurred.",
                        innerException: foreignKeyConstraintConflictException);

                throw await CreateAndLogDependencyValidationException(invalidPatientReferenceException);
            }
            catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
            {
                var lockedPatientException =
                    new LockedPatientException(
                        message: "Locked patient record exception, please try again later",
                        innerException: dbUpdateConcurrencyException);

                throw await CreateAndLogDependencyValidationException(lockedPatientException);
            }
            catch (DbUpdateException databaseUpdateException)
            {
                var failedPatientStorageException =
                    new FailedPatientStorageException(
                        message: "Failed patient storage error occurred, contact support.",
                        innerException: databaseUpdateException);

                throw await CreateAndLogDependencyException(failedPatientStorageException);
            }
            catch (Exception exception)
            {
                var failedPatientServiceException =
                    new FailedPatientServiceException(
                        message: "Failed patient service occurred, please contact support",
                        innerException: exception);

                throw await CreateAndLogServiceException(failedPatientServiceException);
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

                throw await CreateAndLogCriticalDependencyException(failedPatientStorageException);
            }
            catch (Exception exception)
            {
                var failedPatientServiceException =
                    new FailedPatientServiceException(
                        message: "Failed patient service occurred, please contact support",
                        innerException: exception);

                throw await CreateAndLogServiceException(failedPatientServiceException);
            }
        }

        private async ValueTask<PatientValidationException> CreateAndLogValidationException(Xeption exception)
        {
            var patientValidationException =
                new PatientValidationException(
                    message: "Patient validation errors occurred, please try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(patientValidationException);

            return patientValidationException;
        }

        private async ValueTask<PatientDependencyException> CreateAndLogCriticalDependencyException(
            Xeption exception)
        {
            var patientDependencyException =
                new PatientDependencyException(
                    message: "Patient dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogCriticalAsync(patientDependencyException);

            return patientDependencyException;
        }

        private async ValueTask<PatientDependencyValidationException> CreateAndLogDependencyValidationException(
            Xeption exception)
        {
            var patientDependencyValidationException =
                new PatientDependencyValidationException(
                    message: "Patient dependency validation occurred, please try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(patientDependencyValidationException);

            return patientDependencyValidationException;
        }

        private async ValueTask<PatientDependencyException> CreateAndLogDependencyException(
            Xeption exception)
        {
            var patientDependencyException =
                new PatientDependencyException(
                    message: "Patient dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(patientDependencyException);

            return patientDependencyException;
        }

        private async ValueTask<PatientServiceException> CreateAndLogServiceException(
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