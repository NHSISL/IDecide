// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Core.Models.Foundations.Decisions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace LondonDataServices.IDecide.Portal.Server.Tests.Unit.Controllers.PatientDecisions
{
    public partial class PatientDecisionControllerTests
    {
        [Fact]
        public async Task ShouldReturnDecisionsOnGetPatientDecisionsAsync()
        {
            DateTimeOffset? randomDateTimeOffset = GetRandomDateTimeOffset();
            DateTimeOffset? inputFrom = randomDateTimeOffset;
            string randomString = GetRandomString();
            string decisionType = randomString;
            List<Decision> randomDecisions = GetRandomDecisions();
            List<Decision> expectedDecisions = randomDecisions;
            var expectedObjectResult = new OkObjectResult(expectedDecisions);
            var expectedActionResult = new ActionResult<List<Decision>>(expectedObjectResult);

            this.decisionOrchestrationServiceMock.Setup(orchestration =>
                orchestration.RetrieveAllPendingAdoptionDecisionsForConsumer(
                    inputFrom.Value, decisionType))
                    .ReturnsAsync(expectedDecisions);

            // when
            ActionResult<List<Decision>> actualActionResult =
                await this.patientDecisionController.GetPatientDecisionsAsync(
                    from: inputFrom,
                    decisionType: decisionType);

            // then
            actualActionResult.Result.Should().BeOfType<OkObjectResult>();
            actualActionResult.Should().BeEquivalentTo(expectedActionResult);
            var okResult = actualActionResult.Result as OkObjectResult;
            okResult.Value.Should().BeEquivalentTo(expectedDecisions);

            this.decisionOrchestrationServiceMock.Verify(orchestration =>
                orchestration.RetrieveAllPendingAdoptionDecisionsForConsumer(inputFrom.Value, decisionType),
                    Times.Once);

            this.decisionOrchestrationServiceMock.VerifyNoOtherCalls();
        }
    }
}
