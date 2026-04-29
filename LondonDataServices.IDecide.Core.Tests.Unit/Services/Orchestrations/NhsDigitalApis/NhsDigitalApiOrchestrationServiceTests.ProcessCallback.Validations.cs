// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Core.Models.Orchestrations.NhsDigitalApis;
using LondonDataServices.IDecide.Core.Models.Orchestrations.NhsDigitalApis.Exceptions;
using Moq;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Orchestrations.NhsDigitalApis
{
    public partial class NhsDigitalApiOrchestrationServiceTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task ShouldThrowValidationExceptionOnProcessCallbackWithInvalidCodeAsync(
            string invalidCode)
        {
            // given
            string validState = GetRandomString();
            CancellationToken inputCancellationToken = GetCancellationToken();

            var invalidNhsDigitalApiOrchestrationArgumentException =
                new InvalidNhsDigitalApiOrchestrationArgumentException(
                    message: "Invalid NhsDigitalApi orchestration argument. " +
                        "Please correct the errors and try again.");

            invalidNhsDigitalApiOrchestrationArgumentException.AddData(
                key: "code",
                values: "Value is required.");

            var expectedNhsDigitalApiOrchestrationValidationException =
                new NhsDigitalApiOrchestrationValidationException(
                    message: "NhsDigitalApi orchestration validation error occurred, " +
                        "please fix the errors and try again.",
                    innerException: invalidNhsDigitalApiOrchestrationArgumentException);

            // when
            ValueTask processCallbackTask =
                this.nhsDigitalApiOrchestrationService
                    .ProcessCallbackAsync(invalidCode, validState, inputCancellationToken);

            NhsDigitalApiOrchestrationValidationException
                actualNhsDigitalApiOrchestrationValidationException =
                    await Assert.ThrowsAsync<NhsDigitalApiOrchestrationValidationException>(
                        testCode: processCallbackTask.AsTask);

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

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task ShouldThrowValidationExceptionOnProcessCallbackWithInvalidStateAsync(
            string invalidState)
        {
            // given
            string validCode = GetRandomString();
            CancellationToken inputCancellationToken = GetCancellationToken();

            var invalidNhsDigitalApiOrchestrationArgumentException =
                new InvalidNhsDigitalApiOrchestrationArgumentException(
                    message: "Invalid NhsDigitalApi orchestration argument. " +
                        "Please correct the errors and try again.");

            invalidNhsDigitalApiOrchestrationArgumentException.AddData(
                key: "state",
                values: "Value is required.");

            var expectedNhsDigitalApiOrchestrationValidationException =
                new NhsDigitalApiOrchestrationValidationException(
                    message: "NhsDigitalApi orchestration validation error occurred, " +
                        "please fix the errors and try again.",
                    innerException: invalidNhsDigitalApiOrchestrationArgumentException);

            // when
            ValueTask processCallbackTask =
                this.nhsDigitalApiOrchestrationService
                    .ProcessCallbackAsync(validCode, invalidState, inputCancellationToken);

            NhsDigitalApiOrchestrationValidationException
                actualNhsDigitalApiOrchestrationValidationException =
                    await Assert.ThrowsAsync<NhsDigitalApiOrchestrationValidationException>(
                        testCode: processCallbackTask.AsTask);

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

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task ShouldThrowValidationExceptionOnProcessCallbackWithInvalidNhsIdUserUidAsync(
            string invalidNhsIdUserUid)
        {
            // given
            string validCode = GetRandomString();
            string validState = GetRandomString();
            CancellationToken inputCancellationToken = GetCancellationToken();

            string returnedUserInfoJson =
                CreateUserInfoJson(
                    nhsIdUserUid: invalidNhsIdUserUid,
                    name: GetRandomString(),
                    sub: GetRandomString());

            var invalidNhsDigitalApiOrchestrationArgumentException =
                new InvalidNhsDigitalApiOrchestrationArgumentException(
                    message: "Invalid NhsDigitalApi orchestration argument. " +
                        "Please correct the errors and try again.");

            invalidNhsDigitalApiOrchestrationArgumentException.AddData(
                key: nameof(NhsDigitalUserInfo.NhsIdUserUid),
                values: "Value is required.");

            var expectedNhsDigitalApiOrchestrationValidationException =
                new NhsDigitalApiOrchestrationValidationException(
                    message: "NhsDigitalApi orchestration validation error occurred, " +
                        "please fix the errors and try again.",
                    innerException: invalidNhsDigitalApiOrchestrationArgumentException);

            this.nhsDigitalApiServiceMock.Setup(service =>
                service.GetUserInfoAsync(validCode, validState, inputCancellationToken))
                    .ReturnsAsync(returnedUserInfoJson);

            // when
            ValueTask processCallbackTask =
                this.nhsDigitalApiOrchestrationService
                    .ProcessCallbackAsync(validCode, validState, inputCancellationToken);

            NhsDigitalApiOrchestrationValidationException
                actualNhsDigitalApiOrchestrationValidationException =
                    await Assert.ThrowsAsync<NhsDigitalApiOrchestrationValidationException>(
                        testCode: processCallbackTask.AsTask);

            // then
            actualNhsDigitalApiOrchestrationValidationException
                .Should().BeEquivalentTo(expectedNhsDigitalApiOrchestrationValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedNhsDigitalApiOrchestrationValidationException))),
                        Times.Once);

            this.nhsDigitalApiServiceMock.Verify(service =>
                service.GetUserInfoAsync(validCode, validState, inputCancellationToken),
                Times.Once);

            this.nhsDigitalApiServiceMock.VerifyNoOtherCalls();
            this.userServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task ShouldThrowValidationExceptionOnProcessCallbackWithInvalidNameAsync(
            string invalidName)
        {
            // given
            string validCode = GetRandomString();
            string validState = GetRandomString();
            CancellationToken inputCancellationToken = GetCancellationToken();

            string returnedUserInfoJson =
                CreateUserInfoJson(
                    nhsIdUserUid: GetRandomString(),
                    name: invalidName,
                    sub: GetRandomString());

            var invalidNhsDigitalApiOrchestrationArgumentException =
                new InvalidNhsDigitalApiOrchestrationArgumentException(
                    message: "Invalid NhsDigitalApi orchestration argument. " +
                        "Please correct the errors and try again.");

            invalidNhsDigitalApiOrchestrationArgumentException.AddData(
                key: nameof(NhsDigitalUserInfo.Name),
                values: "Value is required.");

            var expectedNhsDigitalApiOrchestrationValidationException =
                new NhsDigitalApiOrchestrationValidationException(
                    message: "NhsDigitalApi orchestration validation error occurred, " +
                        "please fix the errors and try again.",
                    innerException: invalidNhsDigitalApiOrchestrationArgumentException);

            this.nhsDigitalApiServiceMock.Setup(service =>
                service.GetUserInfoAsync(validCode, validState, inputCancellationToken))
                    .ReturnsAsync(returnedUserInfoJson);

            // when
            ValueTask processCallbackTask =
                this.nhsDigitalApiOrchestrationService
                    .ProcessCallbackAsync(validCode, validState, inputCancellationToken);

            NhsDigitalApiOrchestrationValidationException
                actualNhsDigitalApiOrchestrationValidationException =
                    await Assert.ThrowsAsync<NhsDigitalApiOrchestrationValidationException>(
                        testCode: processCallbackTask.AsTask);

            // then
            actualNhsDigitalApiOrchestrationValidationException
                .Should().BeEquivalentTo(expectedNhsDigitalApiOrchestrationValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedNhsDigitalApiOrchestrationValidationException))),
                        Times.Once);

            this.nhsDigitalApiServiceMock.Verify(service =>
                service.GetUserInfoAsync(validCode, validState, inputCancellationToken),
                Times.Once);

            this.nhsDigitalApiServiceMock.VerifyNoOtherCalls();
            this.userServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task ShouldThrowValidationExceptionOnProcessCallbackWithInvalidSubAsync(
            string invalidSub)
        {
            // given
            string validCode = GetRandomString();
            string validState = GetRandomString();
            CancellationToken inputCancellationToken = GetCancellationToken();

            string returnedUserInfoJson =
                CreateUserInfoJson(
                    nhsIdUserUid: GetRandomString(),
                    name: GetRandomString(),
                    sub: invalidSub);

            var invalidNhsDigitalApiOrchestrationArgumentException =
                new InvalidNhsDigitalApiOrchestrationArgumentException(
                    message: "Invalid NhsDigitalApi orchestration argument. " +
                        "Please correct the errors and try again.");

            invalidNhsDigitalApiOrchestrationArgumentException.AddData(
                key: nameof(NhsDigitalUserInfo.Sub),
                values: "Value is required.");

            var expectedNhsDigitalApiOrchestrationValidationException =
                new NhsDigitalApiOrchestrationValidationException(
                    message: "NhsDigitalApi orchestration validation error occurred, " +
                        "please fix the errors and try again.",
                    innerException: invalidNhsDigitalApiOrchestrationArgumentException);

            this.nhsDigitalApiServiceMock.Setup(service =>
                service.GetUserInfoAsync(validCode, validState, inputCancellationToken))
                    .ReturnsAsync(returnedUserInfoJson);

            // when
            ValueTask processCallbackTask =
                this.nhsDigitalApiOrchestrationService
                    .ProcessCallbackAsync(validCode, validState, inputCancellationToken);

            NhsDigitalApiOrchestrationValidationException
                actualNhsDigitalApiOrchestrationValidationException =
                    await Assert.ThrowsAsync<NhsDigitalApiOrchestrationValidationException>(
                        testCode: processCallbackTask.AsTask);

            // then
            actualNhsDigitalApiOrchestrationValidationException
                .Should().BeEquivalentTo(expectedNhsDigitalApiOrchestrationValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedNhsDigitalApiOrchestrationValidationException))),
                        Times.Once);

            this.nhsDigitalApiServiceMock.Verify(service =>
                service.GetUserInfoAsync(validCode, validState, inputCancellationToken),
                Times.Once);

            this.nhsDigitalApiServiceMock.VerifyNoOtherCalls();
            this.userServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
