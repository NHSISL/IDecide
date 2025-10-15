// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Unit.Controllers.ConsumerAdoptions
{
    public partial class ConsumerAdoptionsControllerTests
    {
        [Fact]
        public async Task ShouldRecordConsumerAdoptionAsync()
        {
            // given
            List<Guid> randomGuids = GetRandomGuids();
            List<Guid> inputGuids = randomGuids;
            var expectedResult = new OkResult();
            var expectedActionResult = expectedResult;

            // when
            ActionResult actualActionResult = await consumerAdoptionsController.RecordConsumerAdoptionAsync(inputGuids);

            // then
            actualActionResult.Should().BeEquivalentTo(expectedActionResult);

            consumerOrchestrationServiceMock
                .Verify(service => service.RecordConsumerAdoptionAsync(inputGuids),
                    Times.Once);

            consumerAdoptionServiceMock.VerifyNoOtherCalls();
        }
    }
}
