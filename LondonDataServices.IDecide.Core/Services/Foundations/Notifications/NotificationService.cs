// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Brokers.Notifications;
using LondonDataServices.IDecide.Core.Models.Foundations.Notifications;

namespace LondonDataServices.IDecide.Core.Services.Foundations.Notifications
{
    public partial class NotificationService : INotificationService
    {
        private readonly INotificationBroker notificationBroker;
        private readonly NotificationConfig notificationConfig;

        public NotificationService(INotificationBroker notificationBroker, NotificationConfig notificationConfig)
        {
            this.notificationBroker = notificationBroker;
            this.notificationConfig = notificationConfig;
        }

        public async ValueTask SendCodeNotificationAsync(NotificationInfo notificationInfo)
        {
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
                { "decision.responsiblePersonGivenName", notificationInfo.Decision.ResponsiblePersonGivenName },
                { "decision.responsiblePersonSurname", notificationInfo.Decision.ResponiblePersonSurname },
                { "decision.responsiblePersonRelationship", notificationInfo.Decision.ResponsiblePersonRelationship },
                { "decision.decisionType.name", notificationInfo.Decision.DecisionType.Name }
            };

            switch (notificationInfo.Patient.NotificationPreference)
            {
                case NotificationPreference.Email:
                    await notificationBroker.SendEmailAsync(
                        notificationInfo.Patient.Email,
                        notificationConfig.EmailCodeTemplateId,
                        personalisation);
                    break;

                case NotificationPreference.Sms:
                    await notificationBroker.SendSmsAsync(
                        notificationConfig.SmsCodeTemplateId,
                        personalisation);
                    break;
                    
                case NotificationPreference.Letter:
                    await notificationBroker.SendLetterAsync(
                        notificationConfig.LetterCodeTemplateId,
                        personalisation,
                        string.Empty);
                    break;
            }
        }
    }
}
