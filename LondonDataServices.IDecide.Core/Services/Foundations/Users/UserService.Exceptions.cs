// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using LondonDataServices.IDecide.Core.Models.Foundations.Users;
using LondonDataServices.IDecide.Core.Models.Foundations.Users.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Xeptions;

namespace LondonDataServices.IDecide.Core.Services.Foundations.Users
{
    public partial class UserService
    {
        private delegate ValueTask<User> ReturningUserFunction();
        private delegate ValueTask<IQueryable<User>> ReturningUsersFunction();

        private async ValueTask<User> TryCatch(ReturningUserFunction returningUserFunction)
        {
            try
            {
                return await returningUserFunction();
            }
            catch (NullUserException nullUserException)
            {
                throw await CreateAndLogValidationException(nullUserException);
            }
            catch (InvalidUserException invalidUserException)
            {
                throw await CreateAndLogValidationException(invalidUserException);
            }
            catch (NotFoundUserException notFoundUserException)
            {
                throw await CreateAndLogValidationException(notFoundUserException);
            }
            catch (SqlException sqlException)
            {
                var failedUserStorageException =
                    new FailedUserStorageException(
                        message: "Failed user storage error occurred, contact support.",
                        innerException: sqlException,
                        data: sqlException.Data);

                throw await CreateAndLogCriticalDependencyException(failedUserStorageException);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsUserException =
                    new AlreadyExistsUserException(
                        message: "User with the same Id already exists.",
                        innerException: duplicateKeyException,
                        data: duplicateKeyException.Data);

                throw await CreateAndLogDependencyValidationException(alreadyExistsUserException);
            }
            catch (ForeignKeyConstraintConflictException foreignKeyConstraintConflictException)
            {
                var failedUserStorageException =
                    new FailedUserStorageException(
                        message: "Failed user storage error occurred, contact support.",
                        innerException: foreignKeyConstraintConflictException,
                        data: foreignKeyConstraintConflictException.Data);

                throw await CreateAndLogDependencyException(failedUserStorageException);
            }
            catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
            {
                var lockedUserException =
                    new LockedUserException(
                        message: "Locked user record exception, please try again later",
                        innerException: dbUpdateConcurrencyException,
                        data: dbUpdateConcurrencyException.Data);

                throw await CreateAndLogDependencyValidationException(lockedUserException);
            }
            catch (DbUpdateException databaseUpdateException)
            {
                var failedUserStorageException =
                    new FailedUserStorageException(
                        message: "Failed user storage error occurred, contact support.",
                        innerException: databaseUpdateException,
                        data: databaseUpdateException.Data);

                throw await CreateAndLogDependencyException(failedUserStorageException);
            }
            catch (Exception exception)
            {
                var failedUserServiceException =
                    new FailedUserServiceException(
                        message: "Failed user service occurred, please contact support",
                        innerException: exception);

                throw await CreateAndLogServiceException(failedUserServiceException);
            }
        }

        private async ValueTask<IQueryable<User>> TryCatch(ReturningUsersFunction returningUsersFunction)
        {
            try
            {
                return await returningUsersFunction();
            }
            catch (SqlException sqlException)
            {
                var failedUserStorageException =
                     new FailedUserStorageException(
                         message: "Failed user storage error occurred, contact support.",
                         innerException: sqlException,
                         data: sqlException.Data);

                throw await CreateAndLogCriticalDependencyException(failedUserStorageException);
            }
            catch (Exception exception)
            {
                var failedUserServiceException =
                    new FailedUserServiceException(
                        message: "Failed user service occurred, please contact support",
                        innerException: exception);

                throw await CreateAndLogServiceException(failedUserServiceException);
            }
        }

        private async ValueTask<UserValidationException> CreateAndLogValidationException(Xeption exception)
        {
            var userValidationException =
                new UserValidationException(
                    message: "User validation errors occurred, please try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(userValidationException);

            return userValidationException;
        }

        private async ValueTask<UserDependencyException> CreateAndLogCriticalDependencyException(Xeption exception)
        {
            var userDependencyException =
                new UserDependencyException(
                    message: "User dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogCriticalAsync(userDependencyException);

            return userDependencyException;
        }

        private async ValueTask<UserDependencyValidationException> CreateAndLogDependencyValidationException(
            Xeption exception)
        {
            var userDependencyValidationException =
                new UserDependencyValidationException(
                    message: "User dependency validation occurred, please try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(userDependencyValidationException);

            return userDependencyValidationException;
        }

        private async ValueTask<UserDependencyException> CreateAndLogDependencyException(Xeption exception)
        {
            var userDependencyException =
                new UserDependencyException(
                    message: "User dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(userDependencyException);

            return userDependencyException;
        }

        private async ValueTask<UserServiceException> CreateAndLogServiceException(Xeption exception)
        {
            var userServiceException =
                new UserServiceException(
                    message: "User service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(userServiceException);

            return userServiceException;
        }
    }
}
