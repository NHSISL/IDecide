﻿// ---------------------------------------------------------
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
        public async Task
            ShouldThrowValidationExceptionOnSendSubmissionSuccessNotificationWhenNotificationInfoIsNullAndLogItAsync()
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
                this.notificationService.SendSubmissionSuccessNotificationAsync(nullNotificationInfo);

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
        public async Task
            ShouldThrowValidationExceptionOnSendSubmissionSuccessNotificationIfNotificationInfoIsInvalidAndLogItAsync(
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
                    NotificationPreference = (NotificationPreference)999, // Invalid enum value
                },
                Decision = new Decision
                {
                    DecisionChoice = invalidText,
                    ResponsiblePersonGivenName = invalidText,
                    ResponsiblePersonSurname = invalidText,
                    ResponsiblePersonRelationship = invalidText,
                    DecisionType = new DecisionType
                    {
                        Name = invalidText
                    }
                }
            };

            var invalidArgumentsNotificationException =
                new InvalidArgumentsNotificationException(
                    message: "Invalid notification arguments. Please correct the errors and try again.");

            invalidArgumentsNotificationException.AddData(
                key: nameof(NotificationInfo.Patient.NhsNumber),
                values: "Text is required");

            invalidArgumentsNotificationException.AddData(
                key: nameof(NotificationInfo.Patient.Title),
                values: "Text is required");

            invalidArgumentsNotificationException.AddData(
                key: nameof(NotificationInfo.Patient.GivenName),
                values: "Text is required");

            invalidArgumentsNotificationException.AddData(
                key: nameof(NotificationInfo.Patient.Surname),
                values: "Text is required");

            invalidArgumentsNotificationException.AddData(
                key: nameof(NotificationInfo.Patient.DateOfBirth),
                values: "Date is required");

            invalidArgumentsNotificationException.AddData(
                key: nameof(NotificationInfo.Patient.Gender),
                values: "Text is required");

            invalidArgumentsNotificationException.AddData(
                key: nameof(NotificationInfo.Patient.Email),
                values: "Text is required");

            invalidArgumentsNotificationException.AddData(
                key: nameof(NotificationInfo.Patient.Phone),
                values: "Text is required");

            invalidArgumentsNotificationException.AddData(
                key: nameof(NotificationInfo.Patient.Address),
                values: "Text is required");

            invalidArgumentsNotificationException.AddData(
                key: nameof(NotificationInfo.Patient.PostCode),
                values: "Text is required");

            invalidArgumentsNotificationException.AddData(
                key: nameof(NotificationInfo.Patient.ValidationCode),
                values: "Text is required");

            invalidArgumentsNotificationException.AddData(
                key: nameof(NotificationInfo.Patient.ValidationCodeExpiresOn),
                values: "Date is required");

            invalidArgumentsNotificationException.AddData(
                key: nameof(NotificationInfo.Patient.NotificationPreference),
                values: "Value is required");

            invalidArgumentsNotificationException.AddData(
                key: nameof(NotificationInfo.Decision.DecisionChoice),
                values: "Text is required");

            invalidArgumentsNotificationException.AddData(
                key: nameof(NotificationInfo.Decision.DecisionType.Name),
                values: "Text is required");

            var expectedNotificationValidationException =
                new NotificationValidationException(
                    message: "Notification validation errors occurred, please try again.",
                    innerException: invalidArgumentsNotificationException);

            // when
            ValueTask sendCodeNotificationTask =
                this.notificationService.SendSubmissionSuccessNotificationAsync(invalidNotificationInfo);

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
        public async Task
            ShouldThrowValidationExceptionOnSendSubmissionSuccessNotificationIfSendEmailInputIsInvalidAndLogItAsync(
                string invalidText)
        {
            // given
            NotificationInfo randomNotificationInfo = CreateRandomNotificationInfo();
            randomNotificationInfo.Patient.NotificationPreference = NotificationPreference.Email;
            NotificationInfo inputNotificationInfo = randomNotificationInfo;
            this.notificationConfig.EmailSubmissionSuccessTemplateId = invalidText;

            var invalidArgumentsNotificationException =
                new InvalidArgumentsNotificationException(
                    message: "Invalid notification arguments. Please correct the errors and try again.");

            invalidArgumentsNotificationException.AddData(
                key: nameof(NotificationConfig.EmailCodeTemplateId),
                values: "Text is required");

            var expectedNotificationValidationException =
                new NotificationValidationException(
                    message: "Notification validation errors occurred, please try again.",
                    innerException: invalidArgumentsNotificationException);

            // when
            ValueTask sendCodeNotificationTask =
                this.notificationService.SendSubmissionSuccessNotificationAsync(inputNotificationInfo);

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
