﻿// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using LondonDataServices.IDecide.Core.Models.Foundations.Notifications;
using LondonDataServices.IDecide.Core.Models.Orchestrations.Patients.Exceptions;
using Moq;
using Xeptions;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Orchestrations.Patients
{
    public partial class PatientOrchestrationServiceTests
    {
        [Theory]
        [MemberData(nameof(RecordPatientInformationDependencyValidationExceptions))]
        public async Task ShouldThrowDependencyValidationOnRecordPatientInformationAndLogItAsync(
            Xeption dependencyValidationException)
        {
            // given
            string randomNhsNumber = GenerateRandom10DigitNumber();
            string inputNhsNumber = randomNhsNumber.DeepClone();
            string randomCaptchaToken = GetRandomString();
            string inputCaptchaToken = randomCaptchaToken.DeepClone();
            NotificationPreference randomNotificationPreference = NotificationPreference.Email;
            NotificationPreference inputNotificationPreference = randomNotificationPreference.DeepClone();
            string notificationPreferenceString = inputNotificationPreference.ToString();

            var expectedPatientOrchestrationDependencyValidationException =
                new PatientOrchestrationDependencyValidationException(
                    message: "Patient orchestration dependency validation error occurred, " +
                        "please fix the errors and try again.",
                    innerException: dependencyValidationException.InnerException as Xeption);

            this.securityBrokerMock.Setup(broker =>
                broker.ValidateCaptchaAsync(inputCaptchaToken, ""))
                    .ThrowsAsync(dependencyValidationException);

            // when
            ValueTask recordPatientInformationTask =
                 this.patientOrchestrationService.RecordPatientInformation(
                    inputNhsNumber,
                    inputCaptchaToken,
                    notificationPreferenceString,
                    false);

            PatientOrchestrationDependencyValidationException
                actualPatientOrchestrationDependencyValidationException =
                    await Assert.ThrowsAsync<PatientOrchestrationDependencyValidationException>(
                        testCode: recordPatientInformationTask.AsTask);

            // then
            actualPatientOrchestrationDependencyValidationException
                .Should().BeEquivalentTo(expectedPatientOrchestrationDependencyValidationException);

            this.securityBrokerMock.Verify(broker =>
                broker.ValidateCaptchaAsync(inputCaptchaToken, ""),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedPatientOrchestrationDependencyValidationException))),
                       Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.pdsServiceMock.VerifyNoOtherCalls();
            this.patientServiceMock.VerifyNoOtherCalls();
            this.notificationServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(RecordPatientInformationDependencyExceptions))]
        public async Task ShouldThrowDependencyOnRecordPatientInformationAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            string randomNhsNumber = GenerateRandom10DigitNumber();
            string inputNhsNumber = randomNhsNumber.DeepClone();
            string randomCaptchaToken = GetRandomString();
            string inputCaptchaToken = randomCaptchaToken.DeepClone();
            NotificationPreference randomNotificationPreference = NotificationPreference.Email;
            NotificationPreference inputNotificationPreference = randomNotificationPreference.DeepClone();
            string notificationPreferenceString = inputNotificationPreference.ToString();

            var expectedPatientOrchestrationDependencyException =
                 new PatientOrchestrationDependencyException(
                    message: "Patient orchestration dependency error occurred, " +
                        "please fix the errors and try again.",
                    innerException: dependencyException.InnerException as Xeption);

            this.securityBrokerMock.Setup(broker =>
                broker.ValidateCaptchaAsync(inputCaptchaToken, ""))
                    .ThrowsAsync(dependencyException);

            // when
            ValueTask recordPatientInformationTask =
                 this.patientOrchestrationService.RecordPatientInformation(
                    inputNhsNumber,
                    inputCaptchaToken,
                    notificationPreferenceString,
                    false);

            PatientOrchestrationDependencyException
                actualPatientOrchestrationDependencyException =
                    await Assert.ThrowsAsync<PatientOrchestrationDependencyException>(
                        testCode: recordPatientInformationTask.AsTask);

            // then
            actualPatientOrchestrationDependencyException
                .Should().BeEquivalentTo(expectedPatientOrchestrationDependencyException);

            this.securityBrokerMock.Verify(broker =>
                broker.ValidateCaptchaAsync(inputCaptchaToken, ""),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedPatientOrchestrationDependencyException))),
                       Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.pdsServiceMock.VerifyNoOtherCalls();
            this.patientServiceMock.VerifyNoOtherCalls();
            this.notificationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRecordPatientInformationIfServiceErrorOccurredAndLogItAsync()
        {
            // given
            string randomNhsNumber = GenerateRandom10DigitNumber();
            string inputNhsNumber = randomNhsNumber.DeepClone();
            string randomCaptchaToken = GetRandomString();
            string inputCaptchaToken = randomCaptchaToken.DeepClone();
            NotificationPreference randomNotificationPreference = NotificationPreference.Email;
            NotificationPreference inputNotificationPreference = randomNotificationPreference.DeepClone();
            string notificationPreferenceString = inputNotificationPreference.ToString();
            var serviceException = new Exception();

            var failedServicePatientOrchestrationException =
                new FailedPatientOrchestrationServiceException(
                    message: "Failed patient orchestration service error occurred, contact support.",
                    innerException: serviceException);

            var expectedPatientOrchestrationServiceException =
                new PatientOrchestrationServiceException(
                    message: "Patient orchestration service error occurred, contact support.",
                    innerException: failedServicePatientOrchestrationException);

            this.securityBrokerMock.Setup(broker =>
                broker.ValidateCaptchaAsync(inputCaptchaToken, ""))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask recordPatientInformationTask =
                 this.patientOrchestrationService.RecordPatientInformation(
                    inputNhsNumber,
                    inputCaptchaToken,
                    notificationPreferenceString,
                    false);

            PatientOrchestrationServiceException
                actualPatientOrchestrationValidationException =
                    await Assert.ThrowsAsync<PatientOrchestrationServiceException>(
                        testCode: recordPatientInformationTask.AsTask);

            // then
            actualPatientOrchestrationValidationException.Should().BeEquivalentTo(
                expectedPatientOrchestrationServiceException);

            this.securityBrokerMock.Verify(broker =>
                broker.ValidateCaptchaAsync(inputCaptchaToken, ""),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedPatientOrchestrationServiceException))),
                       Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.pdsServiceMock.VerifyNoOtherCalls();
            this.patientServiceMock.VerifyNoOtherCalls();
            this.notificationServiceMock.VerifyNoOtherCalls();
        }
    }
}
