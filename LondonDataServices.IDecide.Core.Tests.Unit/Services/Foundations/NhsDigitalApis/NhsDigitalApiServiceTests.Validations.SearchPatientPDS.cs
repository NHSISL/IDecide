// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Xeptions;
using LondonDataServices.IDecide.Core.Models.Foundations.NhsDigitalApis.Exceptions;
using Moq;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.Pds;
using Task = System.Threading.Tasks.Task;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.NhsDigitalApis
{
    public partial class NhsDigitalApiServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnSearchPatientPDSWhenSearchCriteriaIsNullAsync()
        {
            // given
            SearchCriteria nullSearchCriteria = null;
            CancellationToken inputCancellationToken = GetCancellationToken();

            var nullNhsDigitalApiSearchCriteriaException =
                new NullNhsDigitalApiSearchCriteriaException(
                    message: "Search criteria is null.");

            var expectedNhsDigitalApiValidationException =
                new NhsDigitalApiValidationException(
                    message: "NhsDigitalApi validation error occurred, " +
                        "please fix the errors and try again.",
                    innerException: nullNhsDigitalApiSearchCriteriaException);

            // when
            ValueTask<string> searchPatientPDSTask =
                this.nhsDigitalApiService.SearchPatientPDSAsync(
                    nullSearchCriteria,
                    inputCancellationToken);

            NhsDigitalApiValidationException actualNhsDigitalApiValidationException =
                await Assert.ThrowsAsync<NhsDigitalApiValidationException>(
                    testCode: searchPatientPDSTask.AsTask);

            // then
            actualNhsDigitalApiValidationException
                .SameExceptionAs(expectedNhsDigitalApiValidationException)
                .Should().BeTrue();

            this.nhsDigitalApiBrokerMock.Verify(broker =>
                broker.SearchPatientPDSAsync(
                    It.IsAny<SearchCriteria>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedNhsDigitalApiValidationException))),
                Times.Once);

            this.nhsDigitalApiBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
