// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Orchestrations.Consumers.Exceptions;
using Xeptions;

namespace LondonDataServices.IDecide.Core.Services.Orchestrations.Consumers
{
    public partial class ConsumerOrchestrationService
    {
        private delegate ValueTask ReturningNothingFunction();

        private async ValueTask TryCatch(ReturningNothingFunction returningNothingFunction)
        {
            try
            {
                await returningNothingFunction();
            }
            catch (InvalidDecisionsException invalidDecisionsException)
            {
                throw await CreateAndLogValidationExceptionAsync(invalidDecisionsException);
            }
        }

        private async ValueTask<ConsumerOrchestrationValidationException>
            CreateAndLogValidationExceptionAsync(Xeption exception)
        {
            var consumerOrchestrationValidationException =
                new ConsumerOrchestrationValidationException(
                    message: "Consumer orchestration validation error occurred, " +
                             "please fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(consumerOrchestrationValidationException);

            return consumerOrchestrationValidationException;
        }
    }
}
