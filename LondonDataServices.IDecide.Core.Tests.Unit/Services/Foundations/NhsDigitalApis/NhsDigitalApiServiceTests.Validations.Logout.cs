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
        [Fact]
        public async Task ShouldThrowValidationExceptionOnLogoutWhenCancellationTokenIsCancelledAsync()
        {
            // given
            CancellationToken cancelledCancellationToken = new CancellationToken(canceled: true);

            var cancelledNhsDigitalApiCancellationTokenException =
                new CancelledNhsDigitalApiCancellationTokenException(
                    message: "Cancellation token is already cancelled.");

            var expectedNhsDigitalApiValidationException =
                new NhsDigitalApiValidationException(
                    message: "NhsDigitalApi validation error occurred, " +
                        "please fix the errors and try again.",
                    innerException: cancelledNhsDigitalApiCancellationTokenException);

            // when
            ValueTask logoutTask =
                this.nhsDigitalApiService.LogoutAsync(cancelledCancellationToken);

            NhsDigitalApiValidationException actualNhsDigitalApiValidationException =
                await Assert.ThrowsAsync<NhsDigitalApiValidationException>(
                    testCode: logoutTask.AsTask);

            // then
            actualNhsDigitalApiValidationException.Should().BeEquivalentTo(
                expectedNhsDigitalApiValidationException);

            this.nhsDigitalApiBrokerMock.Verify(broker =>
                broker.LogoutAsync(It.IsAny<CancellationToken>()),
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
