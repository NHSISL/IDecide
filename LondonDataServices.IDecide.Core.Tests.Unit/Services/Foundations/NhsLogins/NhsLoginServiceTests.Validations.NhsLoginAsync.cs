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
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("   ")]
        public async Task ShouldThrowInvalidArgumentsNhsLoginServiceExceptionOnNhsLoginAsyncIfArgumentInvalidAndLogItAsync(
            string invalidAccessToken)
        {
            // given
            string nullAccessToken = invalidAccessToken;

            var invalidArgumentsException = new InvalidArgumentsNhsLoginServiceException(
                message: "Invalid NHS Login argument. Please correct the errors and try again.");

            invalidArgumentsException.UpsertDataList(
                key: "accessToken",
                value: "Access token is required.");

            var expectedNhsLoginServiceDependencyValidationException =
                new NhsLoginServiceDependencyValidationException(
                    message: "NHS Login validation error occurred, please fix the errors and try again.",
                    innerException: invalidArgumentsException);

            this.securityBrokerMock.Setup(broker =>
                broker.GetAccessTokenAsync())
                    .ReturnsAsync(nullAccessToken);

            // when
            ValueTask<NhsLoginUserInfo> nhsLoginTask =
                this.nhsLoginService.NhsLoginAsync();

            NhsLoginServiceDependencyValidationException actualException =
                await Assert.ThrowsAsync<NhsLoginServiceDependencyValidationException>(
                    nhsLoginTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedNhsLoginServiceDependencyValidationException);

            this.securityBrokerMock.Verify(broker =>
                broker.GetAccessTokenAsync(),
                Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedNhsLoginServiceDependencyValidationException))),
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

            var nhsLoginUserInfoException = new NhsLoginNullResponseException(
                message: "NHS Login userinfo endpoint did not return a successful response.");

            var expectedNhsLoginServiceDependencyValidationException =
                new NhsLoginServiceDependencyValidationException(
                    message: "NHS Login validation error occurred, please fix the errors and try again.",
                    innerException: nhsLoginUserInfoException);

            this.securityBrokerMock.Setup(broker =>
                broker.GetAccessTokenAsync())
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
                broker.GetAccessTokenAsync(),
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