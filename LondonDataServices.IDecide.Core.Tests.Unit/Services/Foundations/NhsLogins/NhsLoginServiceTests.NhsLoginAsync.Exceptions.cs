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
using Xunit;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.NhsLogins
{
    public partial class NhsLoginServiceTests
    {
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
                new NhsLoginServiceServiceException(
                    message: "NHS Login service error occurred, please contact support.",
                    innerException: failedNhsLoginServiceException);

            this.securityBrokerMock.Setup(broker =>
                broker.GetNhsLoginAccessTokenAsync())
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<NhsLoginUserInfo> nhsLoginTask =
                this.nhsLoginService.NhsLoginAsync();

            NhsLoginServiceServiceException actualNhsLoginServiceServiceException =
                await Assert.ThrowsAsync<NhsLoginServiceServiceException>(
                    nhsLoginTask.AsTask);

            // then
            actualNhsLoginServiceServiceException.Should().BeEquivalentTo(
                expectedNhsLoginServiceServiceException);

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
    }
}