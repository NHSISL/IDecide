// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
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
        public async Task ShouldRemoveRecordOnDeleteByIdsAsync()
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
                .Setup(service => service.RemoveDecisionByIdAsync(It.IsAny<Guid>()))
                    .ReturnsAsync(storageDecision);

            // when
            ActionResult<Decision> actualActionResult = await decisionsController.DeleteDecisionByIdAsync(inputId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            decisionServiceMock
                .Verify(service => service.RemoveDecisionByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            decisionServiceMock.VerifyNoOtherCalls();
        }
    }
}
