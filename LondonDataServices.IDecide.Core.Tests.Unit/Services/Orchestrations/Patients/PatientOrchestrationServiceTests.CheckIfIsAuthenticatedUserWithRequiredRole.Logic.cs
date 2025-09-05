// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Core.Models.Orchestrations.Patients.Exceptions;
using Moq;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Orchestrations.Patients
{
    public partial class PatientOrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldCheckIfIsAuthenticatedUserWithRequiredRoleAsyncWithAuthenticatedUserInRole()
        {
            // given
            bool expectedResult = true;

            this.securityBrokerMock.Setup(broker =>
                broker.IsCurrentUserAuthenticatedAsync())
                    .ReturnsAsync(true);

            this.securityBrokerMock.Setup(broker =>
                broker.IsInRoleAsync(this.decisionConfigurations.DecisionWorkflowRoles.First()))
                    .ReturnsAsync(true);

            // when
            bool actualResult =
                await patientOrchestrationService.CheckIfIsAuthenticatedUserWithRequiredRoleAsync();

            //then
            actualResult.Should().Be(expectedResult);

            this.securityBrokerMock.Verify(broker =>
                broker.IsCurrentUserAuthenticatedAsync(),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.IsInRoleAsync(this.decisionConfigurations.DecisionWorkflowRoles.First()),
                    Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.auditBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
            this.pdsServiceMock.VerifyNoOtherCalls();
            this.patientServiceMock.VerifyNoOtherCalls();
            this.notificationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldErrorOnCheckIfIsAuthenticatedUserWithRequiredRoleAsyncWithAuthenticatedUserNotInRole()
        {
            // given
            this.securityBrokerMock.Setup(broker =>
                broker.IsCurrentUserAuthenticatedAsync())
                    .ReturnsAsync(true);

            foreach (string role in this.decisionConfigurations.DecisionWorkflowRoles)
            {
                this.securityBrokerMock.Setup(broker =>
                    broker.IsInRoleAsync(role))
                        .ReturnsAsync(false);
            }

            var expectedUnauthorizedPatientOrchestrationServiceException =
                new UnauthorizedPatientOrchestrationServiceException(
                    "The current user is not authorized to perform this operation.");

            // when
            ValueTask<bool> checkUserAction =
                patientOrchestrationService.CheckIfIsAuthenticatedUserWithRequiredRoleAsync();

            UnauthorizedPatientOrchestrationServiceException
                actualUnauthorizedPatientOrchestrationServiceException =
                    await Assert.ThrowsAsync<UnauthorizedPatientOrchestrationServiceException>(
                        testCode: checkUserAction.AsTask);

            //then
            actualUnauthorizedPatientOrchestrationServiceException
                .Should().BeEquivalentTo(expectedUnauthorizedPatientOrchestrationServiceException);

            this.securityBrokerMock.Verify(broker =>
                broker.IsCurrentUserAuthenticatedAsync(),
                    Times.Once);

            foreach (string role in this.decisionConfigurations.DecisionWorkflowRoles)
            {
                this.securityBrokerMock.Verify(broker =>
                    broker.IsInRoleAsync(role),
                        Times.Once);
            }

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.auditBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
            this.pdsServiceMock.VerifyNoOtherCalls();
            this.patientServiceMock.VerifyNoOtherCalls();
            this.notificationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task
            ShouldErrorOnCheckIfIsAuthenticatedUserWithRequiredRoleAsyncWithNonAuthenticatedUserInvalidCaptcha()
        {
            // given
            this.securityBrokerMock.Setup(broker =>
                broker.IsCurrentUserAuthenticatedAsync())
                    .ReturnsAsync(false);

            this.securityBrokerMock.Setup(broker =>
                 broker.ValidateCaptchaAsync())
                     .ReturnsAsync(false);

            var invalidCaptchaPatientOrchestrationServiceException =
                new InvalidCaptchaPatientOrchestrationServiceException(
                    "The provided captcha token is invalid.");

            // when
            ValueTask<bool> checkUserAction =
                patientOrchestrationService.CheckIfIsAuthenticatedUserWithRequiredRoleAsync();

            InvalidCaptchaPatientOrchestrationServiceException
                actualInvalidCaptchaPatientOrchestrationServiceException =
                    await Assert.ThrowsAsync<InvalidCaptchaPatientOrchestrationServiceException>(
                        testCode: checkUserAction.AsTask);

            //then
            actualInvalidCaptchaPatientOrchestrationServiceException
                .Should().BeEquivalentTo(invalidCaptchaPatientOrchestrationServiceException);

            this.securityBrokerMock.Verify(broker =>
                broker.IsCurrentUserAuthenticatedAsync(),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.ValidateCaptchaAsync(),
                    Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.auditBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
            this.pdsServiceMock.VerifyNoOtherCalls();
            this.patientServiceMock.VerifyNoOtherCalls();
            this.notificationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldCheckIfIsAuthenticatedUserWithRequiredRoleAsyncWithNonAuthenticatedUserValidCaptcha()
        {
            // given
            bool expectedResult = false;

            this.securityBrokerMock.Setup(broker =>
                broker.IsCurrentUserAuthenticatedAsync())
                    .ReturnsAsync(false);

            this.securityBrokerMock.Setup(broker =>
                 broker.ValidateCaptchaAsync())
                     .ReturnsAsync(true);

            // when
            bool actualResult =
                await patientOrchestrationService.CheckIfIsAuthenticatedUserWithRequiredRoleAsync();

            //then
            actualResult.Should().Be(expectedResult);

            this.securityBrokerMock.Verify(broker =>
                broker.IsCurrentUserAuthenticatedAsync(),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.ValidateCaptchaAsync(),
                    Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.auditBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
            this.pdsServiceMock.VerifyNoOtherCalls();
            this.patientServiceMock.VerifyNoOtherCalls();
            this.notificationServiceMock.VerifyNoOtherCalls();
        }
    }
}
