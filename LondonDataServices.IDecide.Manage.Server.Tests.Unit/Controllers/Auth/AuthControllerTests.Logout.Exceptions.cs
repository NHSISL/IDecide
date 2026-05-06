// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
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
        public async Task ShouldReturnBadRequestOnLogoutIfValidationErrorOccurredAsync(
            Xeption validationException)
        {
            // given
            BadRequestObjectResult expectedBadRequestObjectResult =
                BadRequest(validationException.InnerException);

            this.nhsDigitalApiOrchestrationServiceMock
                .Setup(service => service.LogoutAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(validationException);

            // when
            IActionResult actualResult =
                await this.authController.Logout(CancellationToken.None);

            // then
            actualResult.Should().BeEquivalentTo(expectedBadRequestObjectResult);

            this.nhsDigitalApiOrchestrationServiceMock.Verify(
                service => service.LogoutAsync(It.IsAny<CancellationToken>()),
                Times.Once);

            this.nhsDigitalApiOrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnLogoutIfServerErrorOccurredAsync(
            Xeption serverException)
        {
            // given
            InternalServerErrorObjectResult expectedInternalServerErrorObjectResult =
                InternalServerError(serverException);

            this.nhsDigitalApiOrchestrationServiceMock
                .Setup(service => service.LogoutAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(serverException);

            // when
            IActionResult actualResult =
                await this.authController.Logout(CancellationToken.None);

            // then
            actualResult.Should().BeEquivalentTo(expectedInternalServerErrorObjectResult);

            this.nhsDigitalApiOrchestrationServiceMock.Verify(
                service => service.LogoutAsync(It.IsAny<CancellationToken>()),
                Times.Once);

            this.nhsDigitalApiOrchestrationServiceMock.VerifyNoOtherCalls();
        }
    }
}
