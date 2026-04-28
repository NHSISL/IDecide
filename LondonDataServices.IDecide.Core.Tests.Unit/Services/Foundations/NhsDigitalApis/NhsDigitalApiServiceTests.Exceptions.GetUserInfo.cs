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

        [Fact]
        public async Task
            ShouldThrowDependencyValidationExceptionOnGetUserInfoIfClientErrorOccursAndLogItAsync()
        {
            // given
            CancellationToken inputCancellationToken = GetCancellationToken();
            string inputCode = GetRandomString();
            string inputState = GetRandomString();

            var httpRequestException =
                new HttpRequestException(
                    message: GetRandomString(),
                    inner: null,
                    statusCode: HttpStatusCode.BadRequest);

            var clientNhsDigitalApiException =
                new ClientNhsDigitalApiException(
                    message: "NhsDigitalApi client error occurred, please fix the errors and try again.",
                    innerException: httpRequestException,
                    data: httpRequestException.Data);

            var expectedNhsDigitalApiDependencyValidationException =
                new NhsDigitalApiDependencyValidationException(
                    message: "NhsDigitalApi dependency validation error occurred, " +
                        "please fix the errors and try again.",
                    innerException: clientNhsDigitalApiException);

            this.nhsDigitalApiBrokerMock.Setup(broker =>
                broker.GetUserInfoAsync(inputCode, inputState, inputCancellationToken))
                    .ThrowsAsync(httpRequestException);

            // when
            ValueTask<string> getUserInfoTask =
                this.nhsDigitalApiService.GetUserInfoAsync(
                    inputCode,
                    inputState,
                    inputCancellationToken);

            NhsDigitalApiDependencyValidationException actualException =
                await Assert.ThrowsAsync<NhsDigitalApiDependencyValidationException>(
                    testCode: getUserInfoTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(
                expectedNhsDigitalApiDependencyValidationException);

            this.nhsDigitalApiBrokerMock.Verify(broker =>
                broker.GetUserInfoAsync(inputCode, inputState, inputCancellationToken),
                Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedNhsDigitalApiDependencyValidationException))),
                Times.Once);

            this.nhsDigitalApiBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnGetUserInfoIfServiceErrorOccursAndLogItAsync()
        {
            // given
            CancellationToken inputCancellationToken = GetCancellationToken();
            string inputCode = GetRandomString();
            string inputState = GetRandomString();
            var serviceException = new Exception();

            var failedNhsDigitalApiServiceException =
                new FailedNhsDigitalApiServiceException(
                    message: "Failed NhsDigitalApi service error occurred, please contact support.",
                    innerException: serviceException);

            var expectedNhsDigitalApiServiceException =
                new NhsDigitalApiServiceException(
                    message: "NhsDigitalApi service error occurred, please contact support.",
                    innerException: failedNhsDigitalApiServiceException);

            this.nhsDigitalApiBrokerMock.Setup(broker =>
                broker.GetUserInfoAsync(inputCode, inputState, inputCancellationToken))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<string> getUserInfoTask =
                this.nhsDigitalApiService.GetUserInfoAsync(
                    inputCode,
                    inputState,
                    inputCancellationToken);

            NhsDigitalApiServiceException actualException =
                await Assert.ThrowsAsync<NhsDigitalApiServiceException>(
                    testCode: getUserInfoTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedNhsDigitalApiServiceException);

            this.nhsDigitalApiBrokerMock.Verify(broker =>
                broker.GetUserInfoAsync(inputCode, inputState, inputCancellationToken),
                Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedNhsDigitalApiServiceException))),
                Times.Once);

            this.nhsDigitalApiBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldBubbleCancellationOnGetUserInfoIfCancelledAsync()
        {
            // given
            CancellationToken inputCancellationToken = GetCancellationToken();
            string inputCode = GetRandomString();
            string inputState = GetRandomString();
            var operationCanceledException = new OperationCanceledException();

            this.nhsDigitalApiBrokerMock.Setup(broker =>
                broker.GetUserInfoAsync(inputCode, inputState, inputCancellationToken))
                    .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<string> getUserInfoTask =
                this.nhsDigitalApiService.GetUserInfoAsync(
                    inputCode,
                    inputState,
                    inputCancellationToken);

            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    testCode: getUserInfoTask.AsTask);

            // then
            actualException.Should().BeSameAs(operationCanceledException);

            this.nhsDigitalApiBrokerMock.Verify(broker =>
                broker.GetUserInfoAsync(inputCode, inputState, inputCancellationToken),
                Times.Once);

            this.nhsDigitalApiBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
