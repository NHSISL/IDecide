// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Foundations.ConsumerAdoptions;
using LondonDataServices.IDecide.Core.Services.Foundations.ConsumerAdoptions;
using Moq;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.ConsumerAdoptions
{
    public partial class ConsumerAdoptionServiceTests
    {
        [Fact]
        public async Task ShouldBulkAddOrModifyConsumerAdoptionsAsync()
        {
            List<ConsumerAdoption> randomConsumerAdoptions = CreateRandomConsumerAdoptions().ToList();
            List<ConsumerAdoption> inputConsumerAdoptions = randomConsumerAdoptions;
            int batchSize = 10000;

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
                    .Returns(ValueTask.CompletedTask);

            // when
            await consumerAdoptionServiceMock.Object
                .BulkAddOrModifyConsumerAdoptionsAsync(inputConsumerAdoptions);

            // then
            consumerAdoptionServiceMock.Verify(service =>
                service.BulkAddOrModifyBatchAsync(inputConsumerAdoptions, batchSize),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
