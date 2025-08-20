// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using LondonDataServices.IDecide.Core.Models.Foundations.Consumers;
using LondonDataServices.IDecide.Core.Models.Foundations.Consumers.Exceptions;
using Microsoft.Data.SqlClient;
using Xeptions;

namespace LondonDataServices.IDecide.Core.Services.Foundations.Consumers
{
    public partial class ConsumerService
    {
        private delegate ValueTask<Consumer> ReturningConsumerFunction();

        private async ValueTask<Consumer> TryCatch(ReturningConsumerFunction returningConsumerFunction)
        {
            try
            {
                return await returningConsumerFunction();
            }
            catch (NullConsumerException nullConsumerException)
            {
                throw await CreateAndLogValidationException(nullConsumerException);
            }
            catch (InvalidConsumerException invalidConsumerException)
            {
                throw await CreateAndLogValidationException(invalidConsumerException);
            }
            catch (SqlException sqlException)
            {
                var failedConsumerStorageException =
                    new FailedConsumerStorageException(
                        message: "Failed consumer storage error occurred, contact support.",
                        innerException: sqlException);

                throw await CreateAndLogCriticalDependencyException(failedConsumerStorageException);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsConsumerException =
                    new AlreadyExistsConsumerException(
                        message: "Consumer with the same Id already exists.",
                        innerException: duplicateKeyException);

                throw await CreateAndLogDependencyValidationException(alreadyExistsConsumerException);
            }
            catch (ForeignKeyConstraintConflictException foreignKeyConstraintConflictException)
            {
                var invalidConsumerReferenceException =
                    new InvalidConsumerReferenceException(
                        message: "Invalid consumer reference error occurred.",
                        innerException: foreignKeyConstraintConflictException);

                throw await CreateAndLogDependencyValidationException(invalidConsumerReferenceException);
            }
        }

        private async ValueTask<ConsumerValidationException> CreateAndLogValidationException(Xeption exception)
        {
            var consumerValidationException =
                new ConsumerValidationException(
                    message: "Consumer validation errors occurred, please try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(consumerValidationException);

            return consumerValidationException;
        }

        private async ValueTask<ConsumerDependencyException> CreateAndLogCriticalDependencyException(
            Xeption exception)
        {
            var consumerDependencyException =
                new ConsumerDependencyException(
                    message: "Consumer dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogCriticalAsync(consumerDependencyException);

            return consumerDependencyException;
        }

        private async ValueTask<ConsumerDependencyValidationException> CreateAndLogDependencyValidationException(
            Xeption exception)
        {
            var consumerDependencyValidationException =
                new ConsumerDependencyValidationException(
                    message: "Consumer dependency validation occurred, please try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(consumerDependencyValidationException);

            return consumerDependencyValidationException;
        }
    }
}
