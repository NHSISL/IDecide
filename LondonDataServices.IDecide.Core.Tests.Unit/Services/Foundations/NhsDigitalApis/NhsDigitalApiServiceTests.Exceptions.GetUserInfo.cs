// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Core.Models.Foundations.NhsDigitalApis.Exceptions;
using Moq;
using Task = System.Threading.Tasks.Task;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.NhsDigitalApis
{
    public partial class NhsDigitalApiServiceTests
    {
        [Fact]
        public async Task
            ShouldThrowDependencyExceptionOnGetUserInfoIfServerErrorOccursAndLogItAsync()
        {
            // given
            CancellationToken inputCancellationToken = GetCancellationToken();
            string inputCode = GetRandomString();
            string inputState = GetRandomString();

            var httpRequestException =
                new HttpRequestException(
                    message: GetRandomString(),
                    inner: null,
                    statusCode: HttpStatusCode.InternalServerError);

            var serverNhsDigitalApiException =
                new ServerNhsDigitalApiException(
                    message: "NhsDigitalApi server error occurred, please contact support.",
                    innerException: httpRequestException,
                    data: httpRequestException.Data);

            var expectedNhsDigitalApiDependencyException =
                new NhsDigitalApiDependencyException(
                    message: "NhsDigitalApi dependency error occurred, please contact support.",
                    innerException: serverNhsDigitalApiException);

            this.nhsDigitalApiBrokerMock.Setup(broker =>
                broker.GetUserInfoAsync(inputCode, inputState, inputCancellationToken))
                    .ThrowsAsync(httpRequestException);

            // when
            ValueTask<string> getUserInfoTask =
                this.nhsDigitalApiService.GetUserInfoAsync(
                    inputCode,
                    inputState,
                    inputCancellationToken);

            NhsDigitalApiDependencyException actualException =
                await Assert.ThrowsAsync<NhsDigitalApiDependencyException>(
                    testCode: getUserInfoTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedNhsDigitalApiDependencyException);

            this.nhsDigitalApiBrokerMock.Verify(broker =>
                broker.GetUserInfoAsync(inputCode, inputState, inputCancellationToken),
                Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedNhsDigitalApiDependencyException))),
                Times.Once);

            this.nhsDigitalApiBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
