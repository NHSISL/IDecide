// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using ISL.Providers.Notifications.Abstractions;

namespace LondonDataServices.IDecide.Core.Brokers.Notifications
{
    public class NotificationBroker : INotificationBroker
    {
        private readonly INotificationAbstractionProvider notificationAbstractionProvider;

        public NotificationBroker(
            INotificationAbstractionProvider notificationAbstractionProvider)
        {
            this.notificationAbstractionProvider = notificationAbstractionProvider;
        }

        public async ValueTask<string> SendEmailAsync(
            string templateId,
            string toEmail,
            Dictionary<string, dynamic> personalisation) =>
            await notificationAbstractionProvider
                .SendEmailAsync(templateId, toEmail, personalisation, null);

        /// <summary>
        /// Sends a SMS using the specified template ID and personalisation items.
        /// </summary>
        /// <returns>A string representing the unique identifier of the sent SMS.</returns>
        /// <exception cref="NotificationValidationProviderException" />
        /// <exception cref="NotificationDependencyProviderException" />
        /// <exception cref="NotificationServiceProviderException" />
        public async ValueTask<string> SendSmsAsync(
            string templateId,
            string mobileNumber,
            Dictionary<string, dynamic> personalisation) =>
            await notificationAbstractionProvider.SendSmsAsync(templateId, mobileNumber, personalisation);

        /// <summary>
        /// Sends a letter using the specified template ID and personalisation contents.
        /// </summary>
        /// <returns>A string representing the unique identifier of the sent letter.</returns>
        /// <exception cref="NotificationValidationProviderException" />
        /// <exception cref="NotificationDependencyProviderException" />
        /// <exception cref="NotificationServiceProviderException" />
        public async ValueTask<string> SendLetterAsync(
            string templateId,
            string recipientName,
            string addressLine1,
            string addressLine2,
            string addressLine3,
            string addressLine4,
            string addressLine5,
            string postCode,
            Dictionary<string, dynamic> personalisation,
            string clientReference = "")
        {
            return await notificationAbstractionProvider.SendLetterAsync(
                templateId,
                recipientName,
                addressLine1,
                addressLine2,
                addressLine3,
                addressLine4,
                addressLine5,
                postCode,
                personalisation,
                clientReference);

        }

        /// <summary>
        /// Sends a letter using the specified template ID and PDF contents.
        /// </summary>
        /// <returns>A string representing the unique identifier of the sent letter.</returns>
        /// <exception cref="NotificationValidationProviderException" />
        /// <exception cref="NotificationDependencyProviderException" />
        /// <exception cref="NotificationServiceProviderException" />
        public async ValueTask<string> SendPrecompiledLetterAsync(
            string templateId,
            byte[] pdfContents,
            string postage = "") =>
            await notificationAbstractionProvider.SendPrecompiledLetterAsync(templateId, pdfContents, postage);
    }
}
