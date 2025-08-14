// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Core.Models.Foundations.Decisions;
using LondonDataServices.IDecide.Core.Models.Foundations.DecisionTypes;
using LondonDataServices.IDecide.Core.Models.Foundations.Notifications;
using LondonDataServices.IDecide.Core.Models.Foundations.Notifications.Exceptions;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using Moq;

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

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedNotificationValidationException))),
                        Times.Once);

            this.notificationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnSendCodeNotificationIfNotificationInfoIsInvalidAndLogItAsync(
            string invalidText)
        {
            // given
            var invalidNotificationInfo = new NotificationInfo
            {
                Patient = new Patient
                {
                    NhsNumber = invalidText,
                    Title = invalidText,
                    GivenName = invalidText,
                    Surname = invalidText,
                    Gender = invalidText,
                    Email = invalidText,
                    Phone = invalidText,
                    Address = invalidText,
                    PostCode = invalidText,
                    ValidationCode = invalidText,
                },
                Decision = new Decision
                {
                    DecisionChoice = invalidText,
                    ResponsiblePersonGivenName = invalidText,
                    ResponiblePersonSurname = invalidText,
                    ResponsiblePersonRelationship = invalidText,
                    DecisionType = new DecisionType
                    {
                        Name = invalidText
                    }
                }
            };

            var invalidNotificationInfoException =
                new InvalidNotificationInfoException(
                    message: "Invalid notification info. Please correct the errors and try again.");

            invalidNotificationInfoException.AddData(
                key: nameof(NotificationInfo.Patient.NhsNumber),
                values: "Text is required");

            invalidNotificationInfoException.AddData(
                key: nameof(NotificationInfo.Patient.Title),
                values: "Text is required");

            invalidNotificationInfoException.AddData(
                key: nameof(NotificationInfo.Patient.GivenName),
                values: "Text is required");

            invalidNotificationInfoException.AddData(
                key: nameof(NotificationInfo.Patient.Surname),
                values: "Text is required");

            invalidNotificationInfoException.AddData(
                key: nameof(NotificationInfo.Patient.DateOfBirth),
                values: "Date is required");

            invalidNotificationInfoException.AddData(
                key: nameof(NotificationInfo.Patient.Gender),
                values: "Text is required");

            invalidNotificationInfoException.AddData(
                key: nameof(NotificationInfo.Patient.Email),
                values: "Text is required");

            invalidNotificationInfoException.AddData(
                key: nameof(NotificationInfo.Patient.Phone),
                values: "Text is required");

            invalidNotificationInfoException.AddData(
                key: nameof(NotificationInfo.Patient.Address),
                values: "Text is required");

            invalidNotificationInfoException.AddData(
                key: nameof(NotificationInfo.Patient.PostCode),
                values: "Text is required");

            invalidNotificationInfoException.AddData(
                key: nameof(NotificationInfo.Patient.ValidationCode),
                values: "Text is required");

            invalidNotificationInfoException.AddData(
                key: nameof(NotificationInfo.Patient.ValidationCodeExpiresOn),
                values: "Date is required");

            invalidNotificationInfoException.AddData(
                key: nameof(NotificationInfo.Patient.NotificationPreference),
                values: "Value is required");

            invalidNotificationInfoException.AddData(
                key: nameof(NotificationInfo.Decision.DecisionChoice),
                values: "Text is required");

            invalidNotificationInfoException.AddData(
                key: nameof(NotificationInfo.Decision.DecisionType.Name),
                values: "Text is required");

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

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedNotificationValidationException))),
                        Times.Once);

            this.notificationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
