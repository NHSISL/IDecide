// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
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
