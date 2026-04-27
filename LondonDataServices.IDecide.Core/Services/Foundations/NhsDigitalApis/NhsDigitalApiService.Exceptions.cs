// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Foundations.NhsDigitalApis.Exceptions;
using Xeptions;

namespace LondonDataServices.IDecide.Core.Services.Foundations.NhsDigitalApis
{
    public partial class NhsDigitalApiService
    {
        private delegate ValueTask<string> ReturningStringFunction();

        private async ValueTask<string> TryCatch(ReturningStringFunction returningStringFunction)
        {
            try
            {
                return await returningStringFunction();
            }
            catch (NullNhsDigitalApiSearchCriteriaException nullNhsDigitalApiSearchCriteriaException)
            {
                throw await CreateAndLogValidationException(nullNhsDigitalApiSearchCriteriaException);
            }
            catch (HttpRequestException httpRequestException)
                when (httpRequestException.StatusCode == HttpStatusCode.BadRequest)
            {
                var clientNhsDigitalApiException =
                    new ClientNhsDigitalApiException(
                        message: "NhsDigitalApi client error occurred, please fix the errors and try again.",
                        innerException: httpRequestException,
                        data: httpRequestException.Data);

                throw await CreateAndLogDependencyValidationException(clientNhsDigitalApiException);
            }
            catch (HttpRequestException httpRequestException)
                when (httpRequestException.StatusCode == HttpStatusCode.InternalServerError)
            {
                var serverNhsDigitalApiException =
                    new ServerNhsDigitalApiException(
                        message: "NhsDigitalApi server error occurred, please contact support.",
                        innerException: httpRequestException,
                        data: httpRequestException.Data);

                throw await CreateAndLogDependencyException(serverNhsDigitalApiException);
            }
            catch (Exception exception)
            {
                var failedNhsDigitalApiServiceException =
                    new FailedNhsDigitalApiServiceException(
                        message: "Failed NhsDigitalApi service error occurred, please contact support.",
                        innerException: exception);

                throw await CreateAndLogServiceException(failedNhsDigitalApiServiceException);
            }
        }

        private async ValueTask<NhsDigitalApiValidationException> CreateAndLogValidationException(
            Xeption exception)
        {
            var nhsDigitalApiValidationException =
                new NhsDigitalApiValidationException(
                    message: "NhsDigitalApi validation error occurred, " +
                        "please fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(nhsDigitalApiValidationException);

            return nhsDigitalApiValidationException;
        }

        private async ValueTask<NhsDigitalApiDependencyValidationException>
            CreateAndLogDependencyValidationException(Xeption exception)
        {
            var nhsDigitalApiDependencyValidationException =
                new NhsDigitalApiDependencyValidationException(
                    message: "NhsDigitalApi dependency validation error occurred, " +
                        "please fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(nhsDigitalApiDependencyValidationException);

            return nhsDigitalApiDependencyValidationException;
        }

        private async ValueTask<NhsDigitalApiDependencyException> CreateAndLogDependencyException(
            Xeption exception)
        {
            var nhsDigitalApiDependencyException =
                new NhsDigitalApiDependencyException(
                    message: "NhsDigitalApi dependency error occurred, please contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(nhsDigitalApiDependencyException);

            return nhsDigitalApiDependencyException;
        }

        private async ValueTask<NhsDigitalApiServiceException> CreateAndLogServiceException(
            Xeption exception)
        {
            var nhsDigitalApiServiceException =
                new NhsDigitalApiServiceException(
                    message: "NhsDigitalApi service error occurred, please contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(nhsDigitalApiServiceException);

            return nhsDigitalApiServiceException;
        }
    }
}
