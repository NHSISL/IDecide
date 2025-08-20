// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Foundations.Consumers;
using LondonDataServices.IDecide.Core.Models.Foundations.Consumers.Exceptions;
using Xeptions;

namespace LondonDataServices.IDecide.Core.Services.Foundations.Consumers
{
    public partial class ConsumerService
    {
        private delegate ValueTask<Consumer> ReturningConsumerFunction();

        private async ValueTask<Consumer> TryCatch(ReturningConsumerFunction returningConsumerFunction)
        {
            try
            {
                return await returningConsumerFunction();
            }
            catch (NullConsumerException nullConsumerException)
            {
                throw await CreateAndLogValidationException(nullConsumerException);
            }
        }

        private async ValueTask<ConsumerValidationException> CreateAndLogValidationException(Xeption exception)
        {
            var consumerValidationException =
                new ConsumerValidationException(
                    message: "Consumer validation errors occurred, please try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(consumerValidationException);

            return consumerValidationException;
        }
    }
}
