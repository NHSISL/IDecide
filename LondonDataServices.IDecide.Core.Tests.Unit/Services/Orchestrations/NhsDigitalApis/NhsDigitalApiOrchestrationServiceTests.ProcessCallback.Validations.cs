// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
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
        }
    }
