// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.NhsDigitalApis
{
    public partial class NhsDigitalApiServiceTests
    {
        [Fact]
        public async Task ShouldBuildLoginUrlAsync()
        {
            // given
            CancellationToken inputCancellationToken = GetCancellationToken();
            string randomLoginUrl = GetRandomString();
            string expectedLoginUrl = randomLoginUrl.DeepClone();

            this.nhsDigitalApiBrokerMock.Setup(broker =>
                broker.BuildLoginUrlAsync(inputCancellationToken))
                    .ReturnsAsync(randomLoginUrl);

            // when
            string actualLoginUrl =
                await this.nhsDigitalApiService.BuildLoginUrlAsync(inputCancellationToken);

            // then
            actualLoginUrl.Should().BeEquivalentTo(expectedLoginUrl);

            this.nhsDigitalApiBrokerMock.Verify(broker =>
                broker.BuildLoginUrlAsync(inputCancellationToken),
                Times.Once);

            this.nhsDigitalApiBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
