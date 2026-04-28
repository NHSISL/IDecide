// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using Moq;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.NhsDigitalApis
{
    public partial class NhsDigitalApiServiceTests
    {
        [Fact]
        public async Task ShouldLogoutAsync()
        {
            // given
            CancellationToken inputCancellationToken = GetCancellationToken();

            this.nhsDigitalApiBrokerMock.Setup(broker =>
                broker.LogoutAsync(inputCancellationToken))
                    .Returns(ValueTask.CompletedTask);

            // when
            await this.nhsDigitalApiService.LogoutAsync(inputCancellationToken);

            // then
            this.nhsDigitalApiBrokerMock.Verify(broker =>
                broker.LogoutAsync(inputCancellationToken),
                Times.Once);

            this.nhsDigitalApiBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
