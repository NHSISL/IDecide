// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using LondonDataServices.IDecide.Core.Models.Foundations.Decisions;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Models;
using Xeptions;

namespace LondonDataServices.IDecide.Portal.Server.Tests.Unit.Controllers.PatientDecisions
{
    public partial class PatientDecisionControllerTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldReturnBadRequestOnPostPatientDecisionIfValidationErrorOccurredAsync(
            Xeption validationException)
        {
            // given
            string randomNhsNumber = GenerateRandom10DigitNumber();
            string randomValidationCode = GetRandomStringWithLengthOf(5);
            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();
            Patient randomPatient = GetRandomPatient(randomDateTime, randomNhsNumber, randomValidationCode);
            Decision randomDecision = GetRandomDecision(randomPatient);
            Decision inputDecision = randomDecision.DeepClone();

            BadRequestObjectResult expectedBadRequestObjectResult =
                BadRequest(validationException.InnerException);

            var expectedActionResult = expectedBadRequestObjectResult;

            this.decisionOrchestrationServiceMock.Setup(service =>
                service.VerifyAndRecordDecisionAsync(inputDecision))
                    .ThrowsAsync(validationException);

            // when
            ActionResult actualActionResult =
                await this.patientDecisionController.PostPatientDecisionAsync(inputDecision);

            // then
            actualActionResult.Should().BeEquivalentTo(expectedActionResult);

            this.decisionOrchestrationServiceMock.Verify(service =>
                service.VerifyAndRecordDecisionAsync(inputDecision),
                    Times.Once);

            this.decisionOrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnPostPatientDecisionIfServerErrorOccurredAsync(
            Xeption validationException)
        {
            // given
            string randomNhsNumber = GenerateRandom10DigitNumber();
            string randomValidationCode = GetRandomStringWithLengthOf(5);
            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();
            Patient randomPatient = GetRandomPatient(randomDateTime, randomNhsNumber, randomValidationCode);
            Decision randomDecision = GetRandomDecision(randomPatient);
            Decision inputDecision = randomDecision.DeepClone();

            InternalServerErrorObjectResult expectedInternalServerErrorObjectResult =
                InternalServerError(validationException);

            var expectedActionResult = expectedInternalServerErrorObjectResult;

            this.decisionOrchestrationServiceMock.Setup(service =>
                service.VerifyAndRecordDecisionAsync(inputDecision))
                    .ThrowsAsync(validationException);

            // when
            ActionResult actualActionResult =
                await this.patientDecisionController.PostPatientDecisionAsync(inputDecision);

            // then
            actualActionResult.Should().BeEquivalentTo(expectedActionResult);

            this.decisionOrchestrationServiceMock.Verify(service =>
                service.VerifyAndRecordDecisionAsync(inputDecision),
                    Times.Once);

            this.decisionOrchestrationServiceMock.VerifyNoOtherCalls();
        }
    }
}
