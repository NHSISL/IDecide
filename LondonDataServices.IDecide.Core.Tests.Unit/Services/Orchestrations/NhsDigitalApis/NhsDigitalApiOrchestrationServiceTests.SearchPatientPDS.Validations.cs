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
        public async Task ShouldThrowValidationExceptionOnSearchPatientPDSWithNullSearchCriteriaAsync()
        {
            // given
            SearchCriteria nullSearchCriteria = null;
            CancellationToken inputCancellationToken = GetCancellationToken();

            var invalidNhsDigitalApiOrchestrationArgumentException =
                new InvalidNhsDigitalApiOrchestrationArgumentException(
                    message: "Invalid NhsDigitalApi orchestration argument. " +
                        "Please correct the errors and try again.");

            invalidNhsDigitalApiOrchestrationArgumentException.AddData(
                key: "searchCriteria",
                values: "Value is required.");

            var expectedNhsDigitalApiOrchestrationValidationException =
                new NhsDigitalApiOrchestrationValidationException(
                    message: "NhsDigitalApi orchestration validation error occurred, " +
                        "please fix the errors and try again.",
                    innerException: invalidNhsDigitalApiOrchestrationArgumentException);

            // when
            ValueTask<string> searchPatientPDSTask =
                this.nhsDigitalApiOrchestrationService
                    .SearchPatientPDSAsync(nullSearchCriteria, inputCancellationToken);

            NhsDigitalApiOrchestrationValidationException
                actualNhsDigitalApiOrchestrationValidationException =
                    await Assert.ThrowsAsync<NhsDigitalApiOrchestrationValidationException>(
                        testCode: searchPatientPDSTask.AsTask);

            // then
            actualNhsDigitalApiOrchestrationValidationException
                .Should().BeEquivalentTo(expectedNhsDigitalApiOrchestrationValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedNhsDigitalApiOrchestrationValidationException))),
                        Times.Once);

            this.nhsDigitalApiServiceMock.VerifyNoOtherCalls();
            this.userServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
