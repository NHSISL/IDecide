// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
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
        public async Task ShouldSendCodeNotificationAsync(NotificationPreference notificationPreference)
        {
            // given
            NotificationInfo randomNotificationInfo = CreateRandomNotificationInfo();
            randomNotificationInfo.Patient.NotificationPreference = notificationPreference;
            NotificationInfo inputNotificationInfo = randomNotificationInfo;
            var addressLines = inputNotificationInfo.Patient.Address.Split(',');
            var addressLine1 = addressLines.ElementAtOrDefault(0) ?? string.Empty;
            var addressLine2 = addressLines.ElementAtOrDefault(1) ?? string.Empty;
            var addressLine3 = addressLines.ElementAtOrDefault(2) ?? string.Empty;
            var addressLine4 = addressLines.ElementAtOrDefault(3) ?? string.Empty;
            var addressLine5 = addressLines.ElementAtOrDefault(4) ?? string.Empty;
            Dictionary<string, dynamic> personalisation = GetCodePersonalisation(inputNotificationInfo);

            string result = GetRandomString();

            switch (notificationPreference)
            {
                case NotificationPreference.Email:
                    this.notificationConfig.EmailCodeTemplateId = GetRandomString();

                    this.notificationBrokerMock.Setup(broker =>
                        broker.SendEmailAsync(
                            this.notificationConfig.EmailCodeTemplateId,
                            inputNotificationInfo.Patient.Email,
                            personalisation))
                        .ReturnsAsync(result);

                    break;

                case NotificationPreference.Sms:
                    this.notificationConfig.SmsCodeTemplateId = GetRandomString();

                    this.notificationBrokerMock.Setup(broker =>
                        broker.SendSmsAsync(
                            this.notificationConfig.SmsCodeTemplateId,
                            inputNotificationInfo.Patient.Phone,
                            personalisation))
                        .ReturnsAsync(result);

                    break;

                case NotificationPreference.Letter:
                    this.notificationConfig.LetterCodeTemplateId = GetRandomString();

                    this.notificationBrokerMock.Setup(broker =>
                        broker.SendLetterAsync(
                            this.notificationConfig.LetterCodeTemplateId,

                            $"{inputNotificationInfo.Patient.Title} " +
                                $"{inputNotificationInfo.Patient.GivenName} " +
                                    $"{inputNotificationInfo.Patient.Surname}",

                            addressLine1,
                            addressLine2,
                            addressLine3,
                            addressLine4,
                            addressLine5,
                            inputNotificationInfo.Patient.PostCode,
                            personalisation,
                            string.Empty))
                        .ReturnsAsync(result);

                    break;
            }

            // when
            await this.notificationService.SendCodeNotificationAsync(notificationInfo: inputNotificationInfo);

            // then
            switch (notificationPreference)
            {
                case NotificationPreference.Email:
                    this.notificationBrokerMock.Verify(broker =>
                        broker.SendEmailAsync(
                            this.notificationConfig.EmailCodeTemplateId,
                            inputNotificationInfo.Patient.Email,
                            personalisation),
                        Times.Once);
                    break;

                case NotificationPreference.Sms:
                    this.notificationBrokerMock.Verify(broker =>
                        broker.SendSmsAsync(
                            this.notificationConfig.SmsCodeTemplateId,
                            inputNotificationInfo.Patient.Phone,
                            personalisation),
                        Times.Once);
                    break;

                case NotificationPreference.Letter:
                    this.notificationBrokerMock.Verify(broker =>
                        broker.SendLetterAsync(
                            notificationConfig.LetterCodeTemplateId,

                            $"{inputNotificationInfo.Patient.Title} " +
                                $"{inputNotificationInfo.Patient.GivenName} " +
                                    $"{inputNotificationInfo.Patient.Surname}",

                            addressLine1,
                            addressLine2,
                            addressLine3,
                            addressLine4,
                            addressLine5,
                            inputNotificationInfo.Patient.PostCode,

                            personalisation, string.Empty),
                        Times.Once);
                    break;
            }

            this.notificationBrokerMock.VerifyNoOtherCalls();
        }
    }
}
