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
        [Fact]
        public async Task ShouldSendCodeNotificationAsync()
        {
            // given
            NotificationInfo randomNotificationInfo = CreateRandomNotificationInfo();
            NotificationInfo inputNotificationInfo = randomNotificationInfo;
            string subject = GetRandomString();
            string body = GetRandomString();

            Dictionary<string, dynamic> personalisation = new Dictionary<string, dynamic>
            {
                { "patientName", inputNotificationInfo.Patient.GivenName }
            };

            string result = GetRandomString();

            this.notificationBrokerMock.Setup(broker =>
                broker.SendEmailAsync(inputNotificationInfo.Patient.Email, subject, body, personalisation))
                    .ReturnsAsync(result);

            // when
            await this.notificationService.SendCodeNotificationAsync(notificationInfo: inputNotificationInfo);

            // then
            this.notificationBrokerMock.Verify(broker =>
                broker.SendEmailAsync(inputNotificationInfo.Patient.Email, subject, body, personalisation),
                    Times.Once);

            this.notificationBrokerMock.VerifyNoOtherCalls();
        }
    }
}
