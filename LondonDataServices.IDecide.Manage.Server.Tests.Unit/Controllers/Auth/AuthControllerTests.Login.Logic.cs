// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Unit.Controllers.Auth
{
    public partial class AuthControllerTests
    {
        [Fact]
        public async Task ShouldReturnRedirectOnLoginAsync()
        {
            // given
            string randomUrl = GetRandomString();
            string outputUrl = randomUrl;
            string expectedUrl = outputUrl;

            this.nhsDigitalApiOrchestrationServiceMock.Setup(service =>
                service.BuildLoginUrlAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(outputUrl);

            // when
            IActionResult actualResult =
                await this.authController.Login(CancellationToken.None);

            // then
            var redirectResult = actualResult as RedirectResult;
            redirectResult.Should().NotBeNull();
            redirectResult!.Url.Should().Be(expectedUrl);

            this.nhsDigitalApiOrchestrationServiceMock.Verify(service =>
                service.BuildLoginUrlAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.nhsDigitalApiOrchestrationServiceMock.VerifyNoOtherCalls();
        }
    }
}
