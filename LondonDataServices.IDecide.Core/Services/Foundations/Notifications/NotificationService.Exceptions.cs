// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using ISL.Providers.Notifications.Abstractions.Models.Exceptions;
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
            catch (InvalidArgumentsNotificationException invalidArgumentsNotificationException)
            {
                throw await CreateAndLogValidationException(invalidArgumentsNotificationException);
            }
            catch (NotificationProviderValidationException notificationProviderValidationException)
            {
                ClientNotificationException clientNotificationException = new ClientNotificationException(
                    message: "Client notification error occurred, contact support.",
                    innerException: notificationProviderValidationException,
                    data: notificationProviderValidationException.Data);

                throw await CreateAndLogDependencyValidationException(clientNotificationException);
            }
            catch (NotificationProviderDependencyException notificationProviderDependencyException)
            {
                ServerNotificationException serverNotificationException = new ServerNotificationException(
                    message: "Server notification error occurred, contact support.",
                    innerException: notificationProviderDependencyException,
                    data: notificationProviderDependencyException.Data);

                throw await CreateAndLogDependencyException(serverNotificationException);
            }
            catch (NotificationProviderServiceException notificationProviderServiceException)
            {
                ServerNotificationException serverNotificationException = new ServerNotificationException(
                    message: "Server notification error occurred, contact support.",
                    innerException: notificationProviderServiceException,
                    data: notificationProviderServiceException.Data);

                throw await CreateAndLogDependencyException(serverNotificationException);
            }
            catch (Exception exception)
            {
                var failedServiceNotificationException =
                    new FailedServiceNotificationException(
                        message: "Failed service notification error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceException(failedServiceNotificationException);
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

        private async ValueTask<NotificationDependencyValidationException> CreateAndLogDependencyValidationException(
            Xeption exception)
        {
            var notificationDependencyValidationException = new NotificationDependencyValidationException(
                message: "Notification dependency validation error occurred, fix errors and try again.",
                innerException: exception);

            await this.loggingBroker.LogErrorAsync(notificationDependencyValidationException);

            return notificationDependencyValidationException;
        }

        private async ValueTask<NotificationDependencyException> CreateAndLogDependencyException(
            Xeption exception)
        {
            var notificationDependencyException = new NotificationDependencyException(
                message: "Notification dependency error occurred, contact support.",
                innerException: exception);

            await this.loggingBroker.LogErrorAsync(notificationDependencyException);

            return notificationDependencyException;
        }

        private async ValueTask<NotificationServiceException> CreateAndLogServiceException(Xeption exception)
        {
            var notificationServiceException = new NotificationServiceException(
                message: "Notification service error occurred, contact support.",
                innerException: exception);

            await this.loggingBroker.LogErrorAsync(notificationServiceException);

            return notificationServiceException;
        }
    }
}
