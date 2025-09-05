// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Linq;
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
        public async Task ShouldReturnRecordsOnGetAsync()
        {
            // given
            IQueryable<Consumer> randomConsumers = CreateRandomConsumers();
            IQueryable<Consumer> storageConsumers = randomConsumers.DeepClone();
            IQueryable<Consumer> expectedConsumer = storageConsumers.DeepClone();

            var expectedObjectResult =
                new OkObjectResult(expectedConsumer);

            var expectedActionResult =
                new ActionResult<IQueryable<Consumer>>(expectedObjectResult);

            consumerServiceMock
                .Setup(service => service.RetrieveAllConsumersAsync())
                    .ReturnsAsync(storageConsumers);

            // when
            ActionResult<IQueryable<Consumer>> actualActionResult = await consumersController.Get();

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            consumerServiceMock
               .Verify(service => service.RetrieveAllConsumersAsync(),
                   Times.Once);

            consumerServiceMock.VerifyNoOtherCalls();
        }
    }
}
