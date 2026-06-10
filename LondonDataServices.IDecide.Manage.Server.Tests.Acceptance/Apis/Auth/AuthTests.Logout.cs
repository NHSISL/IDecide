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
        public async Task ShouldRedirectToRootOnLogoutAsync()
        {
            // given
            this.apiBroker.nhsDigitalApiOrchestrationServiceMock
                .Setup(service => service.LogoutAsync(It.IsAny<CancellationToken>()))
                .Returns(ValueTask.CompletedTask);

            // when
            HttpResponseMessage response = await this.apiBroker.PostLogoutAsync();

            // then
            response.StatusCode.Should().Be(HttpStatusCode.Redirect);
            response.Headers.Location?.ToString().Should().Be("/");
        }
    }
}
