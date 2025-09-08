// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Brokers.Loggings;
using LondonDataServices.IDecide.Core.Brokers.Notifications;
using LondonDataServices.IDecide.Core.Models.Foundations.Notifications;

namespace LondonDataServices.IDecide.Core.Services.Foundations.Notifications
{
    public partial class NotificationService : INotificationService
    {
        private readonly INotificationBroker notificationBroker;
        private readonly NotificationConfig notificationConfig;
        private readonly ILoggingBroker loggingBroker;

        public NotificationService(
            INotificationBroker notificationBroker,
            NotificationConfig notificationConfig,
            ILoggingBroker loggingBroker)
        {
            this.notificationBroker = notificationBroker;
            this.notificationConfig = notificationConfig;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask SendCodeNotificationAsync(NotificationInfo notificationInfo) =>
            TryCatch(async () =>
            {
                await ValidateNotificationInfo(notificationInfo);

                Dictionary<string, dynamic> personalisation = new Dictionary<string, dynamic>
                {
                    { "patient.nhsNumber", notificationInfo.Patient.NhsNumber },
                    { "patient.title", notificationInfo.Patient.Title },
                    { "patient.givenName", notificationInfo.Patient.GivenName },
                    { "patient.surname", notificationInfo.Patient.Surname },
                    { "patient.dateOfBirth", notificationInfo.Patient.DateOfBirth },
                    { "patient.gender", notificationInfo.Patient.Gender },
                    { "patient.email", notificationInfo.Patient.Email },
                    { "patient.phone", notificationInfo.Patient.Phone },
                    { "patient.address", notificationInfo.Patient.Address },
                    { "patient.postCode", notificationInfo.Patient.PostCode },
                    { "patient.validationCode", notificationInfo.Patient.ValidationCode },
                    { "patient.validationCodeExpiresOn", notificationInfo.Patient.ValidationCodeExpiresOn },
                    { "decision.decisionChoice", notificationInfo.Decision.DecisionChoice },
                    { "decision.decisionType.name", notificationInfo.Decision.DecisionType.Name }
                };

                AddIfNotNull(
                    personalisation,
                    "decision.responsiblePersonGivenName",
                    notificationInfo.Decision.ResponsiblePersonGivenName);

                AddIfNotNull(
                    personalisation,
                    "decision.responsiblePersonSurname",
                    notificationInfo.Decision.ResponsiblePersonSurname);

                AddIfNotNull(
                    personalisation,
                    "decision.responsiblePersonRelationship",
                    notificationInfo.Decision.ResponsiblePersonRelationship);

                switch (notificationInfo.Patient.NotificationPreference)
                {
                    case NotificationPreference.Email:
                        await ValidateSendEmailInputsOnSendCode(
                           notificationInfo.Patient.Email,
                           this.notificationConfig.EmailCodeTemplateId,
                           personalisation);

                        await this.notificationBroker.SendEmailAsync(
                            notificationInfo.Patient.Email,
                            this.notificationConfig.EmailCodeTemplateId,
                            personalisation);

                        break;

                    case NotificationPreference.Sms:
                        await ValidateSendSmsInputsOnSendCode(
                           this.notificationConfig.SmsCodeTemplateId, personalisation);

                        await this.notificationBroker.SendSmsAsync(
                            this.notificationConfig.SmsCodeTemplateId,
                            notificationInfo.Patient.Phone,
                            personalisation);

                        break;

                    case NotificationPreference.Letter:
                        await ValidateSendLetterInputsOnSendCode(
                           this.notificationConfig.LetterCodeTemplateId, personalisation);

                        await this.notificationBroker.SendLetterAsync(
                            this.notificationConfig.LetterCodeTemplateId,
                            personalisation,
                            string.Empty);

                        break;
                }
            });

        public ValueTask SendSubmissionSuccessNotificationAsync(NotificationInfo notificationInfo) =>
            TryCatch(async () =>
            {
                await ValidateNotificationInfo(notificationInfo);

                Dictionary<string, dynamic> personalisation = new Dictionary<string, dynamic>
                {
                    { "patient.nhsNumber", notificationInfo.Patient.NhsNumber },
                    { "patient.title", notificationInfo.Patient.Title },
                    { "patient.givenName", notificationInfo.Patient.GivenName },
                    { "patient.surname", notificationInfo.Patient.Surname },
                    { "patient.dateOfBirth", notificationInfo.Patient.DateOfBirth },
                    { "patient.gender", notificationInfo.Patient.Gender },
                    { "patient.email", notificationInfo.Patient.Email },
                    { "patient.phone", notificationInfo.Patient.Phone },
                    { "patient.address", notificationInfo.Patient.Address },
                    { "patient.postCode", notificationInfo.Patient.PostCode },
                    { "patient.validationCode", notificationInfo.Patient.ValidationCode },
                    { "patient.validationCodeExpiresOn", notificationInfo.Patient.ValidationCodeExpiresOn },
                    { "decision.decisionChoice", notificationInfo.Decision.DecisionChoice },
                    { "decision.decisionType.name", notificationInfo.Decision.DecisionType.Name }
                };

                AddIfNotNull(
                    personalisation,
                    "decision.responsiblePersonGivenName",
                    notificationInfo.Decision.ResponsiblePersonGivenName);

                AddIfNotNull(
                    personalisation,
                    "decision.responsiblePersonSurname",
                    notificationInfo.Decision.ResponsiblePersonSurname);

                AddIfNotNull(
                    personalisation,
                    "decision.responsiblePersonRelationship",
                    notificationInfo.Decision.ResponsiblePersonRelationship);

                switch (notificationInfo.Patient.NotificationPreference)
                {
                    case NotificationPreference.Email:
                        await ValidateSendEmailInputsOnSendSubmissionSuccess(
                            notificationInfo.Patient.Email,
                            this.notificationConfig.EmailSubmissionSuccessTemplateId,
                            personalisation);

                        await this.notificationBroker.SendEmailAsync(
                            notificationInfo.Patient.Email,
                            this.notificationConfig.EmailSubmissionSuccessTemplateId,
                            personalisation);

                        break;

                    case NotificationPreference.Sms:
                        await ValidateSendSmsInputsOnSendSubmissionSuccess(
                            this.notificationConfig.SmsSubmissionSuccessTemplateId, personalisation);

                        await this.notificationBroker.SendSmsAsync(
                            this.notificationConfig.SmsSubmissionSuccessTemplateId,
                            notificationInfo.Patient.Phone,
                            personalisation);

                        break;

                    case NotificationPreference.Letter:
                        await ValidateSendLetterInputsOnSendSubmissionSuccess(
                            this.notificationConfig.LetterSubmissionSuccessTemplateId, personalisation);

                        await this.notificationBroker.SendLetterAsync(
                            this.notificationConfig.LetterSubmissionSuccessTemplateId,
                            personalisation,
                            string.Empty);

                        break;
                }
            });

        public ValueTask SendSubscriberUsageNotificationAsync(NotificationInfo notificationInfo) =>
            TryCatch(async () =>
            {
                await ValidateNotificationInfo(notificationInfo);

                Dictionary<string, dynamic> personalisation = new Dictionary<string, dynamic>
                {
                    { "patient.nhsNumber", notificationInfo.Patient.NhsNumber },
                    { "patient.title", notificationInfo.Patient.Title },
                    { "patient.givenName", notificationInfo.Patient.GivenName },
                    { "patient.surname", notificationInfo.Patient.Surname },
                    { "patient.dateOfBirth", notificationInfo.Patient.DateOfBirth },
                    { "patient.gender", notificationInfo.Patient.Gender },
                    { "patient.email", notificationInfo.Patient.Email },
                    { "patient.phone", notificationInfo.Patient.Phone },
                    { "patient.address", notificationInfo.Patient.Address },
                    { "patient.postCode", notificationInfo.Patient.PostCode },
                    { "patient.validationCode", notificationInfo.Patient.ValidationCode },
                    { "patient.validationCodeExpiresOn", notificationInfo.Patient.ValidationCodeExpiresOn },
                    { "decision.decisionChoice", notificationInfo.Decision.DecisionChoice },
                    { "decision.decisionType.name", notificationInfo.Decision.DecisionType.Name }
                };

                AddIfNotNull(
                    personalisation,
                    "decision.responsiblePersonGivenName",
                    notificationInfo.Decision.ResponsiblePersonGivenName);

                AddIfNotNull(
                    personalisation,
                    "decision.responsiblePersonSurname",
                    notificationInfo.Decision.ResponsiblePersonSurname);

                AddIfNotNull(
                    personalisation,
                    "decision.responsiblePersonRelationship",
                    notificationInfo.Decision.ResponsiblePersonRelationship);

                switch (notificationInfo.Patient.NotificationPreference)
                {
                    case NotificationPreference.Email:
                        await ValidateSendEmailInputsOnSendSubscriberUsage(
                            notificationInfo.Patient.Email,
                            this.notificationConfig.EmailSubscriberUsageTemplateId,
                            personalisation);

                        await this.notificationBroker.SendEmailAsync(
                            notificationInfo.Patient.Email,
                            this.notificationConfig.EmailSubscriberUsageTemplateId,
                            personalisation);

                        break;

                    case NotificationPreference.Sms:
                        await ValidateSendSmsInputsOnSendSubscriberUsage(
                            this.notificationConfig.SmsSubscriberUsageTemplateId, personalisation);

                        await this.notificationBroker.SendSmsAsync(
                            this.notificationConfig.SmsSubscriberUsageTemplateId,
                            notificationInfo.Patient.Phone,
                            personalisation);

                        break;

                    case NotificationPreference.Letter:
                        await ValidateSendLetterInputsOnSendSubscriberUsage(
                            this.notificationConfig.LetterSubscriberUsageTemplateId, personalisation);

                        await this.notificationBroker.SendLetterAsync(
                            this.notificationConfig.LetterSubscriberUsageTemplateId,
                            personalisation,
                            string.Empty);

                        break;
                }
            });

        private static void AddIfNotNull(Dictionary<string, dynamic> personalisation, string key, object value)
        {
            if (value != null)
                personalisation.Add(key, value);
        }
    }
}
