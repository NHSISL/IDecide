// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Core.Models.Foundations.NhsDigitalApis.Exceptions;
using LondonDataServices.IDecide.Core.Models.Orchestrations.NhsDigitalApis.Exceptions;
using Moq;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.Pds;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Orchestrations.NhsDigitalApis
{
    public partial class NhsDigitalApiOrchestrationServiceTests
    {
        [Fact]
        public async Task
            ShouldThrowDependencyValidationExceptionOnSearchPatientPDSWhenValidationExceptionOccursAndLogItAsync()
        {
            // given
            SearchCriteria randomSearchCriteria = CreateRandomSearchCriteria();
            CancellationToken inputCancellationToken = GetCancellationToken();

            var nhsDigitalApiValidationException =
                new NhsDigitalApiValidationException(
                    message: "NhsDigitalApi validation error occurred, " +
                        "please fix the errors and try again.",
                    innerException: new Xeptions.Xeption());

            var expectedNhsDigitalApiOrchestrationDependencyValidationException =
                new NhsDigitalApiOrchestrationDependencyValidationException(
                    message: "NhsDigitalApi orchestration dependency validation error occurred, " +
                        "please fix the errors and try again.",
                    innerException: nhsDigitalApiValidationException);

            this.nhsDigitalApiServiceMock.Setup(service =>
                service.SearchPatientPDSAsync(
                    randomSearchCriteria,
                    inputCancellationToken))
                        .ThrowsAsync(nhsDigitalApiValidationException);

            // when
            ValueTask<string> searchPatientPDSTask =
                this.nhsDigitalApiOrchestrationService.SearchPatientPDSAsync(
                    randomSearchCriteria,
                    inputCancellationToken);

            NhsDigitalApiOrchestrationDependencyValidationException actualException =
                await Assert.ThrowsAsync<NhsDigitalApiOrchestrationDependencyValidationException>(
                    testCode: searchPatientPDSTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(
                expectedNhsDigitalApiOrchestrationDependencyValidationException);

            this.nhsDigitalApiServiceMock.Verify(service =>
                service.SearchPatientPDSAsync(
                    randomSearchCriteria,
                    inputCancellationToken),
                Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedNhsDigitalApiOrchestrationDependencyValidationException))),
                Times.Once);

            this.nhsDigitalApiServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task
            ShouldThrowDependencyValidationExceptionOnSearchPatientPDSWhenDependencyValidationExceptionOccursAndLogItAsync()
        {
            // given
            SearchCriteria randomSearchCriteria = CreateRandomSearchCriteria();
            CancellationToken inputCancellationToken = GetCancellationToken();

            var nhsDigitalApiDependencyValidationException =
                new NhsDigitalApiDependencyValidationException(
                    message: "NhsDigitalApi dependency validation error occurred, " +
                        "please fix the errors and try again.",
                    innerException: new Xeptions.Xeption());

            var expectedNhsDigitalApiOrchestrationDependencyValidationException =
                new NhsDigitalApiOrchestrationDependencyValidationException(
                    message: "NhsDigitalApi orchestration dependency validation error occurred, " +
                        "please fix the errors and try again.",
                    innerException: nhsDigitalApiDependencyValidationException);

            this.nhsDigitalApiServiceMock.Setup(service =>
                service.SearchPatientPDSAsync(
                    randomSearchCriteria,
                    inputCancellationToken))
                        .ThrowsAsync(nhsDigitalApiDependencyValidationException);

            // when
            ValueTask<string> searchPatientPDSTask =
                this.nhsDigitalApiOrchestrationService.SearchPatientPDSAsync(
                    randomSearchCriteria,
                    inputCancellationToken);

            NhsDigitalApiOrchestrationDependencyValidationException actualException =
                await Assert.ThrowsAsync<NhsDigitalApiOrchestrationDependencyValidationException>(
                    testCode: searchPatientPDSTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(
                expectedNhsDigitalApiOrchestrationDependencyValidationException);

            this.nhsDigitalApiServiceMock.Verify(service =>
                service.SearchPatientPDSAsync(
                    randomSearchCriteria,
                    inputCancellationToken),
                Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedNhsDigitalApiOrchestrationDependencyValidationException))),
                Times.Once);

            this.nhsDigitalApiServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task
            ShouldThrowDependencyExceptionOnSearchPatientPDSWhenDependencyExceptionOccursAndLogItAsync()
        {
            // given
            SearchCriteria randomSearchCriteria = CreateRandomSearchCriteria();
            CancellationToken inputCancellationToken = GetCancellationToken();

            var nhsDigitalApiDependencyException =
                new NhsDigitalApiDependencyException(
                    message: "NhsDigitalApi dependency error occurred, please contact support.",
                    innerException: new Xeptions.Xeption());

            var expectedNhsDigitalApiOrchestrationDependencyException =
                new NhsDigitalApiOrchestrationDependencyException(
                    message: "NhsDigitalApi orchestration dependency error occurred, please contact support.",
                    innerException: nhsDigitalApiDependencyException);

            this.nhsDigitalApiServiceMock.Setup(service =>
                service.SearchPatientPDSAsync(
                    randomSearchCriteria,
                    inputCancellationToken))
                        .ThrowsAsync(nhsDigitalApiDependencyException);

            // when
            ValueTask<string> searchPatientPDSTask =
                this.nhsDigitalApiOrchestrationService.SearchPatientPDSAsync(
                    randomSearchCriteria,
                    inputCancellationToken);

            NhsDigitalApiOrchestrationDependencyException actualException =
                await Assert.ThrowsAsync<NhsDigitalApiOrchestrationDependencyException>(
                    testCode: searchPatientPDSTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(
                expectedNhsDigitalApiOrchestrationDependencyException);

            this.nhsDigitalApiServiceMock.Verify(service =>
                service.SearchPatientPDSAsync(
                    randomSearchCriteria,
                    inputCancellationToken),
                Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedNhsDigitalApiOrchestrationDependencyException))),
                Times.Once);

            this.nhsDigitalApiServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
