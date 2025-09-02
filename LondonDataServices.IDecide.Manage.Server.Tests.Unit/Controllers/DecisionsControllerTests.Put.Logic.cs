// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using Force.DeepCloner;
using LondonDataServices.IDecide.Manage.Server.Models.Foundations.Decisions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Unit.Controllers.Decisions
{
    public partial class DecisionsControllerTests
    {
        [Fact]
        public async Task ShouldReturnOkOnPutAsync()
        {
            // given
            Decision randomDecision = CreateRandomDecision();
            Decision inputDecision = randomDecision;
            Decision storageDecision = inputDecision.DeepClone();
            Decision expectedDecision = storageDecision.DeepClone();

            var expectedObjectResult =
                new OkObjectResult(expectedDecision);

            var expectedActionResult =
                new ActionResult<Decision>(expectedObjectResult);

            decisionServiceMock
                .Setup(service => service.ModifyDecisionAsync(inputDecision))
                    .ReturnsAsync(storageDecision);

            // when
            ActionResult<Decision> actualActionResult = await decisionsController.PutDecisionAsync(randomDecision);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            decisionServiceMock
               .Verify(service => service.ModifyDecisionAsync(inputDecision),
                   Times.Once);

            decisionServiceMock.VerifyNoOtherCalls();
        }
    }
}
