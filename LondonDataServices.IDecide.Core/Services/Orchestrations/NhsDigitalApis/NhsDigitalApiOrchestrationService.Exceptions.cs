// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Orchestrations.NhsDigitalApis.Exceptions;
using Xeptions;

namespace LondonDataServices.IDecide.Core.Services.Orchestrations.NhsDigitalApis
{
    public partial class NhsDigitalApiOrchestrationService
    {
        private delegate ValueTask ReturningNothingFunction();

        private async ValueTask TryCatch(ReturningNothingFunction returningNothingFunction)
        {
            try
            {
                await returningNothingFunction();
            }
            catch (InvalidNhsDigitalApiOrchestrationArgumentException
                invalidNhsDigitalApiOrchestrationArgumentException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    invalidNhsDigitalApiOrchestrationArgumentException);
            }
            catch (System.Exception exception)
            {
                var failedNhsDigitalApiOrchestrationServiceException =
                    new FailedNhsDigitalApiOrchestrationServiceException(
                        message: "Failed NhsDigitalApi orchestration service error occurred, " +
                            "please contact support.",
                        innerException: exception);

                throw await CreateAndLogServiceExceptionAsync(
                    failedNhsDigitalApiOrchestrationServiceException);
            }
        }

        private async ValueTask<NhsDigitalApiOrchestrationValidationException>
            CreateAndLogValidationExceptionAsync(Xeption exception)
        {
            var nhsDigitalApiOrchestrationValidationException =
                new NhsDigitalApiOrchestrationValidationException(
                    message: "NhsDigitalApi orchestration validation error occurred, " +
                        "please fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(nhsDigitalApiOrchestrationValidationException);

            return nhsDigitalApiOrchestrationValidationException;
        }

        private async ValueTask<NhsDigitalApiOrchestrationServiceException>
            CreateAndLogServiceExceptionAsync(Xeption exception)
        {
            var nhsDigitalApiOrchestrationServiceException =
                new NhsDigitalApiOrchestrationServiceException(
                    message: "NhsDigitalApi orchestration service error occurred, please contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(nhsDigitalApiOrchestrationServiceException);

            return nhsDigitalApiOrchestrationServiceException;
        }
    }
}
