// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Core.Models.Foundations.ConsumerAdoptions;
using LondonDataServices.IDecide.Core.Models.Foundations.ConsumerAdoptions.Exceptions;
using LondonDataServices.IDecide.Core.Services.Foundations.ConsumerAdoptions;
using Moq;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.ConsumerAdoptions
{
    public partial class ConsumerAdoptionServiceTests
    {
        [Fact]
        public async Task ShouldThrowServiceExceptionOnBulkAddOrModifyIfServiceErrorOccursAndLogItAsync()
        {
            List<ConsumerAdoption> randomConsumerAdoptions = CreateRandomConsumerAdoptions().ToList();
            List<ConsumerAdoption> inputConsumerAdoptions = randomConsumerAdoptions;
            int batchSize = 10000;
            var serviceException = new Exception();

            var failedConsumerAdoptionServiceException =
                new FailedConsumerAdoptionServiceException(
                    message: "Failed consumerAdoption service error occurred, please contact support.",
                    innerException: serviceException);

            var expectedConsumerAdoptionServiceException =
                new ConsumerAdoptionServiceException(
                    message: "ConsumerAdoption service error occurred, contact support.",
                    innerException: failedConsumerAdoptionServiceException);

            var consumerAdoptionServiceMock = new Mock<ConsumerAdoptionService>(
                this.storageBrokerMock.Object,
                this.dateTimeBrokerMock.Object,
                this.securityAuditBrokerMock.Object,
                this.loggingBrokerMock.Object)
            {
                CallBase = true
            };

            consumerAdoptionServiceMock.Setup(service =>
                service.BulkAddOrModifyBatchAsync(inputConsumerAdoptions, batchSize))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask bulkAddOrModifyTask =
                consumerAdoptionServiceMock.Object.BulkAddOrModifyConsumerAdoptionsAsync(inputConsumerAdoptions);

            ConsumerAdoptionServiceException actualConsumerAdoptionServiceException =
                await Assert.ThrowsAsync<ConsumerAdoptionServiceException>(
                    bulkAddOrModifyTask.AsTask);

            // then
            actualConsumerAdoptionServiceException.Should()
                .BeEquivalentTo(expectedConsumerAdoptionServiceException);

            consumerAdoptionServiceMock.Verify(service =>
                service.BulkAddOrModifyBatchAsync(inputConsumerAdoptions, batchSize),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedConsumerAdoptionServiceException))),
                        Times.Once);
        }
    }
}
