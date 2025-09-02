// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using Force.DeepCloner;
using LondonDataServices.IDecide.Core.Models.Foundations.Decisions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
using RESTFulSense.Models;

namespace LondonDataServices.IDecide.Portal.Server.Tests.Unit.Controllers.Decisions
{
    public partial class DecisionsControllerTests
    {
        [Fact]
        public async Task ShouldReturnCreatedOnPostAsync()
        {
            // given
            Decision randomDecision = CreateRandomDecision();
            Decision inputDecision = randomDecision;
            Decision addedDecision = inputDecision.DeepClone();
            Decision expectedDecision = addedDecision.DeepClone();

            var expectedObjectResult =
                new CreatedObjectResult(expectedDecision);

            var expectedActionResult =
                new ActionResult<Decision>(expectedObjectResult);

            decisionServiceMock
                .Setup(service => service.AddDecisionAsync(inputDecision))
                    .ReturnsAsync(addedDecision);

            // when
            ActionResult<Decision> actualActionResult = await decisionsController.PostDecisionAsync(randomDecision);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            decisionServiceMock
               .Verify(service => service.AddDecisionAsync(inputDecision),
                   Times.Once);

            decisionServiceMock.VerifyNoOtherCalls();
        }
    }
}