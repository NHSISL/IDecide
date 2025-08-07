// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using LondonDataServices.IDecide.Core.Models.Foundations.Decisions;
using LondonDataServices.IDecide.Core.Models.Foundations.Decisions.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Xeptions;

namespace LondonDataServices.IDecide.Core.Services.Foundations.Decisions
{
    public partial class DecisionService
    {
        private delegate ValueTask<Decision> ReturningDecisionFunction();
        private delegate ValueTask<IQueryable<Decision>> ReturningDecisionsFunction();

        private async ValueTask<Decision> TryCatch(ReturningDecisionFunction returningDecisionFunction)
        {
            try
            {
                return await returningDecisionFunction();
            }
            catch (NullDecisionException nullDecisionException)
            {
                throw await CreateAndLogValidationException(nullDecisionException);
            }
            catch (InvalidDecisionException invalidDecisionException)
            {
                throw await CreateAndLogValidationException(invalidDecisionException);
            }
            catch (SqlException sqlException)
            {
                var failedDecisionStorageException =
                    new FailedDecisionStorageException(
                        message: "Failed decision storage error occurred, contact support.",
                        innerException: sqlException);

                throw await CreateAndLogCriticalDependencyException(failedDecisionStorageException);
            }
            catch (NotFoundDecisionException notFoundDecisionException)
            {
                throw await CreateAndLogValidationException(notFoundDecisionException);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsDecisionException =
                    new AlreadyExistsDecisionException(
                        message: "Decision with the same Id already exists.",
                        innerException: duplicateKeyException);

                throw await CreateAndLogDependencyValidationException(alreadyExistsDecisionException);
            }
            catch (ForeignKeyConstraintConflictException foreignKeyConstraintConflictException)
            {
                var invalidDecisionReferenceException =
                    new InvalidDecisionReferenceException(
                        message: "Invalid decision reference error occurred.",
                        innerException: foreignKeyConstraintConflictException);

                throw await CreateAndLogDependencyValidationException(invalidDecisionReferenceException);
            }
            catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
            {
                var lockedDecisionException =
                    new LockedDecisionException(
                        message: "Locked decision record exception, please try again later",
                        innerException: dbUpdateConcurrencyException);

                throw await CreateAndLogDependencyValidationException(lockedDecisionException);
            }
            catch (DbUpdateException databaseUpdateException)
            {
                var failedDecisionStorageException =
                    new FailedDecisionStorageException(
                        message: "Failed decision storage error occurred, contact support.",
                        innerException: databaseUpdateException);

                throw await CreateAndLogDependencyException(failedDecisionStorageException);
            }
            catch (Exception exception)
            {
                var failedDecisionServiceException =
                    new FailedDecisionServiceException(
                        message: "Failed decision service occurred, please contact support",
                        innerException: exception);

                throw await CreateAndLogServiceException(failedDecisionServiceException);
            }
        }

        private async ValueTask<IQueryable<Decision>> TryCatch(
            ReturningDecisionsFunction returningDecisionsFunction)
        {
            try
            {
                return await returningDecisionsFunction();
            }
            catch (SqlException sqlException)
            {
                var failedDecisionStorageException =
                    new FailedDecisionStorageException(
                        message: "Failed decision storage error occurred, contact support.",
                        innerException: sqlException);

                throw await CreateAndLogCriticalDependencyException(failedDecisionStorageException);
            }
            catch (Exception exception)
            {
                var failedDecisionServiceException =
                    new FailedDecisionServiceException(
                        message: "Failed decision service occurred, please contact support",
                        innerException: exception);

                throw await CreateAndLogServiceException(failedDecisionServiceException);
            }
        }

        private async ValueTask<DecisionValidationException> CreateAndLogValidationException(Xeption exception)
        {
            var decisionValidationException =
                new DecisionValidationException(
                    message: "Decision validation errors occurred, please try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(decisionValidationException);

            return decisionValidationException;
        }

        private async ValueTask<DecisionDependencyException> CreateAndLogCriticalDependencyException(
            Xeption exception)
        {
            var decisionDependencyException =
                new DecisionDependencyException(
                    message: "Decision dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogCriticalAsync(decisionDependencyException);

            return decisionDependencyException;
        }

        private async ValueTask<DecisionDependencyValidationException> CreateAndLogDependencyValidationException(
            Xeption exception)
        {
            var decisionDependencyValidationException =
                new DecisionDependencyValidationException(
                    message: "Decision dependency validation occurred, please try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(decisionDependencyValidationException);

            return decisionDependencyValidationException;
        }

        private async ValueTask<DecisionDependencyException> CreateAndLogDependencyException(
            Xeption exception)
        {
            var decisionDependencyException =
                new DecisionDependencyException(
                    message: "Decision dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(decisionDependencyException);

            return decisionDependencyException;
        }

        private async ValueTask<DecisionServiceException> CreateAndLogServiceException(
            Xeption exception)
        {
            var decisionServiceException =
                new DecisionServiceException(
                    message: "Decision service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(decisionServiceException);

            return decisionServiceException;
        }
    }
}