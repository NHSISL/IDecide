// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.Providers.Notifications.Abstractions.Models.Exceptions;
using LondonDataServices.IDecide.Core.Models.Foundations.Notifications;
using LondonDataServices.IDecide.Core.Models.Foundations.Notifications.Exceptions;
using Moq;
using Xeptions;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.Notifications
{
    public partial class NotificationServiceTests
    {
        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnSendCodeNotificationAndLogItAsync()
        {
            // given
            NotificationInfo randomNotificationInfo = CreateRandomNotificationInfo();
            randomNotificationInfo.Patient.NotificationPreference = NotificationPreference.Email;
            NotificationInfo inputNotificationInfo = randomNotificationInfo;

            NotificationProviderValidationException notificationProviderValidationException =
                GetNotificationProviderValidationException();

            var clientNotificationException = new ClientNotificationException(
                message: "Client notification error occurred, contact support.",
                innerException: notificationProviderValidationException,
                data: notificationProviderValidationException.Data);

            var expectedNotificationDependencyValidationException = new NotificationDependencyValidationException(
                message: "Notification dependency validation error occurred, fix errors and try again.",
                innerException: clientNotificationException);

            this.notificationBrokerMock.Setup(broker =>
                broker.SendEmailAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, dynamic>>()))
                .ThrowsAsync(notificationProviderValidationException);

            // when
            ValueTask sendCodeNotificationTask =
                this.notificationService.SendCodeNotificationAsync(inputNotificationInfo);

            NotificationDependencyValidationException actualException =
                await Assert.ThrowsAsync<NotificationDependencyValidationException>(
                    sendCodeNotificationTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedNotificationDependencyValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedNotificationDependencyValidationException))),
                Times.Once);

            this.notificationBrokerMock.Verify(broker =>
                broker.SendEmailAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, dynamic>>()),
                Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.notificationBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnSendCodeNotificationAndLogItAsync(Xeption dependencyException)
        {
            // given
            NotificationInfo randomNotificationInfo = CreateRandomNotificationInfo();
            randomNotificationInfo.Patient.NotificationPreference = NotificationPreference.Email;
            NotificationInfo inputNotificationInfo = randomNotificationInfo;

            var serverNotificationException = new ServerNotificationException(
                message: "Server notification error occurred, contact support.",
                innerException: dependencyException,
                data: dependencyException.Data);

            var expectedNotificationDependencyException = new NotificationDependencyException(
                message: "Notification dependency error occurred, contact support.",
                innerException: serverNotificationException);

            this.notificationBrokerMock.Setup(broker =>
                broker.SendEmailAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, dynamic>>()))
                .ThrowsAsync(dependencyException);

            // when
            ValueTask sendCodeNotificationTask =
                this.notificationService.SendCodeNotificationAsync(inputNotificationInfo);

            NotificationDependencyException actualException =
                await Assert.ThrowsAsync<NotificationDependencyException>(
                    sendCodeNotificationTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedNotificationDependencyException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedNotificationDependencyException))),
                Times.Once);

            this.notificationBrokerMock.Verify(broker =>
                broker.SendEmailAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, dynamic>>()),
                Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.notificationBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnSendCodeNotificationAndLogItAsync()
        {
            // given
            NotificationInfo randomNotificationInfo = CreateRandomNotificationInfo();
            randomNotificationInfo.Patient.NotificationPreference = NotificationPreference.Email;
            NotificationInfo inputNotificationInfo = randomNotificationInfo;

            Exception exception = new Exception();

            var failedServiceNotificationException =
                new FailedServiceNotificationException(
                    message: "Failed service notification error occurred, contact support.",
                    innerException: exception,
                    data: exception.Data);

            var expectedNotificationServiceException = new NotificationServiceException(
                message: "Notification service error occurred, contact support.",
                innerException: failedServiceNotificationException);

            this.notificationBrokerMock.Setup(broker =>
                broker.SendEmailAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, dynamic>>()))
                .ThrowsAsync(exception);

            // when
            ValueTask sendCodeNotificationTask =
                this.notificationService.SendCodeNotificationAsync(inputNotificationInfo);

            NotificationServiceException actualException =
                await Assert.ThrowsAsync<NotificationServiceException>(
                    sendCodeNotificationTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedNotificationServiceException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedNotificationServiceException))),
                Times.Once);

            this.notificationBrokerMock.Verify(broker =>
                broker.SendEmailAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, dynamic>>()),
                Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.notificationBrokerMock.VerifyNoOtherCalls();
        }
    }
}
