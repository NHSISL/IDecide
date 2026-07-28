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
        public async Task ShouldThrowDependencyValidationExceptionOnLogoutAndLogItAsync(
            Xeption dependencyValidationException)
        {
            // given
            CancellationToken inputCancellationToken = GetCancellationToken();

            this.nhsDigitalApiServiceMock.Setup(service =>
                service.LogoutAsync(inputCancellationToken))
                    .ThrowsAsync(dependencyValidationException);

            var expectedNhsDigitalApiOrchestrationDependencyValidationException =
                new NhsDigitalApiOrchestrationDependencyValidationException(
                    message: "NhsDigitalApi orchestration dependency validation error occurred, " +
                        "please fix the errors and try again.",
                    innerException: dependencyValidationException);

            // when
            ValueTask logoutTask =
                this.nhsDigitalApiOrchestrationService
                    .LogoutAsync(inputCancellationToken);

            NhsDigitalApiOrchestrationDependencyValidationException
                actualNhsDigitalApiOrchestrationDependencyValidationException =
                    await Assert.ThrowsAsync<NhsDigitalApiOrchestrationDependencyValidationException>(
                        testCode: logoutTask.AsTask);

            // then
            actualNhsDigitalApiOrchestrationDependencyValidationException
                .SameExceptionAs(expectedNhsDigitalApiOrchestrationDependencyValidationException)
                .Should().BeTrue();

            this.nhsDigitalApiServiceMock.Verify(service =>
                service.LogoutAsync(inputCancellationToken),
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
        public async Task ShouldThrowDependencyExceptionOnLogoutAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            CancellationToken inputCancellationToken = GetCancellationToken();

            this.nhsDigitalApiServiceMock.Setup(service =>
                service.LogoutAsync(inputCancellationToken))
                    .ThrowsAsync(dependencyException);

            var expectedNhsDigitalApiOrchestrationDependencyException =
                new NhsDigitalApiOrchestrationDependencyException(
                    message: "NhsDigitalApi orchestration dependency error occurred, " +
                        "please fix the errors and try again.",
                    innerException: dependencyException);

            // when
            ValueTask logoutTask =
                this.nhsDigitalApiOrchestrationService
                    .LogoutAsync(inputCancellationToken);

            NhsDigitalApiOrchestrationDependencyException
                actualNhsDigitalApiOrchestrationDependencyException =
                    await Assert.ThrowsAsync<NhsDigitalApiOrchestrationDependencyException>(
                        testCode: logoutTask.AsTask);

            // then
            actualNhsDigitalApiOrchestrationDependencyException
                .SameExceptionAs(expectedNhsDigitalApiOrchestrationDependencyException)
                .Should().BeTrue();

            this.nhsDigitalApiServiceMock.Verify(service =>
                service.LogoutAsync(inputCancellationToken),
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
        public async Task ShouldThrowServiceExceptionOnLogoutIfServiceErrorOccursAndLogItAsync()
        {
            // given
            CancellationToken inputCancellationToken = GetCancellationToken();
            var serviceException = new Exception();

            this.nhsDigitalApiServiceMock.Setup(service =>
                service.LogoutAsync(inputCancellationToken))
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
            ValueTask logoutTask =
                this.nhsDigitalApiOrchestrationService
                    .LogoutAsync(inputCancellationToken);

            NhsDigitalApiOrchestrationServiceException
                actualNhsDigitalApiOrchestrationServiceException =
                    await Assert.ThrowsAsync<NhsDigitalApiOrchestrationServiceException>(
                        testCode: logoutTask.AsTask);

            // then
            actualNhsDigitalApiOrchestrationServiceException
                .SameExceptionAs(expectedNhsDigitalApiOrchestrationServiceException)
                .Should().BeTrue();

            this.nhsDigitalApiServiceMock.Verify(service =>
                service.LogoutAsync(inputCancellationToken),
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
        public async Task ShouldPropagateCancellationOnLogoutWhenCancelledAsync()
        {
            // given
            var cancellationTokenSource = new CancellationTokenSource();
            CancellationToken inputCancellationToken = cancellationTokenSource.Token;
            cancellationTokenSource.Cancel();

            this.nhsDigitalApiServiceMock.Setup(service =>
                service.LogoutAsync(inputCancellationToken))
                    .ThrowsAsync(new OperationCanceledException(inputCancellationToken));

            // when
            ValueTask logoutTask =
                this.nhsDigitalApiOrchestrationService
                    .LogoutAsync(inputCancellationToken);

            // then
            await Assert.ThrowsAnyAsync<OperationCanceledException>(
                testCode: logoutTask.AsTask);

            this.nhsDigitalApiServiceMock.Verify(service =>
                service.LogoutAsync(inputCancellationToken),
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
