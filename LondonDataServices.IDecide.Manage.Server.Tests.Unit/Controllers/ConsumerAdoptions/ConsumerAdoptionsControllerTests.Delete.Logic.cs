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
        public async Task ShouldRemoveRecordOnDeleteByIdsAsync()
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
                .Setup(service => service.RemoveConsumerAdoptionByIdAsync(It.IsAny<Guid>()))
                    .ReturnsAsync(storageConsumerAdoption);

            // when
            ActionResult<ConsumerAdoption> actualActionResult = await consumerAdoptionsController.DeleteConsumerAdoptionByIdAsync(inputId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            consumerAdoptionServiceMock
                .Verify(service => service.RemoveConsumerAdoptionByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            consumerAdoptionServiceMock.VerifyNoOtherCalls();
        }
    }
}
