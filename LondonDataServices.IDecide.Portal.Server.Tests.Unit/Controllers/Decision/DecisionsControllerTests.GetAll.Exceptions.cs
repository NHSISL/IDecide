// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Foundations.Decisions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
using RESTFulSense.Models;
using Xeptions;

namespace LondonDataServices.IDecide.Portal.Server.Tests.Unit.Controllers.Decisions
{
    public partial class DecisionsControllerTests
    {
        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnGetIfServerErrorOccurredAsync(
            Xeption serverException)
        {
            // given
            IQueryable<Decision> someDecisions = CreateRandomDecisions();

            InternalServerErrorObjectResult expectedInternalServerErrorObjectResult =
                InternalServerError(serverException);

            var expectedActionResult =
                new ActionResult<IQueryable<Decision>>(expectedInternalServerErrorObjectResult);

            this.decisionServiceMock.Setup(service =>
                service.RetrieveAllDecisionsAsync())
                    .ThrowsAsync(serverException);

            // when
            ActionResult<IQueryable<Decision>> actualActionResult =
                await this.decisionsController.Get();

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.decisionServiceMock.Verify(service =>
                service.RetrieveAllDecisionsAsync(),
                    Times.Once);

            this.decisionServiceMock.VerifyNoOtherCalls();
        }
    }
}
