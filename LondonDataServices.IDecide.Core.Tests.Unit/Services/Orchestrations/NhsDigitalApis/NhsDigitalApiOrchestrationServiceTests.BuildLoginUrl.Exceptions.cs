// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Core.Models.Orchestrations.NhsDigitalApis.Exceptions;
using Moq;
using Xeptions;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Orchestrations.NhsDigitalApis
{
    public partial class NhsDigitalApiOrchestrationServiceTests
    {
        [Theory]
        [MemberData(nameof(NhsDigitalApiDependencyValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnBuildLoginUrlAndLogItAsync(
            Xeption dependencyValidationException)
        {
            // given
            CancellationToken inputCancellationToken = GetCancellationToken();

            this.nhsDigitalApiServiceMock.Setup(service =>
                service.BuildLoginUrlAsync(inputCancellationToken))
                    .ThrowsAsync(dependencyValidationException);

            var expectedNhsDigitalApiOrchestrationDependencyValidationException =
                new NhsDigitalApiOrchestrationDependencyValidationException(
                    message: "NhsDigitalApi orchestration dependency validation error occurred, " +
                        "please fix the errors and try again.",
                    innerException: dependencyValidationException);

            // when
            ValueTask<string> buildLoginUrlTask =
                this.nhsDigitalApiOrchestrationService
                    .BuildLoginUrlAsync(inputCancellationToken);

            NhsDigitalApiOrchestrationDependencyValidationException
                actualNhsDigitalApiOrchestrationDependencyValidationException =
                    await Assert.ThrowsAsync<NhsDigitalApiOrchestrationDependencyValidationException>(
                        testCode: buildLoginUrlTask.AsTask);

            // then
            actualNhsDigitalApiOrchestrationDependencyValidationException
                .SameExceptionAs(expectedNhsDigitalApiOrchestrationDependencyValidationException)
                .Should().BeTrue();

            this.nhsDigitalApiServiceMock.Verify(service =>
                service.BuildLoginUrlAsync(inputCancellationToken),
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
        public async Task ShouldThrowDependencyExceptionOnBuildLoginUrlAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            CancellationToken inputCancellationToken = GetCancellationToken();

            this.nhsDigitalApiServiceMock.Setup(service =>
                service.BuildLoginUrlAsync(inputCancellationToken))
                    .ThrowsAsync(dependencyException);

            var expectedNhsDigitalApiOrchestrationDependencyException =
                new NhsDigitalApiOrchestrationDependencyException(
                    message: "NhsDigitalApi orchestration dependency error occurred, " +
                        "please fix the errors and try again.",
                    innerException: dependencyException);

            // when
            ValueTask<string> buildLoginUrlTask =
                this.nhsDigitalApiOrchestrationService
                    .BuildLoginUrlAsync(inputCancellationToken);

            NhsDigitalApiOrchestrationDependencyException
                actualNhsDigitalApiOrchestrationDependencyException =
                    await Assert.ThrowsAsync<NhsDigitalApiOrchestrationDependencyException>(
                        testCode: buildLoginUrlTask.AsTask);

            // then
            actualNhsDigitalApiOrchestrationDependencyException
                .SameExceptionAs(expectedNhsDigitalApiOrchestrationDependencyException)
                .Should().BeTrue();

            this.nhsDigitalApiServiceMock.Verify(service =>
                service.BuildLoginUrlAsync(inputCancellationToken),
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
        public async Task ShouldThrowServiceExceptionOnBuildLoginUrlIfServiceErrorOccursAndLogItAsync()
        {
            // given
            CancellationToken inputCancellationToken = GetCancellationToken();
            var serviceException = new Exception();

            this.nhsDigitalApiServiceMock.Setup(service =>
                service.BuildLoginUrlAsync(inputCancellationToken))
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
            ValueTask<string> buildLoginUrlTask =
                this.nhsDigitalApiOrchestrationService
                    .BuildLoginUrlAsync(inputCancellationToken);

            NhsDigitalApiOrchestrationServiceException
                actualNhsDigitalApiOrchestrationServiceException =
                    await Assert.ThrowsAsync<NhsDigitalApiOrchestrationServiceException>(
                        testCode: buildLoginUrlTask.AsTask);

            // then
            actualNhsDigitalApiOrchestrationServiceException
                .SameExceptionAs(expectedNhsDigitalApiOrchestrationServiceException)
                .Should().BeTrue();

            this.nhsDigitalApiServiceMock.Verify(service =>
                service.BuildLoginUrlAsync(inputCancellationToken),
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
        public async Task ShouldPropagateCancellationOnBuildLoginUrlWhenCancelledAsync()
        {
            // given
            var cancellationTokenSource = new CancellationTokenSource();
            CancellationToken inputCancellationToken = cancellationTokenSource.Token;
            cancellationTokenSource.Cancel();

            this.nhsDigitalApiServiceMock.Setup(service =>
                service.BuildLoginUrlAsync(inputCancellationToken))
                    .ThrowsAsync(new OperationCanceledException(inputCancellationToken));

            // when
            ValueTask<string> buildLoginUrlTask =
                this.nhsDigitalApiOrchestrationService
                    .BuildLoginUrlAsync(inputCancellationToken);

            // then
            await Assert.ThrowsAnyAsync<OperationCanceledException>(
                testCode: buildLoginUrlTask.AsTask);

            this.nhsDigitalApiServiceMock.Verify(service =>
                service.BuildLoginUrlAsync(inputCancellationToken),
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