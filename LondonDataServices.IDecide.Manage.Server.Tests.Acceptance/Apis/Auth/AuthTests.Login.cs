// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Apis.Auth
{
    public partial class AuthTests
    {
        [Fact]
        public async Task ShouldRedirectToLoginUrlAsync()
        {
            // given
            this.apiBroker.nhsDigitalApiOrchestrationServiceMock.Invocations.Clear();
            string randomUrl = "https://cis2.example.com/authorize?" + GetRandomString();
            string expectedUrl = randomUrl;

            this.apiBroker.nhsDigitalApiOrchestrationServiceMock
                .Setup(service => service.BuildLoginUrlAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(randomUrl);

            // when
            HttpResponseMessage response = await this.apiBroker.GetLoginRedirectAsync();

            // then
            response.StatusCode.Should().Be(HttpStatusCode.Redirect);
            response.Headers.Location?.ToString().Should().Be(expectedUrl);

            this.apiBroker.nhsDigitalApiOrchestrationServiceMock.Verify(
                service => service.BuildLoginUrlAsync(It.IsAny<CancellationToken>()),
                Times.Once);

            this.apiBroker.nhsDigitalApiOrchestrationServiceMock.VerifyNoOtherCalls();
        }
    }
}
