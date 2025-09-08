// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
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
        public async Task ShouldRemoveRecordOnDeleteByIdsAsync()
        {
            // given
            Consumer randomConsumer = CreateRandomConsumer();
            Guid inputId = randomConsumer.Id;
            Consumer storageConsumer = randomConsumer;
            Consumer expectedConsumer = storageConsumer.DeepClone();

            var expectedObjectResult =
                new OkObjectResult(expectedConsumer);

            var expectedActionResult =
                new ActionResult<Consumer>(expectedObjectResult);

            consumerServiceMock
                .Setup(service => service.RemoveConsumerByIdAsync(It.IsAny<Guid>()))
                    .ReturnsAsync(storageConsumer);

            // when
            ActionResult<Consumer> actualActionResult = await consumersController.DeleteConsumerByIdAsync(inputId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            consumerServiceMock
                .Verify(service => service.RemoveConsumerByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            consumerServiceMock.VerifyNoOtherCalls();
        }
    }
}
