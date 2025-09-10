// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Core.Models.Orchestrations.Decisions.Exceptions;
using Moq;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Orchestrations.Decisions
{
    public partial class DecisionOrchestrationServiceTests
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
                await decisionOrchestrationService.CheckIfIsAuthenticatedUserWithRequiredRoleAsync();

            //then
            actualResult.Should().Be(expectedResult);

            this.securityBrokerMock.Verify(broker =>
                broker.IsCurrentUserAuthenticatedAsync(),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.IsInRoleAsync(this.decisionConfigurations.DecisionWorkflowRoles.First()),
                    Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.auditBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
            this.patientServiceMock.VerifyNoOtherCalls();
            this.notificationServiceMock.VerifyNoOtherCalls();
            this.decisionServiceMock.VerifyNoOtherCalls();
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

            var expectedUnauthorizedDecisionOrchestrationServiceException =
                new UnauthorizedDecisionOrchestrationServiceException(
                    "The current user is not authorized to perform this operation.");

            // when
            ValueTask<bool> checkUserAction =
                decisionOrchestrationService.CheckIfIsAuthenticatedUserWithRequiredRoleAsync();

            UnauthorizedDecisionOrchestrationServiceException
                actualUnauthorizedDecisionOrchestrationServiceException =
                    await Assert.ThrowsAsync<UnauthorizedDecisionOrchestrationServiceException>(
                        testCode: checkUserAction.AsTask);

            //then
            actualUnauthorizedDecisionOrchestrationServiceException
                .Should().BeEquivalentTo(expectedUnauthorizedDecisionOrchestrationServiceException);

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
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.auditBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
            this.patientServiceMock.VerifyNoOtherCalls();
            this.notificationServiceMock.VerifyNoOtherCalls();
            this.decisionServiceMock.VerifyNoOtherCalls();
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

            var invalidCaptchaDecisionOrchestrationServiceException =
                new InvalidCaptchaDecisionOrchestrationServiceException(
                    "The provided captcha token is invalid.");

            // when
            ValueTask<bool> checkUserAction =
                decisionOrchestrationService.CheckIfIsAuthenticatedUserWithRequiredRoleAsync();

            InvalidCaptchaDecisionOrchestrationServiceException
                actualInvalidCaptchaDecisionOrchestrationServiceException =
                    await Assert.ThrowsAsync<InvalidCaptchaDecisionOrchestrationServiceException>(
                        testCode: checkUserAction.AsTask);

            //then
            actualInvalidCaptchaDecisionOrchestrationServiceException
                .Should().BeEquivalentTo(invalidCaptchaDecisionOrchestrationServiceException);

            this.securityBrokerMock.Verify(broker =>
                broker.IsCurrentUserAuthenticatedAsync(),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.ValidateCaptchaAsync(),
                    Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.auditBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
            this.patientServiceMock.VerifyNoOtherCalls();
            this.notificationServiceMock.VerifyNoOtherCalls();
            this.decisionServiceMock.VerifyNoOtherCalls();
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
                await decisionOrchestrationService.CheckIfIsAuthenticatedUserWithRequiredRoleAsync();

            //then
            actualResult.Should().Be(expectedResult);

            this.securityBrokerMock.Verify(broker =>
                broker.IsCurrentUserAuthenticatedAsync(),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.ValidateCaptchaAsync(),
                    Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.auditBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
            this.patientServiceMock.VerifyNoOtherCalls();
            this.notificationServiceMock.VerifyNoOtherCalls();
            this.decisionServiceMock.VerifyNoOtherCalls();
        }
    }
}
