// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Core.Models.Orchestrations.NhsDigitalApis.Exceptions;
using Moq;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.Pds;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Orchestrations.NhsDigitalApis
{
    public partial class NhsDigitalApiOrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnSearchPatientPDSWhenSearchCriteriaIsNullAsync()
        {
            // given
            SearchCriteria nullSearchCriteria = null;
            CancellationToken inputCancellationToken = GetCancellationToken();

            var nullNhsDigitalApiOrchestrationSearchCriteriaException =
                new NullNhsDigitalApiOrchestrationSearchCriteriaException(
                    message: "Search criteria is null.");

            var expectedNhsDigitalApiOrchestrationValidationException =
                new NhsDigitalApiOrchestrationValidationException(
                    message: "NhsDigitalApi orchestration validation error occurred, " +
                        "please fix the errors and try again.",
                    innerException: nullNhsDigitalApiOrchestrationSearchCriteriaException);

            // when
            ValueTask<string> searchPatientPDSTask =
                this.nhsDigitalApiOrchestrationService.SearchPatientPDSAsync(
                    nullSearchCriteria,
                    inputCancellationToken);

            NhsDigitalApiOrchestrationValidationException
                actualNhsDigitalApiOrchestrationValidationException =
                    await Assert.ThrowsAsync<NhsDigitalApiOrchestrationValidationException>(
                        testCode: searchPatientPDSTask.AsTask);

            // then
            actualNhsDigitalApiOrchestrationValidationException.Should().BeEquivalentTo(
                expectedNhsDigitalApiOrchestrationValidationException);

            this.nhsDigitalApiServiceMock.Verify(service =>
                service.SearchPatientPDSAsync(
                    It.IsAny<SearchCriteria>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedNhsDigitalApiOrchestrationValidationException))),
                Times.Once);

            this.nhsDigitalApiServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
