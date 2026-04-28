// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Core.Models.Foundations.NhsDigitalApis.Exceptions;
using Moq;
using Task = System.Threading.Tasks.Task;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.NhsDigitalApis
{
    public partial class NhsDigitalApiServiceTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task ShouldThrowValidationExceptionOnGetUserInfoWhenCodeIsInvalidAsync(
            string invalidCode)
        {
            // given
            CancellationToken inputCancellationToken = GetCancellationToken();
            string inputState = GetRandomString();

            var invalidNhsDigitalApiUserInfoException =
                new InvalidNhsDigitalApiUserInfoException(
                    message: "Invalid NhsDigitalApi user info arguments. " +
                        "Please correct the errors and try again.");

            invalidNhsDigitalApiUserInfoException.UpsertDataList(
                key: "code",
                value: "Value is required.");

            var expectedNhsDigitalApiValidationException =
                new NhsDigitalApiValidationException(
                    message: "NhsDigitalApi validation error occurred, " +
                        "please fix the errors and try again.",
                    innerException: invalidNhsDigitalApiUserInfoException);

            // when
            ValueTask<string> getUserInfoTask =
                this.nhsDigitalApiService.GetUserInfoAsync(
                    invalidCode,
                    inputState,
                    inputCancellationToken);

            NhsDigitalApiValidationException actualException =
                await Assert.ThrowsAsync<NhsDigitalApiValidationException>(
                    testCode: getUserInfoTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedNhsDigitalApiValidationException);

            this.nhsDigitalApiBrokerMock.Verify(broker =>
                broker.GetUserInfoAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedNhsDigitalApiValidationException))),
                Times.Once);

            this.nhsDigitalApiBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task ShouldThrowValidationExceptionOnGetUserInfoWhenStateIsInvalidAsync(
            string invalidState)
        {
            // given
            CancellationToken inputCancellationToken = GetCancellationToken();
            string inputCode = GetRandomString();

            var invalidNhsDigitalApiUserInfoException =
                new InvalidNhsDigitalApiUserInfoException(
                    message: "Invalid NhsDigitalApi user info arguments. " +
                        "Please correct the errors and try again.");

            invalidNhsDigitalApiUserInfoException.UpsertDataList(
                key: "state",
                value: "Value is required.");

            var expectedNhsDigitalApiValidationException =
                new NhsDigitalApiValidationException(
                    message: "NhsDigitalApi validation error occurred, " +
                        "please fix the errors and try again.",
                    innerException: invalidNhsDigitalApiUserInfoException);

            // when
            ValueTask<string> getUserInfoTask =
                this.nhsDigitalApiService.GetUserInfoAsync(
                    inputCode,
                    invalidState,
                    inputCancellationToken);

            NhsDigitalApiValidationException actualException =
                await Assert.ThrowsAsync<NhsDigitalApiValidationException>(
                    testCode: getUserInfoTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedNhsDigitalApiValidationException);

            this.nhsDigitalApiBrokerMock.Verify(broker =>
                broker.GetUserInfoAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
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
