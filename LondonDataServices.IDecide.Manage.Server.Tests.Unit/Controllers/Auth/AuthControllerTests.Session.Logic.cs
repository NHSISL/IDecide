// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Unit.Controllers.Auth
{
    public partial class AuthControllerTests
    {
        [Fact]
        public async Task ShouldReturnOkWithSessionInfoAsync()
        {
            // given
            string randomAccessToken = GetRandomString();
            string outputAccessToken = randomAccessToken;
            string randomName = GetRandomString();
            string randomRole = GetRandomString();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, randomName),
                new Claim(ClaimTypes.Role, randomRole)
            };

            var identity = new ClaimsIdentity(claims, authenticationType: "TestScheme");
            var principal = new ClaimsPrincipal(identity);

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(c => c.User).Returns(principal);

            this.authController.ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext.Object
            };

            this.nhsDigitalApiOrchestrationServiceMock
                .Setup(service => service.GetAccessTokenAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(outputAccessToken);

            // when
            IActionResult actualResult =
                await this.authController.Session(CancellationToken.None);

            // then
            var okResult = actualResult as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(200);

            this.nhsDigitalApiOrchestrationServiceMock.Verify(
                service => service.GetAccessTokenAsync(It.IsAny<CancellationToken>()),
                Times.Once);

            this.nhsDigitalApiOrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnUnauthorizedWhenAccessTokenIsNullOrWhiteSpaceAsync()
        {
            // given
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, GetRandomString())
            };

            var identity = new ClaimsIdentity(claims, authenticationType: "TestScheme");
            var principal = new ClaimsPrincipal(identity);

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(c => c.User).Returns(principal);

            this.authController.ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext.Object
            };

            this.nhsDigitalApiOrchestrationServiceMock
                .Setup(service => service.GetAccessTokenAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(string.Empty);

            // when
            IActionResult actualResult =
                await this.authController.Session(CancellationToken.None);

            // then
            actualResult.Should().BeOfType<UnauthorizedResult>();

            this.nhsDigitalApiOrchestrationServiceMock.Verify(
                service => service.GetAccessTokenAsync(It.IsAny<CancellationToken>()),
                Times.Once);

            this.nhsDigitalApiOrchestrationServiceMock.VerifyNoOtherCalls();
        }
    }
}
