// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Foundations.NhsDigitalApis.Exceptions;
using LondonDataServices.IDecide.Core.Models.Orchestrations.NhsDigitalApis.Exceptions;
using Xeptions;

namespace LondonDataServices.IDecide.Core.Services.Orchestrations.NhsDigitalApis
{
    public partial class NhsDigitalApiOrchestrationService
    {
        private delegate ValueTask<string> ReturningStringFunction();

        private async ValueTask<string> TryCatch(ReturningStringFunction returningStringFunction)
        {
            try
            {
                return await returningStringFunction();
            }
            catch (NullNhsDigitalApiOrchestrationSearchCriteriaException
                nullNhsDigitalApiOrchestrationSearchCriteriaException)
            {
                var nhsDigitalApiOrchestrationValidationException =
                    new NhsDigitalApiOrchestrationValidationException(
                        message: "NhsDigitalApi orchestration validation error occurred, " +
                            "please fix the errors and try again.",
                        innerException: nullNhsDigitalApiOrchestrationSearchCriteriaException);

                await this.loggingBroker.LogErrorAsync(nhsDigitalApiOrchestrationValidationException);

                throw nhsDigitalApiOrchestrationValidationException;
            }
            catch (NhsDigitalApiDependencyValidationException nhsDigitalApiDependencyValidationException)
            {
                var nhsDigitalApiOrchestrationDependencyValidationException =
                    new NhsDigitalApiOrchestrationDependencyValidationException(
                        message: "NhsDigitalApi orchestration dependency validation error occurred, " +
                            "please fix the errors and try again.",
                        innerException: nhsDigitalApiDependencyValidationException);

                await this.loggingBroker.LogErrorAsync(nhsDigitalApiOrchestrationDependencyValidationException);

                throw nhsDigitalApiOrchestrationDependencyValidationException;
            }
            catch (NhsDigitalApiValidationException nhsDigitalApiValidationException)
            {
                var nhsDigitalApiOrchestrationDependencyValidationException =
                    new NhsDigitalApiOrchestrationDependencyValidationException(
                        message: "NhsDigitalApi orchestration dependency validation error occurred, " +
                            "please fix the errors and try again.",
                        innerException: nhsDigitalApiValidationException);

                await this.loggingBroker.LogErrorAsync(nhsDigitalApiOrchestrationDependencyValidationException);

                throw nhsDigitalApiOrchestrationDependencyValidationException;
            }
        }
    }
}
