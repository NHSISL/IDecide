// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using LondonDataServices.IDecide.Core.Models.Foundations.DecisionTypes;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using LondonDataServices.IDecide.Core.Models.Foundations.DecisionTypes.Exceptions;
using Xeptions;

namespace LondonDataServices.IDecide.Core.Services.Foundations.DecisionTypes
{
    public partial class DecisionTypeService
    {
        private delegate ValueTask<DecisionType> ReturningDecisionTypeFunction();
        private delegate ValueTask<IQueryable<DecisionType>> ReturningDecisionTypesFunction();

        private async ValueTask<DecisionType> TryCatch(ReturningDecisionTypeFunction returningDecisionTypeFunction)
        {
            try
            {
                return await returningDecisionTypeFunction();
            }
            catch (NullDecisionTypeException nullDecisionTypeException)
            {
                throw await CreateAndLogValidationException(nullDecisionTypeException);
            }
            catch (InvalidDecisionTypeException invalidDecisionTypeException)
            {
                throw await CreateAndLogValidationException(invalidDecisionTypeException);
            }
            catch (SqlException sqlException)
            {
                var failedDecisionTypeStorageException =
                    new FailedDecisionTypeStorageException(
                        message: "Failed decisionType storage error occurred, contact support.",
                        innerException: sqlException);

                throw await CreateAndLogCriticalDependencyException(failedDecisionTypeStorageException);
            }
            catch (NotFoundDecisionTypeException notFoundDecisionTypeException)
            {
                throw await CreateAndLogValidationException(notFoundDecisionTypeException);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsDecisionTypeException =
                    new AlreadyExistsDecisionTypeException(
                        message: "DecisionType with the same Id already exists.",
                        innerException: duplicateKeyException);

                throw await CreateAndLogDependencyValidationException(alreadyExistsDecisionTypeException);
            }
            catch (ForeignKeyConstraintConflictException foreignKeyConstraintConflictException)
            {
                var invalidDecisionTypeReferenceException =
                    new InvalidDecisionTypeReferenceException(
                        message: "Invalid decisionType reference error occurred.",
                        innerException: foreignKeyConstraintConflictException);

                throw await CreateAndLogDependencyValidationException(invalidDecisionTypeReferenceException);
            }
            catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
            {
                var lockedDecisionTypeException =
                    new LockedDecisionTypeException(
                        message: "Locked decisionType record exception, please try again later",
                        innerException: dbUpdateConcurrencyException);

                throw await CreateAndLogDependencyValidationException(lockedDecisionTypeException);
            }
            catch (DbUpdateException databaseUpdateException)
            {
                var failedDecisionTypeStorageException =
                    new FailedDecisionTypeStorageException(
                        message: "Failed decisionType storage error occurred, contact support.",
                        innerException: databaseUpdateException);

                throw await CreateAndLogDependencyException(failedDecisionTypeStorageException);
            }
            catch (Exception exception)
            {
                var failedDecisionTypeServiceException =
                    new FailedDecisionTypeServiceException(
                        message: "Failed decisionType service occurred, please contact support",
                        innerException: exception);

                throw await CreateAndLogServiceException(failedDecisionTypeServiceException);
            }
        }

        private async ValueTask<IQueryable<DecisionType>> TryCatch(
            ReturningDecisionTypesFunction returningDecisionTypesFunction)
        {
            try
            {
                return await returningDecisionTypesFunction();
            }
            catch (SqlException sqlException)
            {
                var failedDecisionTypeStorageException =
                    new FailedDecisionTypeStorageException(
                        message: "Failed decisionType storage error occurred, contact support.",
                        innerException: sqlException);

                throw await CreateAndLogCriticalDependencyException(failedDecisionTypeStorageException);
            }
            catch (Exception exception)
            {
                var failedDecisionTypeServiceException =
                    new FailedDecisionTypeServiceException(
                        message: "Failed decisionType service occurred, please contact support",
                        innerException: exception);

                throw await CreateAndLogServiceException(failedDecisionTypeServiceException);
            }
        }

        private async ValueTask<DecisionTypeValidationException> CreateAndLogValidationException(Xeption exception)
        {
            var decisionTypeValidationException =
                new DecisionTypeValidationException(
                    message: "DecisionType validation errors occurred, please try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(decisionTypeValidationException);

            return decisionTypeValidationException;
        }

        private async ValueTask<DecisionTypeDependencyException> CreateAndLogCriticalDependencyException(
            Xeption exception)
        {
            var decisionTypeDependencyException =
                new DecisionTypeDependencyException(
                    message: "DecisionType dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogCriticalAsync(decisionTypeDependencyException);

            return decisionTypeDependencyException;
        }

        private async ValueTask<DecisionTypeDependencyValidationException> CreateAndLogDependencyValidationException(
            Xeption exception)
        {
            var decisionTypeDependencyValidationException =
                new DecisionTypeDependencyValidationException(
                    message: "DecisionType dependency validation occurred, please try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(decisionTypeDependencyValidationException);

            return decisionTypeDependencyValidationException;
        }

        private async ValueTask<DecisionTypeDependencyException> CreateAndLogDependencyException(
            Xeption exception)
        {
            var decisionTypeDependencyException =
                new DecisionTypeDependencyException(
                    message: "DecisionType dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(decisionTypeDependencyException);

            return decisionTypeDependencyException;
        }

        private async ValueTask<DecisionTypeServiceException> CreateAndLogServiceException(
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