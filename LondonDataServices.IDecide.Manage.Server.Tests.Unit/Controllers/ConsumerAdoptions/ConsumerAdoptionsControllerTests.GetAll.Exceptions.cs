// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Foundations.ConsumerAdoptions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
using RESTFulSense.Models;
using Xeptions;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Unit.Controllers.ConsumerAdoptions
{
    public partial class ConsumerAdoptionsControllerTests
    {
        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnGetIfServerErrorOccurredAsync(
            Xeption serverException)
        {
            // given
            IQueryable<ConsumerAdoption> someConsumerAdoptions = CreateRandomConsumerAdoptions();

            InternalServerErrorObjectResult expectedInternalServerErrorObjectResult =
                InternalServerError(serverException);

            var expectedActionResult =
                new ActionResult<IQueryable<ConsumerAdoption>>(expectedInternalServerErrorObjectResult);

            this.consumerAdoptionServiceMock.Setup(service =>
                service.RetrieveAllConsumerAdoptionsAsync())
                    .ThrowsAsync(serverException);

            // when
            ActionResult<IQueryable<ConsumerAdoption>> actualActionResult =
                await this.consumerAdoptionsController.Get();

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.consumerAdoptionServiceMock.Verify(service =>
                service.RetrieveAllConsumerAdoptionsAsync(),
                    Times.Once);

            this.consumerAdoptionServiceMock.VerifyNoOtherCalls();
            this.consumerOrchestrationServiceMock.VerifyNoOtherCalls();
        }
    }
}
