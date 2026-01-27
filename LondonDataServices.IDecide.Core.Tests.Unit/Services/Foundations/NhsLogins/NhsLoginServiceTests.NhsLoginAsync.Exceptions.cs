// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections;
using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Core.Models.Foundations.NhsLogins;
using LondonDataServices.IDecide.Core.Models.Foundations.NhsLogins.Exceptions;
using Moq;
using Xeptions;
using Xunit;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.NhsLogins
{
    public partial class NhsLoginServiceTests
    {
        [Theory]
        [MemberData(nameof(DependencyValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnNhsLoginAsyncAndLogItAsync(
            Exception dependencyValidationException)
        {
            // given
            var clientNhsLoginException = new ClientNhsLoginException(
                message: "NHS Login client error occurred, please fix the errors and try again.",
                innerException: dependencyValidationException,
                data: dependencyValidationException.Data);

            this.securityBrokerMock.Setup(broker =>
                broker.GetAccessTokenAsync())
                .ThrowsAsync(dependencyValidationException);

            var expectedDependencyValidationException =
                new NhsLoginServiceDependencyValidationException(
                    message: "NHS Login dependency validation error occurred, fix errors and try again.",
                    innerException: clientNhsLoginException as Xeption);

            // when
            ValueTask<NhsLoginUserInfo> nhsLoginTask =
                this.nhsLoginService.NhsLoginAsync();

            NhsLoginServiceDependencyValidationException actualException =
                await Assert.ThrowsAsync<NhsLoginServiceDependencyValidationException>(
                    nhsLoginTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedDependencyValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedDependencyValidationException))),
                Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetAccessTokenAsync(),
                Times.Once);

            this.securityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnNhsLoginAsyncAndLogItAsync(
            Exception dependencyException)
        {
            // given
            var serverNhsLoginException = new ServerNhsLoginException(
                message: "NHS Login userinfo endpoint did not return a successful response.",
                innerException: dependencyException,
                data: dependencyException.Data);

            this.securityBrokerMock.Setup(broker =>
                broker.GetAccessTokenAsync())
                    .ThrowsAsync(dependencyException);

            var expectedNhsLoginServiceDependencyException = new NhsLoginServiceDependencyException(
                message: "NHS Login dependency error occurred, please contact support.",
                innerException: serverNhsLoginException);

            // when
            ValueTask<NhsLoginUserInfo> nhsLoginTask =
                this.nhsLoginService.NhsLoginAsync();

            NhsLoginServiceDependencyException actualException =
                await Assert.ThrowsAsync<NhsLoginServiceDependencyException>(
                    nhsLoginTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedNhsLoginServiceDependencyException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedNhsLoginServiceDependencyException))),
                Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetAccessTokenAsync(),
                Times.Once);

            this.securityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnNhsLoginAsyncIfServiceErrorOccurredAndLogItAsync()
        {
            // given
            var serviceException = new Exception();

            var failedNhsLoginServiceException =
                new FailedNhsLoginServiceException(
                    message: "Failed NHS Login service error occurred, please contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedNhsLoginServiceServiceException =
                new NhsLoginServiceException(
                    message: "NHS Login service error occurred, please contact support.",
                    innerException: failedNhsLoginServiceException);

            this.securityBrokerMock.Setup(broker =>
                broker.GetAccessTokenAsync())
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<NhsLoginUserInfo> nhsLoginTask =
                this.nhsLoginService.NhsLoginAsync();

            NhsLoginServiceException actualNhsLoginServiceServiceException =
                await Assert.ThrowsAsync<NhsLoginServiceException>(
                    nhsLoginTask.AsTask);

            // then
            actualNhsLoginServiceServiceException.Should().BeEquivalentTo(
                expectedNhsLoginServiceServiceException);

            this.securityBrokerMock.Verify(broker =>
                broker.GetAccessTokenAsync(),
                Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedNhsLoginServiceServiceException))),
                Times.Once);

            this.securityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}