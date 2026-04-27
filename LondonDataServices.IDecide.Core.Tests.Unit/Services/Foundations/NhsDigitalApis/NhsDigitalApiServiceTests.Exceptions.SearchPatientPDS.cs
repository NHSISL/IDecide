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
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.Pds;
using Task = System.Threading.Tasks.Task;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.NhsDigitalApis
{
    public partial class NhsDigitalApiServiceTests
    {
        [Fact]
        public async Task ShouldThrowServiceExceptionOnSearchPatientPDSIfServiceErrorOccursAndLogItAsync()
        {
            // given
            SearchCriteria randomSearchCriteria = CreateRandomSearchCriteria();
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
                broker.SearchPatientPDSAsync(
                    randomSearchCriteria,
                    inputCancellationToken))
                        .ThrowsAsync(serviceException);

            // when
            ValueTask<string> searchPatientPDSTask =
                this.nhsDigitalApiService.SearchPatientPDSAsync(
                    randomSearchCriteria,
                    inputCancellationToken);

            NhsDigitalApiServiceException actualNhsDigitalApiServiceException =
                await Assert.ThrowsAsync<NhsDigitalApiServiceException>(
                    testCode: searchPatientPDSTask.AsTask);

            // then
            actualNhsDigitalApiServiceException.Should().BeEquivalentTo(
                expectedNhsDigitalApiServiceException);

            this.nhsDigitalApiBrokerMock.Verify(broker =>
                broker.SearchPatientPDSAsync(
                    randomSearchCriteria,
                    inputCancellationToken),
                Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedNhsDigitalApiServiceException))),
                Times.Once);

            this.nhsDigitalApiBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task
            ShouldThrowDependencyValidationExceptionOnSearchPatientPDSIfClientErrorOccursAndLogItAsync()
        {
            // given
            SearchCriteria randomSearchCriteria = CreateRandomSearchCriteria();
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
                broker.SearchPatientPDSAsync(
                    randomSearchCriteria,
                    inputCancellationToken))
                        .ThrowsAsync(httpRequestException);

            // when
            ValueTask<string> searchPatientPDSTask =
                this.nhsDigitalApiService.SearchPatientPDSAsync(
                    randomSearchCriteria,
                    inputCancellationToken);

            NhsDigitalApiDependencyValidationException actualException =
                await Assert.ThrowsAsync<NhsDigitalApiDependencyValidationException>(
                    testCode: searchPatientPDSTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(
                expectedNhsDigitalApiDependencyValidationException);

            this.nhsDigitalApiBrokerMock.Verify(broker =>
                broker.SearchPatientPDSAsync(
                    randomSearchCriteria,
                    inputCancellationToken),
                Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedNhsDigitalApiDependencyValidationException))),
                Times.Once);

            this.nhsDigitalApiBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
