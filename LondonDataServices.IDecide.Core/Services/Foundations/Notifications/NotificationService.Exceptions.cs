// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Foundations.Notifications.Exceptions;
using Xeptions;

namespace LondonDataServices.IDecide.Core.Services.Foundations.Notifications
{
    public partial class NotificationService
    {
        private delegate ValueTask ReturningNothingFunction();

        private async ValueTask TryCatch(ReturningNothingFunction returningNothingFunction)
        {
            try
            {
                await returningNothingFunction();
            }
            catch (NullNotificationInfoException nullNotificationDecisionException)
            {
                throw await CreateAndLogValidationException(nullNotificationDecisionException);
            }
            catch (InvalidNotificationInfoException invalidNotificationDecisionException)
            {
                throw await CreateAndLogValidationException(invalidNotificationDecisionException);
            }
        }

        private async ValueTask<NotificationValidationException> CreateAndLogValidationException(Xeption exception)
        {
            var notificationValidationException =
                new NotificationValidationException(
                    message: "Notification validation errors occurred, please try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(notificationValidationException);

            return notificationValidationException;
        }
    }
}
