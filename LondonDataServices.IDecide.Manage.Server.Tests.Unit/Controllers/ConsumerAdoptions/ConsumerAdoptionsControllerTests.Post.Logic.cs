// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using Force.DeepCloner;
using LondonDataServices.IDecide.Core.Models.Foundations.ConsumerAdoptions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
using RESTFulSense.Models;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Unit.Controllers.ConsumerAdoptions
{
    public partial class ConsumerAdoptionsControllerTests
    {
        [Fact]
        public async Task ShouldReturnCreatedOnPostAsync()
        {
            // given
            ConsumerAdoption randomConsumerAdoption = CreateRandomConsumerAdoption();
            ConsumerAdoption inputConsumerAdoption = randomConsumerAdoption;
            ConsumerAdoption addedConsumerAdoption = inputConsumerAdoption.DeepClone();
            ConsumerAdoption expectedConsumerAdoption = addedConsumerAdoption.DeepClone();

            var expectedObjectResult =
                new CreatedObjectResult(expectedConsumerAdoption);

            var expectedActionResult =
                new ActionResult<ConsumerAdoption>(expectedObjectResult);

            consumerAdoptionServiceMock
                .Setup(service => service.AddConsumerAdoptionAsync(inputConsumerAdoption))
                    .ReturnsAsync(addedConsumerAdoption);

            // when
            ActionResult<ConsumerAdoption> actualActionResult = await consumerAdoptionsController.PostConsumerAdoptionAsync(randomConsumerAdoption);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            consumerAdoptionServiceMock
               .Verify(service => service.AddConsumerAdoptionAsync(inputConsumerAdoption),
                   Times.Once);

            this.consumerAdoptionServiceMock.VerifyNoOtherCalls();
            this.consumerOrchestrationServiceMock.VerifyNoOtherCalls();
        }
    }
}