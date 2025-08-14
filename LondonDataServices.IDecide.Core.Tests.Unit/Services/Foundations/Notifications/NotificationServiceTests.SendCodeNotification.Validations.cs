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
                    message: "Notification validation errors occurred, please try again.",
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

        [Fact]
        public async Task ShouldThrowValidationExceptionOnSendCodeNotificationIfNotificationInfoIsInvalidAndLogItAsync()
        {
            // given
            var invalidNotificationInfo = new NotificationInfo
            {
                Patient = null,
                Decision = null
            };

            var invalidNotificationInfoException =
                new InvalidNotificationInfoException(
                    message: "Invalid notification info. Please correct the errors and try again.");

            invalidNotificationInfoException.AddData(
                key: nameof(NotificationInfo.Patient),
                values: "Patient is required");

            invalidNotificationInfoException.AddData(
                key: nameof(NotificationInfo.Decision),
                values: "Decision is required");

            var expectedNotificationValidationException =
                new NotificationValidationException(
                    message: "Notification validation errors occurred, please try again.",
                    innerException: invalidNotificationInfoException);

            // when
            ValueTask sendCodeNotificationTask =
                this.notificationService.SendCodeNotificationAsync(invalidNotificationInfo);

            NotificationValidationException actualNotificationValidationException =
                await Assert.ThrowsAsync<NotificationValidationException>(
                    () => sendCodeNotificationTask.AsTask());

            // then
            actualNotificationValidationException.Should().BeEquivalentTo(expectedNotificationValidationException);
            this.notificationBrokerMock.VerifyNoOtherCalls();
        }
    }
}
