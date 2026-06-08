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
        public async Task ShouldReturnOkWithSessionInfoAsync()
        {
            // given
            string randomAccessToken = GetRandomString();

            this.apiBroker.nhsDigitalApiOrchestrationServiceMock
                .Setup(service => service.GetAccessTokenAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(randomAccessToken);

            // when
            HttpResponseMessage response = await this.apiBroker.GetSessionAsync();

            // then
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task ShouldReturnUnauthorizedWhenAccessTokenIsEmptyAsync()
        {
            // given
            this.apiBroker.nhsDigitalApiOrchestrationServiceMock
                .Setup(service => service.GetAccessTokenAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(string.Empty);

            // when
            HttpResponseMessage response = await this.apiBroker.GetSessionAsync();

            // then
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}
