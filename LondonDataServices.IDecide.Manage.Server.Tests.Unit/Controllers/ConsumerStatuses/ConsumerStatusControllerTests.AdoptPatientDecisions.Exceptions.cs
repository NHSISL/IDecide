// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using FluentAssertions;
using Force.DeepCloner;
using LondonDataServices.IDecide.Core.Models.Foundations.Decisions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xeptions;
using Task = System.Threading.Tasks.Task;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Unit.Controllers.ConsumerStatuses
{
    public partial class ConsumerStatusControllerTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldReturnBadRequestOnAdoptPatientDecisionsIfValidationErrorOccurredAsync(
            Xeption validationException)
        {
            // given
            List<Decision> randomDecisions = GetRandomDecisions();
            List<Decision> inputDecisions = randomDecisions.DeepClone();

            BadRequestObjectResult expectedBadRequestObjectResult =
                BadRequest(validationException.InnerException);

            var expectedActionResult = expectedBadRequestObjectResult;

            this.consumerOrchestrationServiceMock.Setup(service =>
                service.AdoptPatientDecisions(inputDecisions))
                    .ThrowsAsync(validationException);

            // when
            ActionResult actualActionResult =
                await this.consumerStatusController.AdoptPatientDecisionsAsync(inputDecisions);

            // then
            actualActionResult.Should().BeEquivalentTo(expectedActionResult);

            this.consumerOrchestrationServiceMock.Verify(service =>
                    service.AdoptPatientDecisions(inputDecisions),
                Times.Once);

            this.consumerOrchestrationServiceMock.VerifyNoOtherCalls();
        }
    }
}
