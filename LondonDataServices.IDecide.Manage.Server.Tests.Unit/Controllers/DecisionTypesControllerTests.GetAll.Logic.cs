// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Linq;
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
        public async Task ShouldReturnRecordsOnGetAsync()
        {
            // given
            IQueryable<DecisionType> randomDecisionTypes = CreateRandomDecisionTypes();
            IQueryable<DecisionType> storageDecisionTypes = randomDecisionTypes.DeepClone();
            IQueryable<DecisionType> expectedDecisionType = storageDecisionTypes.DeepClone();

            var expectedObjectResult =
                new OkObjectResult(expectedDecisionType);

            var expectedActionResult =
                new ActionResult<IQueryable<DecisionType>>(expectedObjectResult);

            decisionTypeServiceMock
                .Setup(service => service.RetrieveAllDecisionTypesAsync())
                    .ReturnsAsync(storageDecisionTypes);

            // when
            ActionResult<IQueryable<DecisionType>> actualActionResult = await decisionTypesController.Get();

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            decisionTypeServiceMock
               .Verify(service => service.RetrieveAllDecisionTypesAsync(),
                   Times.Once);

            decisionTypeServiceMock.VerifyNoOtherCalls();
        }
    }
}
