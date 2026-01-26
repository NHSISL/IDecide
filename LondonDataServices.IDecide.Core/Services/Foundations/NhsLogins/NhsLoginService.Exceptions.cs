// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Foundations.NhsLogins;
using LondonDataServices.IDecide.Core.Models.Foundations.NhsLogins.Exceptions;
using Xeptions;

namespace LondonDataServices.IDecide.Core.Services.Foundations.NhsLogins
{
    public partial class NhsLoginService
    {
        private delegate ValueTask<NhsLoginUserInfo> ReturningNhsLoginFunction();

        private async ValueTask<NhsLoginUserInfo> TryCatch(ReturningNhsLoginFunction returningNhsLoginFunction)
        {
            try
            {
                return await returningNhsLoginFunction();
            }
            catch (InvalidArgumentsNhsLoginServiceException invalidArgumentsException)
            {
                throw await CreateAndLogValidationExceptionAsync(invalidArgumentsException);
            }
            catch (NhsLoginUserInfoException userInfoException)
            {
                throw await CreateAndLogValidationExceptionAsync(userInfoException);
            }
            catch (NhsLoginServiceDependencyValidationException dependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(dependencyValidationException);
            }
            catch (NhsLoginServiceDependencyException dependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(dependencyException);
            }
            catch (FailedNhsLoginServiceException failedServiceException)
            {
                throw await CreateAndLogServiceExceptionAsync(failedServiceException);
            }
            catch (NhsLoginServiceServiceException serviceException)
            {
                throw await CreateAndLogServiceExceptionAsync(serviceException);
            }
            catch (Exception exception)
            {
                var failedNhsLoginServiceException = new FailedNhsLoginServiceException(
                    message: "Failed NHS Login service error occurred, please contact support.",
                    innerException: exception,
                    data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(failedNhsLoginServiceException);
            }
        }

        private async ValueTask<NhsLoginServiceDependencyValidationException>
            CreateAndLogValidationExceptionAsync(Xeption exception)
        {
            var validationException = new NhsLoginServiceDependencyValidationException(
                message: "NHS Login validation error occurred, please fix the errors and try again.",
                innerException: exception);

            await this.loggingBroker.LogErrorAsync(validationException);

            return validationException;
        }

        private async ValueTask<NhsLoginServiceDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(Xeption exception)
        {
            var dependencyValidationException = new NhsLoginServiceDependencyValidationException(
                message: "NHS Login dependency validation error occurred, fix errors and try again.",
                innerException: exception);

            await this.loggingBroker.LogErrorAsync(dependencyValidationException);

            return dependencyValidationException;
        }

        private async ValueTask<NhsLoginServiceDependencyException>
            CreateAndLogDependencyExceptionAsync(Xeption exception)
        {
            var dependencyException = new NhsLoginServiceDependencyException(
                message: "NHS Login dependency error occurred, please contact support.",
                innerException: exception);

            await this.loggingBroker.LogErrorAsync(dependencyException);

            return dependencyException;
        }

        private async ValueTask<NhsLoginServiceServiceException>
            CreateAndLogServiceExceptionAsync(Xeption exception)
        {
            var serviceException = new NhsLoginServiceServiceException(
                message: "NHS Login service error occurred, please contact support.",
                innerException: exception);

            await this.loggingBroker.LogErrorAsync(serviceException);

            return serviceException;
        }
    }
}