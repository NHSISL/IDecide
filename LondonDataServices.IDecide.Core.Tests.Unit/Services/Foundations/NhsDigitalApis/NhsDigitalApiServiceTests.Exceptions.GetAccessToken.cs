// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Xeptions;
using LondonDataServices.IDecide.Core.Models.Foundations.NhsDigitalApis.Exceptions;
using Moq;
using Task = System.Threading.Tasks.Task;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.NhsDigitalApis
{
    public partial class NhsDigitalApiServiceTests
    {
        [Fact]
        public async Task
            ShouldThrowDependencyExceptionOnGetAccessTokenIfServerErrorOccursAndLogItAsync()
        {
            // given
            CancellationToken inputCancellationToken = GetCancellationToken();

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
                broker.GetAccessTokenAsync(inputCancellationToken))
                    .ThrowsAsync(httpRequestException);

            // when
            ValueTask<string> getAccessTokenTask =
                this.nhsDigitalApiService.GetAccessTokenAsync(inputCancellationToken);

            NhsDigitalApiDependencyException actualException =
                await Assert.ThrowsAsync<NhsDigitalApiDependencyException>(
                    testCode: getAccessTokenTask.AsTask);

            // then
            actualException.SameExceptionAs(expectedNhsDigitalApiDependencyException).Should().BeTrue();

            this.nhsDigitalApiBrokerMock.Verify(broker =>
                broker.GetAccessTokenAsync(inputCancellationToken),
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
            ShouldThrowDependencyValidationExceptionOnGetAccessTokenIfClientErrorOccursAndLogItAsync()
        {
            // given
            CancellationToken inputCancellationToken = GetCancellationToken();

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
                broker.GetAccessTokenAsync(inputCancellationToken))
                    .ThrowsAsync(httpRequestException);

            // when
            ValueTask<string> getAccessTokenTask =
                this.nhsDigitalApiService.GetAccessTokenAsync(inputCancellationToken);

            NhsDigitalApiDependencyValidationException actualException =
                await Assert.ThrowsAsync<NhsDigitalApiDependencyValidationException>(
                    testCode: getAccessTokenTask.AsTask);

            // then
            actualException
                .SameExceptionAs(expectedNhsDigitalApiDependencyValidationException)
                .Should().BeTrue();

            this.nhsDigitalApiBrokerMock.Verify(broker =>
                broker.GetAccessTokenAsync(inputCancellationToken),
                Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedNhsDigitalApiDependencyValidationException))),
                Times.Once);

            this.nhsDigitalApiBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnGetAccessTokenIfServiceErrorOccursAndLogItAsync()
        {
            // given
            CancellationToken inputCancellationToken = GetCancellationToken();
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
                broker.GetAccessTokenAsync(inputCancellationToken))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<string> getAccessTokenTask =
                this.nhsDigitalApiService.GetAccessTokenAsync(inputCancellationToken);

            NhsDigitalApiServiceException actualNhsDigitalApiServiceException =
                await Assert.ThrowsAsync<NhsDigitalApiServiceException>(
                    testCode: getAccessTokenTask.AsTask);

            // then
            actualNhsDigitalApiServiceException
                .SameExceptionAs(expectedNhsDigitalApiServiceException)
                .Should().BeTrue();

            this.nhsDigitalApiBrokerMock.Verify(broker =>
                broker.GetAccessTokenAsync(inputCancellationToken),
                Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedNhsDigitalApiServiceException))),
                Times.Once);

            this.nhsDigitalApiBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldBubbleCancellationOnGetAccessTokenIfCancelledAsync()
        {
            // given
            CancellationToken inputCancellationToken = GetCancellationToken();
            var operationCanceledException = new OperationCanceledException();

            this.nhsDigitalApiBrokerMock.Setup(broker =>
                broker.GetAccessTokenAsync(inputCancellationToken))
                    .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<string> getAccessTokenTask =
                this.nhsDigitalApiService.GetAccessTokenAsync(inputCancellationToken);

            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    testCode: getAccessTokenTask.AsTask);

            // then
            ((object)actualException).Should().BeSameAs(operationCanceledException);

            this.nhsDigitalApiBrokerMock.Verify(broker =>
                broker.GetAccessTokenAsync(inputCancellationToken),
                Times.Once);

            this.nhsDigitalApiBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldBubbleCancellationOnGetAccessTokenIfTokenAlreadyCancelledAsync()
        {
            // given
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken inputCancellationToken = cancellationTokenSource.Token;

            // when
            ValueTask<string> getAccessTokenTask =
                this.nhsDigitalApiService.GetAccessTokenAsync(inputCancellationToken);

            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    testCode: getAccessTokenTask.AsTask);

            // then
            ((object)actualException).Should().BeOfType<OperationCanceledException>();

            this.nhsDigitalApiBrokerMock.Verify(broker =>
                broker.GetAccessTokenAsync(It.IsAny<CancellationToken>()),
                Times.Never);

            this.nhsDigitalApiBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
