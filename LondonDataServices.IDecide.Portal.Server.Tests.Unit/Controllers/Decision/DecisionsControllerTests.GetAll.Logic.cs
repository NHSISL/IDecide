// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using Force.DeepCloner;
using LondonDataServices.IDecide.Core.Models.Foundations.Decisions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;

namespace LondonDataServices.IDecide.Portal.Server.Tests.Unit.Controllers.Decisions
{
    public partial class DecisionsControllerTests
    {
        [Fact]
        public async Task ShouldReturnRecordsOnGetAsync()
        {
            // given
            IQueryable<Decision> randomDecisions = CreateRandomDecisions();
            IQueryable<Decision> storageDecisions = randomDecisions.DeepClone();
            IQueryable<Decision> expectedDecision = storageDecisions.DeepClone();

            var expectedObjectResult =
                new OkObjectResult(expectedDecision);

            var expectedActionResult =
                new ActionResult<IQueryable<Decision>>(expectedObjectResult);

            decisionServiceMock
                .Setup(service => service.RetrieveAllDecisionsAsync())
                    .ReturnsAsync(storageDecisions);

            // when
            ActionResult<IQueryable<Decision>> actualActionResult = await decisionsController.Get();

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            decisionServiceMock
               .Verify(service => service.RetrieveAllDecisionsAsync(),
                   Times.Once);

            decisionServiceMock.VerifyNoOtherCalls();
        }
    }
}
