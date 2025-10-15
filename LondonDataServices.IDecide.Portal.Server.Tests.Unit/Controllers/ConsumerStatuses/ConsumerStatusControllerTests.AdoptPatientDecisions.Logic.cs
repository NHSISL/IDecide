// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using LondonDataServices.IDecide.Core.Models.Foundations.Decisions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace LondonDataServices.IDecide.Portal.Server.Tests.Unit.Controllers.ConsumerStatuses
{
    public partial class ConsumerStatusControllerTests
    {
        [Fact]
        public async Task ShouldReturnOkOnAdoptPatientDecisionsAsync()
        {
            // given
            List<Decision> randomDecisions = GetRandomDecisions();
            List<Decision> inputDecisions = randomDecisions.DeepClone();
            var expectedResult = new OkResult();
            var expectedActionResult = expectedResult;

            this.consumerOrchestrationServiceMock.Setup(orchestration =>
                orchestration.AdoptPatientDecisions(inputDecisions))
                    .Returns(ValueTask.CompletedTask);

            // when
            ActionResult actualActionResult =
                await this.consumerStatusController.AdoptPatientDecisionsAsync(inputDecisions);

            // then
            actualActionResult.Should().BeEquivalentTo(expectedActionResult);

            this.consumerOrchestrationServiceMock.Verify(orchestration =>
                orchestration.AdoptPatientDecisions(inputDecisions),
                    Times.Once);

            this.consumerOrchestrationServiceMock.VerifyNoOtherCalls();
        }
    }
}
