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
using RESTFulSense.Models;
using Xeptions;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Unit.Controllers.Auth
{
    public partial class AuthControllerTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldReturnBadRequestOnSessionIfValidationErrorOccurredAsync(
            Xeption validationException)
        {
            // given
            BadRequestObjectResult expectedBadRequestObjectResult =
                BadRequest(validationException.InnerException);

            var identity = new ClaimsIdentity(
                new List<Claim> { new Claim(ClaimTypes.Name, GetRandomString()) },
                authenticationType: "TestScheme");

            var principal = new ClaimsPrincipal(identity);
            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(c => c.User).Returns(principal);

            this.authController.ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext.Object
            };

            this.nhsDigitalApiOrchestrationServiceMock
                .Setup(service => service.GetAccessTokenAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(validationException);

            // when
            IActionResult actualResult =
                await this.authController.Session(CancellationToken.None);

            // then
            actualResult.Should().BeEquivalentTo(expectedBadRequestObjectResult);

            this.nhsDigitalApiOrchestrationServiceMock.Verify(
                service => service.GetAccessTokenAsync(It.IsAny<CancellationToken>()),
                Times.Once);

            this.nhsDigitalApiOrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnSessionIfServerErrorOccurredAsync(
            Xeption serverException)
        {
            // given
            InternalServerErrorObjectResult expectedInternalServerErrorObjectResult =
                InternalServerError(serverException);

            var identity = new ClaimsIdentity(
                new List<Claim> { new Claim(ClaimTypes.Name, GetRandomString()) },
                authenticationType: "TestScheme");

            var principal = new ClaimsPrincipal(identity);
            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(c => c.User).Returns(principal);

            this.authController.ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext.Object
            };

            this.nhsDigitalApiOrchestrationServiceMock
                .Setup(service => service.GetAccessTokenAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(serverException);

            // when
            IActionResult actualResult =
                await this.authController.Session(CancellationToken.None);

            // then
            actualResult.Should().BeEquivalentTo(expectedInternalServerErrorObjectResult);

            this.nhsDigitalApiOrchestrationServiceMock.Verify(
                service => service.GetAccessTokenAsync(It.IsAny<CancellationToken>()),
                Times.Once);

            this.nhsDigitalApiOrchestrationServiceMock.VerifyNoOtherCalls();
        }
    }
}
