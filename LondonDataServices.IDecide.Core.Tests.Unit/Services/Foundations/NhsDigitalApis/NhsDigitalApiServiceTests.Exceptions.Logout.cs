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
            ShouldThrowDependencyValidationExceptionOnLogoutIfClientErrorOccursAndLogItAsync()
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
                broker.LogoutAsync(inputCancellationToken))
                    .ThrowsAsync(httpRequestException);

            // when
            ValueTask logoutTask =
                this.nhsDigitalApiService.LogoutAsync(inputCancellationToken);

            NhsDigitalApiDependencyValidationException actualException =
                await Assert.ThrowsAsync<NhsDigitalApiDependencyValidationException>(
                    testCode: logoutTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(
                expectedNhsDigitalApiDependencyValidationException);

            this.nhsDigitalApiBrokerMock.Verify(broker =>
                broker.LogoutAsync(inputCancellationToken),
                Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedNhsDigitalApiDependencyValidationException))),
                Times.Once);

            this.nhsDigitalApiBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnLogoutIfServiceErrorOccursAndLogItAsync()
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
                broker.LogoutAsync(inputCancellationToken))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask logoutTask =
                this.nhsDigitalApiService.LogoutAsync(inputCancellationToken);

            NhsDigitalApiServiceException actualNhsDigitalApiServiceException =
                await Assert.ThrowsAsync<NhsDigitalApiServiceException>(
                    testCode: logoutTask.AsTask);

            // then
            actualNhsDigitalApiServiceException.Should().BeEquivalentTo(
                expectedNhsDigitalApiServiceException);

            this.nhsDigitalApiBrokerMock.Verify(broker =>
                broker.LogoutAsync(inputCancellationToken),
                Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedNhsDigitalApiServiceException))),
                Times.Once);

            this.nhsDigitalApiBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
