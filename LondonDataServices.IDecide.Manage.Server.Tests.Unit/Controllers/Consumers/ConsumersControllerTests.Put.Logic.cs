// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using Force.DeepCloner;
using LondonDataServices.IDecide.Core.Models.Foundations.Consumers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Unit.Controllers.Consumers
{
    public partial class ConsumersControllerTests
    {
        [Fact]
        public async Task ShouldReturnOkOnPutAsync()
        {
            // given
            Consumer randomConsumer = CreateRandomConsumer();
            Consumer inputConsumer = randomConsumer;
            Consumer storageConsumer = inputConsumer.DeepClone();
            Consumer expectedConsumer = storageConsumer.DeepClone();

            var expectedObjectResult =
                new OkObjectResult(expectedConsumer);

            var expectedActionResult =
                new ActionResult<Consumer>(expectedObjectResult);

            consumerServiceMock
                .Setup(service => service.ModifyConsumerAsync(inputConsumer))
                    .ReturnsAsync(storageConsumer);

            // when
            ActionResult<Consumer> actualActionResult = await consumersController.PutConsumerAsync(randomConsumer);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            consumerServiceMock
               .Verify(service => service.ModifyConsumerAsync(inputConsumer),
                   Times.Once);

            consumerServiceMock.VerifyNoOtherCalls();
        }
    }
}
