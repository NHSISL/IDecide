// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using Force.DeepCloner;
using LondonDataServices.IDecide.Core.Models.Foundations.DecisionTypes;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
using RESTFulSense.Models;

namespace LondonDataServices.IDecide.Portal.Server.Tests.Unit.Controllers.DecisionTypes
{
    public partial class DecisionTypesControllerTests
    {
        [Fact]
        public async Task ShouldReturnCreatedOnPostAsync()
        {
            // given
            DecisionType randomDecisionType = CreateRandomDecisionType();
            DecisionType inputDecisionType = randomDecisionType;
            DecisionType addedDecisionType = inputDecisionType.DeepClone();
            DecisionType expectedDecisionType = addedDecisionType.DeepClone();

            var expectedObjectResult =
                new CreatedObjectResult(expectedDecisionType);

            var expectedActionResult =
                new ActionResult<DecisionType>(expectedObjectResult);

            decisionTypeServiceMock
                .Setup(service => service.AddDecisionTypeAsync(inputDecisionType))
                    .ReturnsAsync(addedDecisionType);

            // when
            ActionResult<DecisionType> actualActionResult = 
                await decisionTypesController.PostDecisionTypeAsync(randomDecisionType);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            decisionTypeServiceMock
               .Verify(service => service.AddDecisionTypeAsync(inputDecisionType),
                   Times.Once);

            decisionTypeServiceMock.VerifyNoOtherCalls();
        }
    }
}