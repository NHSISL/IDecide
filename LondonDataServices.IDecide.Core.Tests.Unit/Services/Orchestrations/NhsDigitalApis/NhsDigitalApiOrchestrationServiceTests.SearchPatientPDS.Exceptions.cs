// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Core.Models.Orchestrations.NhsDigitalApis.Exceptions;
using Moq;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.Pds;
using Xeptions;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Orchestrations.NhsDigitalApis
{
    public partial class NhsDigitalApiOrchestrationServiceTests
    {
        [Theory]
        [MemberData(nameof(NhsDigitalApiDependencyValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnSearchPatientPDSAndLogItAsync(
            Xeption dependencyValidationException)
        {
            // given
            SearchCriteria randomSearchCriteria = CreateRandomSearchCriteria();
            SearchCriteria inputSearchCriteria = randomSearchCriteria;
            CancellationToken inputCancellationToken = GetCancellationToken();

            this.nhsDigitalApiServiceMock.Setup(service =>
                service.SearchPatientPDSAsync(inputSearchCriteria, inputCancellationToken))
                    .ThrowsAsync(dependencyValidationException);

            var expectedNhsDigitalApiOrchestrationDependencyValidationException =
                new NhsDigitalApiOrchestrationDependencyValidationException(
                    message: "NhsDigitalApi orchestration dependency validation error occurred, " +
                        "please fix the errors and try again.",
                    innerException: dependencyValidationException);

            // when
            ValueTask<string> searchPatientPDSTask =
                this.nhsDigitalApiOrchestrationService
                    .SearchPatientPDSAsync(inputSearchCriteria, inputCancellationToken);

            NhsDigitalApiOrchestrationDependencyValidationException
                actualNhsDigitalApiOrchestrationDependencyValidationException =
                    await Assert.ThrowsAsync<NhsDigitalApiOrchestrationDependencyValidationException>(
                        testCode: searchPatientPDSTask.AsTask);

            // then
            actualNhsDigitalApiOrchestrationDependencyValidationException
                .Should().BeEquivalentTo(expectedNhsDigitalApiOrchestrationDependencyValidationException);

            this.nhsDigitalApiServiceMock.Verify(service =>
                service.SearchPatientPDSAsync(inputSearchCriteria, inputCancellationToken),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedNhsDigitalApiOrchestrationDependencyValidationException))),
                        Times.Once);

            this.nhsDigitalApiServiceMock.VerifyNoOtherCalls();
            this.userServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(NhsDigitalApiDependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnSearchPatientPDSAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            SearchCriteria randomSearchCriteria = CreateRandomSearchCriteria();
            SearchCriteria inputSearchCriteria = randomSearchCriteria;
            CancellationToken inputCancellationToken = GetCancellationToken();

            this.nhsDigitalApiServiceMock.Setup(service =>
                service.SearchPatientPDSAsync(inputSearchCriteria, inputCancellationToken))
                    .ThrowsAsync(dependencyException);

            var expectedNhsDigitalApiOrchestrationDependencyException =
                new NhsDigitalApiOrchestrationDependencyException(
                    message: "NhsDigitalApi orchestration dependency error occurred, " +
                        "please fix the errors and try again.",
                    innerException: dependencyException);

            // when
            ValueTask<string> searchPatientPDSTask =
                this.nhsDigitalApiOrchestrationService
                    .SearchPatientPDSAsync(inputSearchCriteria, inputCancellationToken);

            NhsDigitalApiOrchestrationDependencyException
                actualNhsDigitalApiOrchestrationDependencyException =
                    await Assert.ThrowsAsync<NhsDigitalApiOrchestrationDependencyException>(
                        testCode: searchPatientPDSTask.AsTask);

            // then
            actualNhsDigitalApiOrchestrationDependencyException
                .Should().BeEquivalentTo(expectedNhsDigitalApiOrchestrationDependencyException);

            this.nhsDigitalApiServiceMock.Verify(service =>
                service.SearchPatientPDSAsync(inputSearchCriteria, inputCancellationToken),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedNhsDigitalApiOrchestrationDependencyException))),
                        Times.Once);

            this.nhsDigitalApiServiceMock.VerifyNoOtherCalls();
            this.userServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnSearchPatientPDSIfServiceErrorOccursAndLogItAsync()
        {
            // given
            SearchCriteria randomSearchCriteria = CreateRandomSearchCriteria();
            SearchCriteria inputSearchCriteria = randomSearchCriteria;
            CancellationToken inputCancellationToken = GetCancellationToken();
            var serviceException = new Exception();

            this.nhsDigitalApiServiceMock.Setup(service =>
                service.SearchPatientPDSAsync(inputSearchCriteria, inputCancellationToken))
                    .ThrowsAsync(serviceException);

            var failedNhsDigitalApiOrchestrationServiceException =
                new FailedNhsDigitalApiOrchestrationServiceException(
                    message: "Failed NhsDigitalApi orchestration service error occurred, " +
                        "please contact support.",
                    innerException: serviceException);

            var expectedNhsDigitalApiOrchestrationServiceException =
                new NhsDigitalApiOrchestrationServiceException(
                    message: "NhsDigitalApi orchestration service error occurred, please contact support.",
                    innerException: failedNhsDigitalApiOrchestrationServiceException);

            // when
            ValueTask<string> searchPatientPDSTask =
                this.nhsDigitalApiOrchestrationService
                    .SearchPatientPDSAsync(inputSearchCriteria, inputCancellationToken);

            NhsDigitalApiOrchestrationServiceException
                actualNhsDigitalApiOrchestrationServiceException =
                    await Assert.ThrowsAsync<NhsDigitalApiOrchestrationServiceException>(
                        testCode: searchPatientPDSTask.AsTask);

            // then
            actualNhsDigitalApiOrchestrationServiceException
                .Should().BeEquivalentTo(expectedNhsDigitalApiOrchestrationServiceException);

            this.nhsDigitalApiServiceMock.Verify(service =>
                service.SearchPatientPDSAsync(inputSearchCriteria, inputCancellationToken),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedNhsDigitalApiOrchestrationServiceException))),
                        Times.Once);

            this.nhsDigitalApiServiceMock.VerifyNoOtherCalls();
            this.userServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldPropagateCancellationOnSearchPatientPDSWhenCancelledAsync()
        {
            // given
            SearchCriteria randomSearchCriteria = CreateRandomSearchCriteria();
            SearchCriteria inputSearchCriteria = randomSearchCriteria;
            var cancellationTokenSource = new CancellationTokenSource();
            CancellationToken inputCancellationToken = cancellationTokenSource.Token;
            cancellationTokenSource.Cancel();

            this.nhsDigitalApiServiceMock.Setup(service =>
                service.SearchPatientPDSAsync(inputSearchCriteria, inputCancellationToken))
                    .ThrowsAsync(new OperationCanceledException(inputCancellationToken));

            // when
            ValueTask<string> searchPatientPDSTask =
                this.nhsDigitalApiOrchestrationService
                    .SearchPatientPDSAsync(inputSearchCriteria, inputCancellationToken);

            // then
            await Assert.ThrowsAnyAsync<OperationCanceledException>(
                testCode: searchPatientPDSTask.AsTask);

            this.nhsDigitalApiServiceMock.Verify(service =>
                service.SearchPatientPDSAsync(inputSearchCriteria, inputCancellationToken),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.IsAny<Exception>()),
                    Times.Never);

            this.nhsDigitalApiServiceMock.VerifyNoOtherCalls();
            this.userServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
