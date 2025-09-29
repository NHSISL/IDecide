// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Foundations.Notifications;
using Moq;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.Notifications
{
    public partial class NotificationServiceTests
    {
        [Theory]
        [InlineData(NotificationPreference.Email)]
        [InlineData(NotificationPreference.Letter)]
        [InlineData(NotificationPreference.Sms)]
        public async Task ShouldSendSubscriberUsageNotificationAsync(NotificationPreference notificationPreference)
        {
            // given
            NotificationInfo randomNotificationInfo = CreateRandomNotificationInfo();
            randomNotificationInfo.Patient.NotificationPreference = notificationPreference;
            NotificationInfo inputNotificationInfo = randomNotificationInfo;
            Dictionary<string, dynamic> personalisation = GetDecisionPersonalisation(inputNotificationInfo);
            string result = GetRandomString();

            switch (notificationPreference)
            {
                case NotificationPreference.Email:
                    this.notificationConfig.EmailSubscriberUsageTemplateId = GetRandomString();

                    this.notificationBrokerMock.Setup(broker =>
                        broker.SendEmailAsync(
                            this.notificationConfig.EmailSubscriberUsageTemplateId,
                            inputNotificationInfo.Patient.Email,
                            personalisation))
                        .ReturnsAsync(result);

                    break;

                case NotificationPreference.Sms:
                    this.notificationConfig.SmsSubscriberUsageTemplateId = GetRandomString();

                    this.notificationBrokerMock.Setup(broker =>
                        broker.SendSmsAsync(
                            this.notificationConfig.SmsSubscriberUsageTemplateId,
                            inputNotificationInfo.Patient.Phone,
                            personalisation))
                        .ReturnsAsync(result);

                    break;

                case NotificationPreference.Letter:
                    this.notificationConfig.LetterSubscriberUsageTemplateId = GetRandomString();

                    this.notificationBrokerMock.Setup(broker =>
                        broker.SendLetterAsync(
                            this.notificationConfig.LetterSubscriberUsageTemplateId,
                            personalisation,
                            string.Empty))
                        .ReturnsAsync(result);

                    break;
            }

            // when
            await this.notificationService.SendSubscriberUsageNotificationAsync(notificationInfo: inputNotificationInfo);

            // then
            switch (notificationPreference)
            {
                case NotificationPreference.Email:
                    this.notificationBrokerMock.Verify(broker =>
                        broker.SendEmailAsync(
                            this.notificationConfig.EmailSubscriberUsageTemplateId,
                            inputNotificationInfo.Patient.Email,
                            personalisation),
                        Times.Once);

                    break;

                case NotificationPreference.Sms:
                    this.notificationBrokerMock.Verify(broker =>
                        broker.SendSmsAsync(
                            this.notificationConfig.SmsSubscriberUsageTemplateId,
                            inputNotificationInfo.Patient.Phone,
                            personalisation),
                        Times.Once);

                    break;

                case NotificationPreference.Letter:
                    this.notificationBrokerMock.Verify(broker =>
                        broker.SendLetterAsync(
                            notificationConfig.LetterSubscriberUsageTemplateId,
                            personalisation,
                            string.Empty),
                        Times.Once);

                    break;
            }

            this.notificationBrokerMock.VerifyNoOtherCalls();
        }
    }
}
