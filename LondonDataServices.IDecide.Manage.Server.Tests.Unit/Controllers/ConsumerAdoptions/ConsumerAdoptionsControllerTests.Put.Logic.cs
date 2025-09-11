// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using Force.DeepCloner;
using LondonDataServices.IDecide.Core.Models.Foundations.ConsumerAdoptions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Unit.Controllers.ConsumerAdoptions
{
    public partial class ConsumerAdoptionsControllerTests
    {
        [Fact]
        public async Task ShouldReturnOkOnPutAsync()
        {
            // given
            ConsumerAdoption randomConsumerAdoption = CreateRandomConsumerAdoption();
            ConsumerAdoption inputConsumerAdoption = randomConsumerAdoption;
            ConsumerAdoption storageConsumerAdoption = inputConsumerAdoption.DeepClone();
            ConsumerAdoption expectedConsumerAdoption = storageConsumerAdoption.DeepClone();

            var expectedObjectResult =
                new OkObjectResult(expectedConsumerAdoption);

            var expectedActionResult =
                new ActionResult<ConsumerAdoption>(expectedObjectResult);

            consumerAdoptionServiceMock
                .Setup(service => service.ModifyConsumerAdoptionAsync(inputConsumerAdoption))
                    .ReturnsAsync(storageConsumerAdoption);

            // when
            ActionResult<ConsumerAdoption> actualActionResult = await consumerAdoptionsController.PutConsumerAdoptionAsync(randomConsumerAdoption);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            consumerAdoptionServiceMock
               .Verify(service => service.ModifyConsumerAdoptionAsync(inputConsumerAdoption),
                   Times.Once);

            consumerAdoptionServiceMock.VerifyNoOtherCalls();
        }
    }
}
