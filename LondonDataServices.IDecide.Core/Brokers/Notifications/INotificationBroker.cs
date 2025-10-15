﻿// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;

namespace LondonDataServices.IDecide.Core.Brokers.Notifications
{
    public interface INotificationBroker
    {
        /// <summary>
        /// Sends an email using the specified template ID and personalisation items.
        /// </summary>
        /// <returns>A string representing the unique identifier of the sent email.</returns>
        /// <exception cref="NotificationValidationProviderException" />
        /// <exception cref="NotificationDependencyProviderException" />
        /// <exception cref="NotificationServiceProviderException" />
        ValueTask<string> SendEmailAsync(
            string templateId,
            string toEmail,
            Dictionary<string, dynamic> personalisation);

        /// <summary>
        /// Sends an SMS using the specified template ID and personalisation items.
        /// </summary>
        /// <returns>A string representing the unique identifier of the sent SMS.</returns>
        /// <exception cref="NotificationValidationProviderException" />
        /// <exception cref="NotificationDependencyProviderException" />
        /// <exception cref="NotificationServiceProviderException" />
        ValueTask<string> SendSmsAsync(
            string templateId,
            string mobileNumber,
            Dictionary<string, dynamic> personalisation);

        /// <summary>
        /// Sends a letter using the specified template ID and personalisation contents.
        /// </summary>
        /// <returns>A string representing the unique identifier of the sent letter.</returns>
        /// <exception cref="NotificationValidationProviderException" />
        /// <exception cref="NotificationDependencyProviderException" />
        /// <exception cref="NotificationServiceProviderException" />
        ValueTask<string> SendLetterAsync(
            string templateId,
            string recipientName,
            string addressLine1,
            string addressLine2,
            string addressLine3,
            string addressLine4,
            string addressLine5,
            string postCode,
            Dictionary<string, dynamic> personalisation,
            string clientReference = "");

        /// <summary>
        /// Sends a letter using the specified template ID and PDF contents.
        /// </summary>
        /// <returns>A string representing the unique identifier of the sent letter.</returns>
        /// <exception cref="NotificationValidationProviderException" />
        /// <exception cref="NotificationDependencyProviderException" />
        /// <exception cref="NotificationServiceProviderException" />
        ValueTask<string> SendPrecompiledLetterAsync(
            string templateId,
            byte[] pdfContents,
            string postage = "");
    }
}
