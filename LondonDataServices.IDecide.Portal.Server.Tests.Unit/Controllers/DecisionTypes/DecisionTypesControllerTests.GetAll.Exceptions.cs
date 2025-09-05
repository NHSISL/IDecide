// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Foundations.DecisionTypes;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
using RESTFulSense.Models;
using Xeptions;

namespace LondonDataServices.IDecide.Portal.Server.Tests.Unit.Controllers.DecisionTypes
{
    public partial class DecisionTypesControllerTests
    {
        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnGetIfServerErrorOccurredAsync(
            Xeption serverException)
        {
            // given
            IQueryable<DecisionType> someDecisionTypes = CreateRandomDecisionTypes();

            InternalServerErrorObjectResult expectedInternalServerErrorObjectResult =
                InternalServerError(serverException);

            var expectedActionResult =
                new ActionResult<IQueryable<DecisionType>>(expectedInternalServerErrorObjectResult);

            this.decisionTypeServiceMock.Setup(service =>
                service.RetrieveAllDecisionTypesAsync())
                    .ThrowsAsync(serverException);

            // when
            ActionResult<IQueryable<DecisionType>> actualActionResult =
                await this.decisionTypesController.Get();

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.decisionTypeServiceMock.Verify(service =>
                service.RetrieveAllDecisionTypesAsync(),
                    Times.Once);

            this.decisionTypeServiceMock.VerifyNoOtherCalls();
        }
    }
}
