// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
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
        public async Task ShouldReturnRecordOnGetByIdsAsync()
        {
            // given
            ConsumerAdoption randomConsumerAdoption = CreateRandomConsumerAdoption();
            Guid inputId = randomConsumerAdoption.Id;
            ConsumerAdoption storageConsumerAdoption = randomConsumerAdoption;
            ConsumerAdoption expectedConsumerAdoption = storageConsumerAdoption.DeepClone();

            var expectedObjectResult =
                new OkObjectResult(expectedConsumerAdoption);

            var expectedActionResult =
                new ActionResult<ConsumerAdoption>(expectedObjectResult);

            consumerAdoptionServiceMock
                .Setup(service => service.RetrieveConsumerAdoptionByIdAsync(It.IsAny<Guid>()))
                    .ReturnsAsync(storageConsumerAdoption);

            // when
            ActionResult<ConsumerAdoption> actualActionResult = await consumerAdoptionsController.GetConsumerAdoptionByIdAsync(inputId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            consumerAdoptionServiceMock
                .Verify(service => service.RetrieveConsumerAdoptionByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.consumerAdoptionServiceMock.VerifyNoOtherCalls();
            this.consumerOrchestrationServiceMock.VerifyNoOtherCalls();
        }
    }
}
