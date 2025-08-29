// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using LondonDataServices.IDecide.Core.Models.Foundations.ConsumerStatuses;
using LondonDataServices.IDecide.Core.Models.Foundations.ConsumerStatuses.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Xeptions;

namespace LondonDataServices.IDecide.Core.Services.Foundations.ConsumerStatuses
{
    public partial class ConsumerStatusService
    {
        private delegate ValueTask<ConsumerStatus> ReturningConsumerStatusFunction();

        private async ValueTask<ConsumerStatus> TryCatch(ReturningConsumerStatusFunction returningConsumerStatusFunction)
        {
            try
            {
                return await returningConsumerStatusFunction();
            }
            catch (NullConsumerStatusException nullConsumerStatusException)
            {
                throw await CreateAndLogValidationException(nullConsumerStatusException);
            }
            catch (InvalidConsumerStatusException invalidConsumerStatusException)
            {
                throw await CreateAndLogValidationException(invalidConsumerStatusException);
            }
            catch (SqlException sqlException)
            {
                var failedConsumerStatusStorageException =
                    new FailedConsumerStatusStorageException(
                        message: "Failed consumerStatus storage error occurred, contact support.",
                        innerException: sqlException);

                throw await CreateAndLogCriticalDependencyException(failedConsumerStatusStorageException);
            }
            catch (NotFoundConsumerStatusException notFoundConsumerStatusException)
            {
                throw await CreateAndLogValidationException(notFoundConsumerStatusException);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsConsumerStatusException =
                    new AlreadyExistsConsumerStatusException(
                        message: "ConsumerStatus with the same Id already exists.",
                        innerException: duplicateKeyException);

                throw await CreateAndLogDependencyValidationException(alreadyExistsConsumerStatusException);
            }
            catch (ForeignKeyConstraintConflictException foreignKeyConstraintConflictException)
            {
                var invalidConsumerStatusReferenceException =
                    new InvalidConsumerStatusReferenceException(
                        message: "Invalid consumerStatus reference error occurred.",
                        innerException: foreignKeyConstraintConflictException);

                throw await CreateAndLogDependencyValidationException(invalidConsumerStatusReferenceException);
            }
            catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
            {
                var lockedConsumerStatusException =
                    new LockedConsumerStatusException(
                        message: "Locked consumerStatus record exception, please try again later",
                        innerException: dbUpdateConcurrencyException);

                throw await CreateAndLogDependencyValidationException(lockedConsumerStatusException);
            }
            catch (DbUpdateException databaseUpdateException)
            {
                var failedConsumerStatusStorageException =
                    new FailedConsumerStatusStorageException(
                        message: "Failed consumerStatus storage error occurred, contact support.",
                        innerException: databaseUpdateException);

                throw await CreateAndLogDependencyException(failedConsumerStatusStorageException);
            }
            catch (Exception exception)
            {
                var failedConsumerStatusServiceException =
                    new FailedConsumerStatusServiceException(
                        message: "Failed consumerStatus service occurred, please contact support",
                        innerException: exception);

                throw await CreateAndLogServiceException(failedConsumerStatusServiceException);
            }
        }

        private async ValueTask<ConsumerStatusValidationException> CreateAndLogValidationException(Xeption exception)
        {
            var consumerStatusValidationException =
                new ConsumerStatusValidationException(
                    message: "ConsumerStatus validation errors occurred, please try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(consumerStatusValidationException);

            return consumerStatusValidationException;
        }

        private async ValueTask<ConsumerStatusDependencyException> CreateAndLogCriticalDependencyException(
            Xeption exception)
        {
            var consumerStatusDependencyException =
                new ConsumerStatusDependencyException(
                    message: "ConsumerStatus dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogCriticalAsync(consumerStatusDependencyException);

            return consumerStatusDependencyException;
        }

        private async ValueTask<ConsumerStatusDependencyValidationException> CreateAndLogDependencyValidationException(
            Xeption exception)
        {
            var consumerStatusDependencyValidationException =
                new ConsumerStatusDependencyValidationException(
                    message: "ConsumerStatus dependency validation occurred, please try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(consumerStatusDependencyValidationException);

            return consumerStatusDependencyValidationException;
        }

        private async ValueTask<ConsumerStatusDependencyException> CreateAndLogDependencyException(
            Xeption exception)
        {
            var consumerStatusDependencyException =
                new ConsumerStatusDependencyException(
                    message: "ConsumerStatus dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(consumerStatusDependencyException);

            return consumerStatusDependencyException;
        }

        private async ValueTask<ConsumerStatusServiceException> CreateAndLogServiceException(Xeption exception)
        {
            var consumerStatusServiceException =
                new ConsumerStatusServiceException(
                    message: "ConsumerStatus service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(consumerStatusServiceException);

            return consumerStatusServiceException;
        }
    }
}
