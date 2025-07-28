// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using EFxceptions.Models.Exceptions;
using LondonDataServices.IDecide.Core.Models.Foundations.DecisionTypes;
using LondonDataServices.IDecide.Core.Models.Foundations.DecisionTypes.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xeptions;

namespace LondonDataServices.IDecide.Core.Services.Foundations.DecisionTypes
{
    public partial class DecisionTypeService
    {
        private delegate ValueTask<DecisionType> ReturningDecisionTypeFunction();

        private async ValueTask<DecisionType> TryCatch(ReturningDecisionTypeFunction returningDecisionTypeFunction)
        {
            try
            {
                return await returningDecisionTypeFunction();
            }
            catch (NullDecisionTypeException nullDecisionTypeException)
            {
                throw await CreateAndLogValidationExceptionAsync(nullDecisionTypeException);
            }
            catch (InvalidDecisionTypeException invalidDecisionTypeException)
            {
                throw await CreateAndLogValidationExceptionAsync(invalidDecisionTypeException);
            }
            catch (SqlException sqlException)
            {
                var failedDecisionTypeStorageException =
                    new FailedDecisionTypeStorageException(
                        message: "Failed decision type storage error occurred, contact support.",
                        innerException: sqlException);

                throw await CreateAndLogCriticalDependencyExceptionAsync(failedDecisionTypeStorageException);
            }
            catch (NotFoundDecisionTypeException notFoundDecisionTypeException)
            {
                throw await CreateAndLogValidationExceptionAsync(notFoundDecisionTypeException);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsDecisionTypeException =
                    new AlreadyExistsDecisionTypeException(
                        message: "DecisionType with the same Id already exists.",
                        innerException: duplicateKeyException,
                        data: duplicateKeyException.Data);

                throw await CreateAndLogDependencyValidationExceptionAsync(alreadyExistsDecisionTypeException);
            }
            catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
            {
                var lockedDecisionTypeException =
                    new LockedDecisionTypeException(
                        message: "Locked decision type record exception, please fix errors and try again.",
                        innerException: dbUpdateConcurrencyException);

                throw await CreateAndLogDependencyValidationExceptionAsync(lockedDecisionTypeException);
            }
            catch (DbUpdateException databaseUpdateException)
            {
                var failedDecisionTypeStorageException =
                    new FailedDecisionTypeStorageException(
                        message: "Failed decision type storage error occurred, contact support.",
                        innerException: databaseUpdateException);

                throw await CreateAndLogDependencyExceptionAsync(failedDecisionTypeStorageException);
            }
            catch (Exception exception)
            {
                var failedDecisionTypeServiceException =
                    new FailedDecisionTypeServiceException(
                        message: "Failed decision type service occurred, please contact support",
                        innerException: exception);

                throw await CreateAndLogServiceExceptionAsync(failedDecisionTypeServiceException);
            }
        }

        private async ValueTask<DecisionTypeValidationException> CreateAndLogValidationExceptionAsync(
            Xeption exception)
        {
            var decisionTypeValidationException =
                new DecisionTypeValidationException(
                    message: "DecisionType validation error occurred, please fix errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(decisionTypeValidationException);

            return decisionTypeValidationException;
        }

        private async ValueTask<DecisionTypeDependencyException> CreateAndLogCriticalDependencyExceptionAsync(
            Xeption exception)
        {
            var decisionTypeDependencyException =
                new DecisionTypeDependencyException(
                    message: "DecisionType dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogCriticalAsync(decisionTypeDependencyException);

            return decisionTypeDependencyException;
        }

        private async ValueTask<DecisionTypeDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(Xeption exception)
        {
            var decisionTypeDependencyValidationException =
                new DecisionTypeDependencyValidationException(
                    message: "DecisionType dependency validation occurred, please fix errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(decisionTypeDependencyValidationException);

            return decisionTypeDependencyValidationException;
        }

        private async ValueTask<DecisionTypeDependencyException> CreateAndLogDependencyExceptionAsync(
            Xeption exception)
        {
            var decisionTypeDependencyException =
                new DecisionTypeDependencyException(
                    message: "DecisionType dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(decisionTypeDependencyException);

            return decisionTypeDependencyException;
        }

        private async ValueTask<DecisionTypeServiceException> CreateAndLogServiceExceptionAsync(
            Xeption exception)
        {
            var decisionTypeServiceException =
                new DecisionTypeServiceException(
                    message: "DecisionType service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(decisionTypeServiceException);

            return decisionTypeServiceException;
        }
    }
}
