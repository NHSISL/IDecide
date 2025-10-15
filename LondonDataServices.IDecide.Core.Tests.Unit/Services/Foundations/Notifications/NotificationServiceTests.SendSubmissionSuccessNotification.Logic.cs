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
        public async Task ShouldSendSubmissionSuccessNotificationAsync(NotificationPreference notificationPreference)
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
                    this.notificationConfig.EmailSubmissionSuccessTemplateId = GetRandomString();

                    this.notificationBrokerMock.Setup(broker =>
                        broker.SendEmailAsync(
                            this.notificationConfig.EmailSubmissionSuccessTemplateId,
                            inputNotificationInfo.Patient.Email,
                            personalisation))
                        .ReturnsAsync(result);

                    break;

                case NotificationPreference.Sms:

                    this.notificationConfig.SmsSubmissionSuccessTemplateId = GetRandomString();

                    this.notificationBrokerMock.Setup(broker =>
                        broker.SendSmsAsync(
                            this.notificationConfig.SmsSubmissionSuccessTemplateId,
                            inputNotificationInfo.Patient.Phone,
                            personalisation))
                        .ReturnsAsync(result);

                    break;

                case NotificationPreference.Letter:

                    this.notificationConfig.LetterSubmissionSuccessTemplateId = GetRandomString();

                    this.notificationBrokerMock.Setup(broker =>
                        broker.SendLetterAsync(
                            this.notificationConfig.LetterSubmissionSuccessTemplateId,
                            inputNotificationInfo.Patient.PostalAddress.RecipientName,
                            inputNotificationInfo.Patient.PostalAddress.AddressLine1,
                            inputNotificationInfo.Patient.PostalAddress.AddressLine2,
                            inputNotificationInfo.Patient.PostalAddress.AddressLine3,
                            inputNotificationInfo.Patient.PostalAddress.AddressLine4,
                            inputNotificationInfo.Patient.PostalAddress.AddressLine5,
                            inputNotificationInfo.Patient.PostCode,
                            personalisation,
                            string.Empty))
                        .ReturnsAsync(result);

                    break;
            }
            // when
            await this.notificationService.SendSubmissionSuccessNotificationAsync(inputNotificationInfo);

            // then
            switch (notificationPreference)
            {
                case NotificationPreference.Email:
                    this.notificationBrokerMock.Verify(broker =>
                            broker.SendEmailAsync(
                                this.notificationConfig.EmailSubmissionSuccessTemplateId,
                                inputNotificationInfo.Patient.Email,
                                personalisation),
                        Times.Once);
                    break;

                case NotificationPreference.Sms:
                    this.notificationBrokerMock.Verify(broker =>
                            broker.SendSmsAsync(
                                this.notificationConfig.SmsSubmissionSuccessTemplateId,
                                inputNotificationInfo.Patient.Phone,
                                personalisation),
                        Times.Once);
                    break;

                case NotificationPreference.Letter:
                    this.notificationBrokerMock.Verify(broker =>
                            broker.SendLetterAsync(
                                notificationConfig.LetterSubmissionSuccessTemplateId,
                                inputNotificationInfo.Patient.PostalAddress.RecipientName,
                                inputNotificationInfo.Patient.PostalAddress.AddressLine1,
                                inputNotificationInfo.Patient.PostalAddress.AddressLine2,
                                inputNotificationInfo.Patient.PostalAddress.AddressLine3,
                                inputNotificationInfo.Patient.PostalAddress.AddressLine4,
                                inputNotificationInfo.Patient.PostalAddress.AddressLine5,
                                inputNotificationInfo.Patient.PostCode,
                                personalisation,
                                string.Empty),
                        Times.Once);
                    break;
            }

            this.notificationBrokerMock.VerifyNoOtherCalls();
        }
    }
}
