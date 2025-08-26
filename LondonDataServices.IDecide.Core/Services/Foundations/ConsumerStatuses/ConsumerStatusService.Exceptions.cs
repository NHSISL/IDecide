// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Foundations.ConsumerStatuses;
using LondonDataServices.IDecide.Core.Models.Foundations.ConsumerStatuses.Exceptions;
using Xeptions;

namespace LondonDataServices.IDecide.Core.Services.Foundations.ConsumerStatuses
{
    public partial class ConsumerStatusService
    {
        private delegate ValueTask<ConsumerStatus> ReturningConsumerStatusFunction();

        private async ValueTask<ConsumerStatus> TryCatch(ReturningConsumerStatusFunction returningConsumerStatusFunction)
        {
            try
            {
                return await returningConsumerStatusFunction();
            }
            catch (NullConsumerStatusException nullConsumerStatusException)
            {
                throw await CreateAndLogValidationException(nullConsumerStatusException);
            }
            catch (InvalidConsumerStatusException invalidConsumerStatusException)
            {
                throw await CreateAndLogValidationException(invalidConsumerStatusException);
            }
        }

        private async ValueTask<ConsumerStatusValidationException> CreateAndLogValidationException(Xeption exception)
        {
            var consumerStatusValidationException =
                new ConsumerStatusValidationException(
                    message: "ConsumerStatus validation errors occurred, please try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(consumerStatusValidationException);

            return consumerStatusValidationException;
        }
    }
}
