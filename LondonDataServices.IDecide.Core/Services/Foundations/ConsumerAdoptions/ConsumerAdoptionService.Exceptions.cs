// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using LondonDataServices.IDecide.Core.Models.Foundations.ConsumerAdoptions;
using LondonDataServices.IDecide.Core.Models.Foundations.ConsumerAdoptions.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Xeptions;

namespace LondonDataServices.IDecide.Core.Services.Foundations.ConsumerAdoptions
{
    public partial class ConsumerAdoptionService
    {
        private delegate ValueTask<ConsumerAdoption> ReturningConsumerAdoptionFunction();
        private delegate ValueTask<IQueryable<ConsumerAdoption>> ReturningConsumerAdoptionsFunction();

        private async ValueTask<ConsumerAdoption> TryCatch(
            ReturningConsumerAdoptionFunction returningConsumerAdoptionFunction)
        {
            try
            {
                return await returningConsumerAdoptionFunction();
            }
            catch (NullConsumerAdoptionException nullConsumerAdoptionException)
            {
                throw await CreateAndLogValidationException(nullConsumerAdoptionException);
            }
            catch (InvalidConsumerAdoptionException invalidConsumerAdoptionException)
            {
                throw await CreateAndLogValidationException(invalidConsumerAdoptionException);
            }
            catch (SqlException sqlException)
            {
                var failedConsumerAdoptionStorageException =
                    new FailedConsumerAdoptionStorageException(
                        message: "Failed consumerAdoption storage error occurred, contact support.",
                        innerException: sqlException);

                throw await CreateAndLogCriticalDependencyException(failedConsumerAdoptionStorageException);
            }
            catch (NotFoundConsumerAdoptionException notFoundConsumerAdoptionException)
            {
                throw await CreateAndLogValidationException(notFoundConsumerAdoptionException);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsConsumerAdoptionException =
                    new AlreadyExistsConsumerAdoptionException(
                        message: "ConsumerAdoption with the same Id already exists.",
                        innerException: duplicateKeyException);

                throw await CreateAndLogDependencyValidationException(alreadyExistsConsumerAdoptionException);
            }
            catch (ForeignKeyConstraintConflictException foreignKeyConstraintConflictException)
            {
                var invalidConsumerAdoptionReferenceException =
                    new InvalidConsumerAdoptionReferenceException(
                        message: "Invalid consumerAdoption reference error occurred.",
                        innerException: foreignKeyConstraintConflictException);

                throw await CreateAndLogDependencyValidationException(invalidConsumerAdoptionReferenceException);
            }
            catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
            {
                var lockedConsumerAdoptionException =
                    new LockedConsumerAdoptionException(
                        message: "Locked consumerAdoption record exception, please try again later",
                        innerException: dbUpdateConcurrencyException);

                throw await CreateAndLogDependencyValidationException(lockedConsumerAdoptionException);
            }
            catch (DbUpdateException databaseUpdateException)
            {
                var failedConsumerAdoptionStorageException =
                    new FailedConsumerAdoptionStorageException(
                        message: "Failed consumerAdoption storage error occurred, contact support.",
                        innerException: databaseUpdateException);

                throw await CreateAndLogDependencyException(failedConsumerAdoptionStorageException);
            }
            catch (Exception exception)
            {
                var failedConsumerAdoptionServiceException =
                    new FailedConsumerAdoptionServiceException(
                        message: "Failed consumerAdoption service occurred, please contact support",
                        innerException: exception);

                throw await CreateAndLogServiceException(failedConsumerAdoptionServiceException);
            }
        }

        private async ValueTask<IQueryable<ConsumerAdoption>> TryCatch(
            ReturningConsumerAdoptionsFunction returningConsumerAdoptionsFunction)
        {
            try
            {
                return await returningConsumerAdoptionsFunction();
            }
            catch (SqlException sqlException)
            {
                var failedConsumerAdoptionStorageException =
                    new FailedConsumerAdoptionStorageException(
                        message: "Failed consumerAdoption storage error occurred, contact support.",
                        innerException: sqlException);

                throw await CreateAndLogCriticalDependencyException(failedConsumerAdoptionStorageException);
            }
            catch (Exception exception)
            {
                var failedConsumerAdoptionServiceException =
                    new FailedConsumerAdoptionServiceException(
                        message: "Failed consumerAdoption service occurred, please contact support",
                        innerException: exception);

                throw await CreateAndLogServiceException(failedConsumerAdoptionServiceException);
            }
        }

        private async ValueTask<ConsumerAdoptionValidationException> CreateAndLogValidationException(Xeption exception)
        {
            var consumerAdoptionValidationException =
                new ConsumerAdoptionValidationException(
                    message: "ConsumerAdoption validation errors occurred, please try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(consumerAdoptionValidationException);

            return consumerAdoptionValidationException;
        }

        private async ValueTask<ConsumerAdoptionDependencyException> CreateAndLogCriticalDependencyException(
            Xeption exception)
        {
            var consumerAdoptionDependencyException =
                new ConsumerAdoptionDependencyException(
                    message: "ConsumerAdoption dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogCriticalAsync(consumerAdoptionDependencyException);

            return consumerAdoptionDependencyException;
        }

        private async ValueTask<ConsumerAdoptionDependencyValidationException> CreateAndLogDependencyValidationException(
            Xeption exception)
        {
            var consumerAdoptionDependencyValidationException =
                new ConsumerAdoptionDependencyValidationException(
                    message: "ConsumerAdoption dependency validation occurred, please try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(consumerAdoptionDependencyValidationException);

            return consumerAdoptionDependencyValidationException;
        }

        private async ValueTask<ConsumerAdoptionDependencyException> CreateAndLogDependencyException(
            Xeption exception)
        {
            var consumerAdoptionDependencyException =
                new ConsumerAdoptionDependencyException(
                    message: "ConsumerAdoption dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(consumerAdoptionDependencyException);

            return consumerAdoptionDependencyException;
        }

        private async ValueTask<ConsumerAdoptionServiceException> CreateAndLogServiceException(Xeption exception)
        {
            var consumerAdoptionServiceException =
                new ConsumerAdoptionServiceException(
                    message: "ConsumerAdoption service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(consumerAdoptionServiceException);

            return consumerAdoptionServiceException;
        }
    }
}
