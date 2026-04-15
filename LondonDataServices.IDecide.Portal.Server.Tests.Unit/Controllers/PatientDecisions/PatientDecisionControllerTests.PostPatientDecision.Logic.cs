// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using LondonDataServices.IDecide.Core.Models.Foundations.Decisions;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace LondonDataServices.IDecide.Portal.Server.Tests.Unit.Controllers.PatientDecisions
{
    public partial class PatientDecisionControllerTests
    {
        [Fact]
        public async Task ShouldReturnOkOnPostPatientDecisionAsync()
        {
            // given
            string randomNhsNumber = GenerateRandom10DigitNumber();
            string randomValidationCode = GetRandomStringWithLengthOf(5);
            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();
            Patient randomPatient = GetRandomPatient(randomDateTime, randomNhsNumber, randomValidationCode);
            Decision randomDecision = GetRandomDecision(randomPatient);
            Decision inputDecision = randomDecision.DeepClone();
            var expectedResult = new OkResult();
            var expectedActionResult = expectedResult;

            var user = new ClaimsPrincipal(
                new ClaimsIdentity(new[]
                {
                    new Claim("given_name", "TestGivenName"),
                    new Claim("surname", "TestSurname")
                }));

            this.patientDecisionController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // when
            ActionResult actualActionResult = await this.patientDecisionController
                .PostPatientDecisionAsync(inputDecision);

            // then
            actualActionResult.Should().BeEquivalentTo(expectedActionResult);

            // Assert that the claims were copied to the decision
            inputDecision.ResponsiblePersonGivenName.Should().Be("TestGivenName");
            inputDecision.ResponsiblePersonSurname.Should().Be("TestSurname");

            this.decisionOrchestrationServiceMock.Verify(service =>
                service.VerifyAndRecordDecisionAsync(inputDecision),
                   Times.Once);

            this.decisionOrchestrationServiceMock.VerifyNoOtherCalls();
        }
    }
}
