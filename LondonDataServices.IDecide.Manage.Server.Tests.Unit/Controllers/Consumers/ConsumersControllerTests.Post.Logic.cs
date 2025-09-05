// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using Force.DeepCloner;
using LondonDataServices.IDecide.Core.Models.Foundations.Consumers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
using RESTFulSense.Models;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Unit.Controllers.Consumers
{
    public partial class ConsumersControllerTests
    {
        [Fact]
        public async Task ShouldReturnCreatedOnPostAsync()
        {
            // given
            Consumer randomConsumer = CreateRandomConsumer();
            Consumer inputConsumer = randomConsumer;
            Consumer addedConsumer = inputConsumer.DeepClone();
            Consumer expectedConsumer = addedConsumer.DeepClone();

            var expectedObjectResult =
                new CreatedObjectResult(expectedConsumer);

            var expectedActionResult =
                new ActionResult<Consumer>(expectedObjectResult);

            consumerServiceMock
                .Setup(service => service.AddConsumerAsync(inputConsumer))
                    .ReturnsAsync(addedConsumer);

            // when
            ActionResult<Consumer> actualActionResult = await consumersController.PostConsumerAsync(randomConsumer);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            consumerServiceMock
               .Verify(service => service.AddConsumerAsync(inputConsumer),
                   Times.Once);

            consumerServiceMock.VerifyNoOtherCalls();
        }
    }
}