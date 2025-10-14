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
using Xeptions;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Unit.Controllers.PatientDecisions
{
    public partial class PatientDecisionControllerTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldReturnBadRequestOnGetPatientDecisionsIfValidationErrorOccurredAsync(
            Xeption validationException)
        {
            // given
            DateTimeOffset? randomDateTimeOffset = GetRandomDateTimeOffset();
            DateTimeOffset? inputFrom = randomDateTimeOffset;
            string randomString = GetRandomString();
            string decisionType = randomString;

            BadRequestObjectResult expectedBadRequestObjectResult =
                BadRequest(validationException.InnerException);

            var expectedActionResult = new ActionResult<List<Decision>>(expectedBadRequestObjectResult);

            this.decisionOrchestrationServiceMock.Setup(service =>
                service.RetrieveAllPendingAdoptionDecisionsForConsumer(inputFrom.Value, decisionType))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<List<Decision>> actualActionResult =
                await this.patientDecisionController.GetPatientDecisionsAsync(
                    from: inputFrom,
                    decisionType: decisionType);

            // then
            actualActionResult.Should().BeEquivalentTo(expectedActionResult);

            this.decisionOrchestrationServiceMock.Verify(service =>
                service.RetrieveAllPendingAdoptionDecisionsForConsumer(inputFrom.Value, decisionType),
                    Times.Once);

            this.decisionOrchestrationServiceMock.VerifyNoOtherCalls();
        }
    }
}
