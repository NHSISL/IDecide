// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Foundations.Consumers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
using RESTFulSense.Models;
using Xeptions;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Unit.Controllers.Consumers
{
    public partial class ConsumersControllerTests
    {
        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnGetIfServerErrorOccurredAsync(
            Xeption serverException)
        {
            // given
            IQueryable<Consumer> someConsumers = CreateRandomConsumers();

            InternalServerErrorObjectResult expectedInternalServerErrorObjectResult =
                InternalServerError(serverException);

            var expectedActionResult =
                new ActionResult<IQueryable<Consumer>>(expectedInternalServerErrorObjectResult);

            this.consumerServiceMock.Setup(service =>
                service.RetrieveAllConsumersAsync())
                    .ThrowsAsync(serverException);

            // when
            ActionResult<IQueryable<Consumer>> actualActionResult =
                await this.consumersController.Get();

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.consumerServiceMock.Verify(service =>
                service.RetrieveAllConsumersAsync(),
                    Times.Once);

            this.consumerServiceMock.VerifyNoOtherCalls();
        }
    }
}
