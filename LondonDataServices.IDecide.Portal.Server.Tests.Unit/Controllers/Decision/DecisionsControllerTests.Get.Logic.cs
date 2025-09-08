// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
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
        public async Task ShouldReturnRecordOnGetByIdsAsync()
        {
            // given
            Decision randomDecision = CreateRandomDecision();
            Guid inputId = randomDecision.Id;
            Decision storageDecision = randomDecision;
            Decision expectedDecision = storageDecision.DeepClone();

            var expectedObjectResult =
                new OkObjectResult(expectedDecision);

            var expectedActionResult =
                new ActionResult<Decision>(expectedObjectResult);

            decisionServiceMock
                .Setup(service => service.RetrieveDecisionByIdAsync(It.IsAny<Guid>()))
                    .ReturnsAsync(storageDecision);

            // when
            ActionResult<Decision> actualActionResult = await decisionsController.GetDecisionByIdAsync(inputId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            decisionServiceMock
                .Verify(service => service.RetrieveDecisionByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            decisionServiceMock.VerifyNoOtherCalls();
        }
    }
}
