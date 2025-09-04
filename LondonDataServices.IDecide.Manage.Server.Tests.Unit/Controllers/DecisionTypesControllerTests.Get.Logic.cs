// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using Force.DeepCloner;
using LondonDataServices.IDecide.Core.Models.Foundations.DecisionTypes;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Unit.Controllers.DecisionTypes
{
    public partial class DecisionTypesControllerTests
    {
        [Fact]
        public async Task ShouldReturnRecordOnGetByIdsAsync()
        {
            // given
            DecisionType randomDecisionType = CreateRandomDecisionType();
            Guid inputId = randomDecisionType.Id;
            DecisionType storageDecisionType = randomDecisionType;
            DecisionType expectedDecisionType = storageDecisionType.DeepClone();

            var expectedObjectResult =
                new OkObjectResult(expectedDecisionType);

            var expectedActionResult =
                new ActionResult<DecisionType>(expectedObjectResult);

            decisionTypeServiceMock
                .Setup(service => service.RetrieveDecisionTypeByIdAsync(It.IsAny<Guid>()))
                    .ReturnsAsync(storageDecisionType);

            // when
            ActionResult<DecisionType> actualActionResult = await decisionTypesController.GetDecisionTypeByIdAsync(inputId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            decisionTypeServiceMock
                .Verify(service => service.RetrieveDecisionTypeByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            decisionTypeServiceMock.VerifyNoOtherCalls();
        }
    }
}
