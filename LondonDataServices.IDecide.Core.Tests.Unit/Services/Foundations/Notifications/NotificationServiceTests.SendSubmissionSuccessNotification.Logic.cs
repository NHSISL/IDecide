﻿// ---------------------------------------------------------
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
            Dictionary<string, dynamic> personalisation = GetPersonalisation(inputNotificationInfo);

            string result = GetRandomString();

            switch (notificationPreference)
            {
                case NotificationPreference.Email:
                    this.notificationConfig.EmailSubmissionSuccessTemplateId = GetRandomString();

                    this.notificationBrokerMock.Setup(broker =>
                        broker.SendEmailAsync(
                            inputNotificationInfo.Patient.Email,
                            this.notificationConfig.EmailSubmissionSuccessTemplateId,
                            personalisation))
                        .ReturnsAsync(result);

                    break;

                case NotificationPreference.Sms:

                    this.notificationConfig.SmsSubmissionSuccessTemplateId = GetRandomString();

                    this.notificationBrokerMock.Setup(broker =>
                        broker.SendSmsAsync(
                            this.notificationConfig.SmsSubmissionSuccessTemplateId, personalisation))
                        .ReturnsAsync(result);

                    break;

                case NotificationPreference.Letter:

                    this.notificationConfig.LetterSubmissionSuccessTemplateId = GetRandomString();

                    this.notificationBrokerMock.Setup(broker =>
                        broker.SendLetterAsync(
                            this.notificationConfig.LetterSubmissionSuccessTemplateId,
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
                                inputNotificationInfo.Patient.Email,
                                this.notificationConfig.EmailSubmissionSuccessTemplateId,
                                personalisation),
                        Times.Once);
                    break;

                case NotificationPreference.Sms:
                    this.notificationBrokerMock.Verify(broker =>
                            broker.SendSmsAsync(
                                this.notificationConfig.SmsSubmissionSuccessTemplateId, personalisation),
                        Times.Once);
                    break;

                case NotificationPreference.Letter:
                    this.notificationBrokerMock.Verify(broker =>
                            broker.SendLetterAsync(
                                notificationConfig.LetterSubmissionSuccessTemplateId,
                                personalisation,
                                string.Empty),
                        Times.Once);
                    break;
            }

            this.notificationBrokerMock.VerifyNoOtherCalls();
        }
    }
}
