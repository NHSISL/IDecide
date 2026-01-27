// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Net;
using System.Net.Http;
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
            catch (NhsLoginNullResponseException nhsLoginNullResponseException)
            {
                throw await CreateAndLogValidationExceptionAsync(nhsLoginNullResponseException);
            }
            catch (HttpRequestException httpRequestException)
                when (httpRequestException.StatusCode == HttpStatusCode.BadRequest)
            {
                var clientNhsLoginException = new ClientNhsLoginException(
                    message: "NHS Login client error occurred, please fix the errors and try again.",
                    innerException: httpRequestException,
                    data: httpRequestException.Data);

                throw await CreateAndLogDependencyValidationExceptionAsync(clientNhsLoginException);
            }
            catch (OperationCanceledException operationCanceledException)
            {
                var clientNhsLoginException = new ClientNhsLoginException(
                    message: "NHS Login client error occurred, please fix the errors and try again.",
                    innerException: operationCanceledException,
                    data: operationCanceledException.Data);

                throw await CreateAndLogDependencyValidationExceptionAsync(clientNhsLoginException);
            }
            catch (HttpRequestException httpRequestException)
                when (httpRequestException.StatusCode == HttpStatusCode.InternalServerError)
            {
                var serverNhsLoginException = new ServerNhsLoginException(
                   message: "NHS Login userinfo endpoint did not return a successful response.",
                   innerException: httpRequestException,
                   data: httpRequestException.Data);

                throw await CreateAndLogDependencyExceptionAsync(serverNhsLoginException);
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
            CreateAndLogDependencyValidationExceptionAsync(Exception exception)
        {
            var dependencyValidationException = new NhsLoginServiceDependencyValidationException(
                message: "NHS Login dependency validation error occurred, fix errors and try again.",
                innerException: exception as Xeption);

            await this.loggingBroker.LogErrorAsync(dependencyValidationException);

            return dependencyValidationException;
        }

        private async ValueTask<NhsLoginServiceDependencyException>
            CreateAndLogDependencyExceptionAsync(Exception exception)
        {
            var dependencyException = new NhsLoginServiceDependencyException(
                message: "NHS Login dependency error occurred, please contact support.",
                innerException: exception as Xeption);

            await this.loggingBroker.LogErrorAsync(dependencyException);

            return dependencyException;
        }

        private async ValueTask<NhsLoginServiceException>
            CreateAndLogServiceExceptionAsync(Xeption exception)
        {
            var serviceException = new NhsLoginServiceException(
                message: "NHS Login service error occurred, please contact support.",
                innerException: exception);

            await this.loggingBroker.LogErrorAsync(serviceException);

            return serviceException;
        }
    }
}