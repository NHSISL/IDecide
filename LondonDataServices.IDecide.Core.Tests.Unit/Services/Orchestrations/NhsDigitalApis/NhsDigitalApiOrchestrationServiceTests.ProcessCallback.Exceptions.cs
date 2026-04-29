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
        [MemberData(nameof(DependencyValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnProcessCallbackAndLogItAsync(
            Xeption dependencyValidationException)
        {
            // given
            string randomCode = GetRandomString();
            string randomState = GetRandomString();
            string inputCode = randomCode;
            string inputState = randomState;
            CancellationToken inputCancellationToken = GetCancellationToken();

            this.nhsDigitalApiServiceMock.Setup(service =>
                service.GetUserInfoAsync(inputCode, inputState, inputCancellationToken))
                    .ThrowsAsync(dependencyValidationException);

            var expectedNhsDigitalApiOrchestrationDependencyValidationException =
                new NhsDigitalApiOrchestrationDependencyValidationException(
                    message: "NhsDigitalApi orchestration dependency validation error occurred, " +
                        "please fix the errors and try again.",
                    innerException: dependencyValidationException);

            // when
            ValueTask processCallbackTask =
                this.nhsDigitalApiOrchestrationService
                    .ProcessCallbackAsync(inputCode, inputState, inputCancellationToken);

            NhsDigitalApiOrchestrationDependencyValidationException
                actualNhsDigitalApiOrchestrationDependencyValidationException =
                    await Assert.ThrowsAsync<NhsDigitalApiOrchestrationDependencyValidationException>(
                        testCode: processCallbackTask.AsTask);

            // then
            actualNhsDigitalApiOrchestrationDependencyValidationException
                .Should().BeEquivalentTo(expectedNhsDigitalApiOrchestrationDependencyValidationException);

            this.nhsDigitalApiServiceMock.Verify(service =>
                service.GetUserInfoAsync(inputCode, inputState, inputCancellationToken),
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
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnProcessCallbackAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            string randomCode = GetRandomString();
            string randomState = GetRandomString();
            string inputCode = randomCode;
            string inputState = randomState;
            CancellationToken inputCancellationToken = GetCancellationToken();

            this.nhsDigitalApiServiceMock.Setup(service =>
                service.GetUserInfoAsync(inputCode, inputState, inputCancellationToken))
                    .ThrowsAsync(dependencyException);

            var expectedNhsDigitalApiOrchestrationDependencyException =
                new NhsDigitalApiOrchestrationDependencyException(
                    message: "NhsDigitalApi orchestration dependency error occurred, " +
                        "please fix the errors and try again.",
                    innerException: dependencyException);

            // when
            ValueTask processCallbackTask =
                this.nhsDigitalApiOrchestrationService
                    .ProcessCallbackAsync(inputCode, inputState, inputCancellationToken);

            NhsDigitalApiOrchestrationDependencyException
                actualNhsDigitalApiOrchestrationDependencyException =
                    await Assert.ThrowsAsync<NhsDigitalApiOrchestrationDependencyException>(
                        testCode: processCallbackTask.AsTask);

            // then
            actualNhsDigitalApiOrchestrationDependencyException
                .Should().BeEquivalentTo(expectedNhsDigitalApiOrchestrationDependencyException);

            this.nhsDigitalApiServiceMock.Verify(service =>
                service.GetUserInfoAsync(inputCode, inputState, inputCancellationToken),
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
        public async Task ShouldThrowServiceExceptionOnProcessCallbackIfServiceErrorOccursAndLogItAsync()
        {
            // given
            string randomCode = GetRandomString();
            string randomState = GetRandomString();
            string inputCode = randomCode;
            string inputState = randomState;
            CancellationToken inputCancellationToken = GetCancellationToken();
            var serviceException = new Exception();

            this.nhsDigitalApiServiceMock.Setup(service =>
                service.GetUserInfoAsync(inputCode, inputState, inputCancellationToken))
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
            ValueTask processCallbackTask =
                this.nhsDigitalApiOrchestrationService
                    .ProcessCallbackAsync(inputCode, inputState, inputCancellationToken);

            NhsDigitalApiOrchestrationServiceException
                actualNhsDigitalApiOrchestrationServiceException =
                    await Assert.ThrowsAsync<NhsDigitalApiOrchestrationServiceException>(
                        testCode: processCallbackTask.AsTask);

            // then
            actualNhsDigitalApiOrchestrationServiceException
                .Should().BeEquivalentTo(expectedNhsDigitalApiOrchestrationServiceException);

            this.nhsDigitalApiServiceMock.Verify(service =>
                service.GetUserInfoAsync(inputCode, inputState, inputCancellationToken),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedNhsDigitalApiOrchestrationServiceException))),
                        Times.Once);

            this.nhsDigitalApiServiceMock.VerifyNoOtherCalls();
            this.userServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
