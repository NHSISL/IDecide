// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Core.Models.Foundations.Notifications;
using LondonDataServices.IDecide.Core.Models.Foundations.Notifications.Exceptions;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.Notifications
{
    public partial class NotificationServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnSendCodeNotificationWhenNotificationInfoIsNullAndLogItAsync()
        {
            // given
            NotificationInfo nullNotificationInfo = null;

            var nullNotificationInfoException =
                new NullNotificationInfoException(message: "Notification info is null.");

            var expectedNotificationValidationException =
                new NotificationValidationException(
                    message: "Notification info validation errors occurred, please try again.",
                    innerException: nullNotificationInfoException);

            // when
            ValueTask sendCodeNotificationTask =
                this.notificationService.SendCodeNotificationAsync(nullNotificationInfo);

            NotificationValidationException actualNotificationValidationException =
                await Assert.ThrowsAsync<NotificationValidationException>(
                    () => sendCodeNotificationTask.AsTask());

            // then
            actualNotificationValidationException.Should().BeEquivalentTo(expectedNotificationValidationException);
            this.notificationBrokerMock.VerifyNoOtherCalls();
        }
    }
}
