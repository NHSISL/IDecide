// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Core.Models.Foundations.NhsLogins;
using LondonDataServices.IDecide.Core.Models.Foundations.NhsLogins.Exceptions;
using Moq;
using Xunit;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.NhsLogins
{
    public partial class NhsLoginServiceTests
    {
        [Fact]
        public async Task ShouldThrowNhsLoginServiceServiceExceptionOnNhsLoginAsyncIfAccessTokenIsNullAndLogItAsync()
        {
            // given
            string nullAccessToken = null;

            var nullNhsLoginException = new NullNhsLoginException(
                message: "Access token is required.");

            var failedNhsLoginServiceException = new FailedNhsLoginServiceException(
                message: "Failed NHS Login service error occurred, please contact support.",
                innerException: nullNhsLoginException,
                data: null);

            var expectedNhsLoginServiceServiceException = new NhsLoginServiceServiceException(
                message: "NHS Login service error occurred, please contact support.",
                innerException: failedNhsLoginServiceException);

            this.securityBrokerMock.Setup(broker =>
                broker.GetNhsLoginAccessTokenAsync())
                    .ReturnsAsync(nullAccessToken);

            // when
            ValueTask<NhsLoginUserInfo> nhsLoginTask =
                this.nhsLoginService.NhsLoginAsync();

            NhsLoginServiceServiceException actualException =
                await Assert.ThrowsAsync<NhsLoginServiceServiceException>(
                    nhsLoginTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedNhsLoginServiceServiceException);

            this.securityBrokerMock.Verify(broker =>
                broker.GetNhsLoginAccessTokenAsync(),
                Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedNhsLoginServiceServiceException))),
                Times.Once);

            this.securityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowNhsLoginServiceDependencyValidationExceptionOnNhsLoginAsyncIfUserInfoIsNullAndLogItAsync()
        {
            // given
            string validAccessToken = "valid-token";
            NhsLoginUserInfo nullUserInfo = null;

            var nhsLoginUserInfoException = new NhsLoginUserInfoException(
                message: "NHS Login userinfo endpoint did not return a successful response.");

            var expectedNhsLoginServiceDependencyValidationException =
                new NhsLoginServiceDependencyValidationException(
                    message: "NHS Login validation error occurred, please fix the errors and try again.",
                    innerException: nhsLoginUserInfoException);

            this.securityBrokerMock.Setup(broker =>
                broker.GetNhsLoginAccessTokenAsync())
                    .ReturnsAsync(validAccessToken);

            this.securityBrokerMock.Setup(broker =>
                broker.GetNhsLoginUserInfoAsync(validAccessToken))
                    .ReturnsAsync(nullUserInfo);

            // when
            ValueTask<NhsLoginUserInfo> nhsLoginTask =
                this.nhsLoginService.NhsLoginAsync();

            NhsLoginServiceDependencyValidationException actualException =
                await Assert.ThrowsAsync<NhsLoginServiceDependencyValidationException>(
                    nhsLoginTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(
                expectedNhsLoginServiceDependencyValidationException);

            this.securityBrokerMock.Verify(broker =>
                broker.GetNhsLoginAccessTokenAsync(),
                Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetNhsLoginUserInfoAsync(validAccessToken),
                Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedNhsLoginServiceDependencyValidationException))),
                Times.Once);

            this.securityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}