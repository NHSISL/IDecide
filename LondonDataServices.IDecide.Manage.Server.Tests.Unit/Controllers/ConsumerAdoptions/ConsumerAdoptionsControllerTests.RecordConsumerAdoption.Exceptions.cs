// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Models;
using Xeptions;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Unit.Controllers.ConsumerAdoptions
{
    public partial class ConsumerAdoptionsControllerTests
    {
        [Theory]
        [MemberData(nameof(OrchestrationValidationExceptions))]
        public async Task ShouldReturnBadRequestOnRecordConsumerAdoptionIfValidationErrorOccurredAsync(
            Xeption validationException)
        {
            // given
            List<Guid> someGuids = GetRandomGuids();

            BadRequestObjectResult expectedBadRequestObjectResult =
                 BadRequest(validationException.InnerException);

            var expectedActionResult = expectedBadRequestObjectResult;

            this.consumerOrchestrationServiceMock.Setup(service =>
                service.RecordConsumerAdoptionAsync(It.IsAny<List<Guid>>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult actualActionResult =
                await this.consumerAdoptionsController.RecordConsumerAdoptionAsync(someGuids);

            // then
            actualActionResult.Should().BeEquivalentTo(expectedActionResult);

            this.consumerOrchestrationServiceMock.Verify(service =>
                service.RecordConsumerAdoptionAsync(It.IsAny<List<Guid>>()),
                    Times.Once);

            this.consumerAdoptionServiceMock.VerifyNoOtherCalls();
            this.consumerOrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(OrchestrationServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnRecordConsumerAdoptionIfServerErrorOccurredAsync(
            Xeption serverException)
        {
            // given
            List<Guid> someGuids = GetRandomGuids();

            InternalServerErrorObjectResult expectedInternalServerErrorObjectResult =
                InternalServerError(serverException);

            var expectedActionResult = expectedInternalServerErrorObjectResult;

            this.consumerOrchestrationServiceMock.Setup(service =>
                service.RecordConsumerAdoptionAsync(It.IsAny<List<Guid>>()))
                    .ThrowsAsync(serverException);

            // when
            ActionResult actualActionResult =
                await this.consumerAdoptionsController.RecordConsumerAdoptionAsync(someGuids);

            // then
            actualActionResult.Should().BeEquivalentTo(expectedActionResult);

            this.consumerOrchestrationServiceMock.Verify(service =>
                service.RecordConsumerAdoptionAsync(It.IsAny<List<Guid>>()),
                    Times.Once);

            this.consumerAdoptionServiceMock.VerifyNoOtherCalls();
            this.consumerOrchestrationServiceMock.VerifyNoOtherCalls();
        }
    }
}
