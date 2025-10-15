// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Linq;
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
        public async Task ShouldReturnRecordsOnGetAsync()
        {
            // given
            IQueryable<ConsumerAdoption> randomConsumerAdoptions = CreateRandomConsumerAdoptions();
            IQueryable<ConsumerAdoption> storageConsumerAdoptions = randomConsumerAdoptions.DeepClone();
            IQueryable<ConsumerAdoption> expectedConsumerAdoption = storageConsumerAdoptions.DeepClone();

            var expectedObjectResult =
                new OkObjectResult(expectedConsumerAdoption);

            var expectedActionResult =
                new ActionResult<IQueryable<ConsumerAdoption>>(expectedObjectResult);

            consumerAdoptionServiceMock
                .Setup(service => service.RetrieveAllConsumerAdoptionsAsync())
                    .ReturnsAsync(storageConsumerAdoptions);

            // when
            ActionResult<IQueryable<ConsumerAdoption>> actualActionResult = await consumerAdoptionsController.Get();

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            consumerAdoptionServiceMock
               .Verify(service => service.RetrieveAllConsumerAdoptionsAsync(),
                   Times.Once);

            this.consumerAdoptionServiceMock.VerifyNoOtherCalls();
            this.consumerOrchestrationServiceMock.VerifyNoOtherCalls();
        }
    }
}
