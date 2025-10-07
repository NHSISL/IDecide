// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
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
        public async Task ShouldBulkAddOrModifyBatchAsync()
        {
            // given
            int batchSize = 10000;
            List<ConsumerAdoption> randomNewConsumerAdoptions = CreateRandomConsumerAdoptions().ToList();
            List<ConsumerAdoption> randomExistingConsumerAdoptions = CreateRandomConsumerAdoptions().ToList();
            List<ConsumerAdoption> inputConsumerAdoptions = new List<ConsumerAdoption>();
            inputConsumerAdoptions.AddRange(randomNewConsumerAdoptions);
            inputConsumerAdoptions.AddRange(randomExistingConsumerAdoptions);
            int batchCount = GetBatchSize(inputConsumerAdoptions.Count, batchSize);

            var consumerAdoptionServiceMock = new Mock<ConsumerAdoptionService>(
                this.storageBrokerMock.Object,
                this.dateTimeBrokerMock.Object,
                this.securityAuditBrokerMock.Object,
                this.loggingBrokerMock.Object)
            {
                CallBase = true
            };

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllConsumerAdoptionsAsync())
                    .ReturnsAsync(randomExistingConsumerAdoptions.AsQueryable());

            int totalRecords = inputConsumerAdoptions.Count;

            for (int i = 0; i < totalRecords; i += batchSize)
            {
                var batch = inputConsumerAdoptions.Skip(i).Take(batchSize).ToList();
                List<Guid> batchIds = batch.Select(consumerAdoption => consumerAdoption.Id).ToList();

                List<Guid> storageIds = randomExistingConsumerAdoptions
                    .Select(consumerAdoption => consumerAdoption.Id)
                    .ToList();

                var existingIds = storageIds
                    .Where(storageId => batchIds.Contains(storageId))
                    .Select(storageId => storageId)
                    .ToList();

                var existingConsumerAdoptions = batch
                    .Where(consumerAdoption => existingIds.Contains(consumerAdoption.Id)).ToList();

                var newConsumerAdoptions = batch
                    .Where(consumerAdoption => !existingIds.Contains(consumerAdoption.Id)).ToList();

                consumerAdoptionServiceMock.Setup(service =>
                    service.ValidateConsumerAdoptionsAndAssignIdAndAuditOnAddAsync(newConsumerAdoptions))
                        .ReturnsAsync(newConsumerAdoptions);

                this.storageBrokerMock.Setup(broker =>
                    broker.BulkInsertConsumerAdoptionsAsync(newConsumerAdoptions))
                        .Returns(ValueTask.CompletedTask);

                consumerAdoptionServiceMock.Setup(service =>
                    service.ValidateConsumerAdoptionsAndAssignAuditOnModifyAsync(existingConsumerAdoptions))
                        .ReturnsAsync(existingConsumerAdoptions);

                this.storageBrokerMock.Setup(broker =>
                    broker.BulkUpdateConsumerAdoptionsAsync(existingConsumerAdoptions))
                        .Returns(ValueTask.CompletedTask);
            }

            // when
            await consumerAdoptionServiceMock.Object
                .BulkAddOrModifyBatchAsync(inputConsumerAdoptions, batchSize);

            // then
            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllConsumerAdoptionsAsync(),
                    Times.Exactly(batchCount));

            for (int i = 0; i < totalRecords; i += batchSize)
            {
                var batch = inputConsumerAdoptions.Skip(i).Take(batchSize).ToList();
                List<Guid> batchIds = batch.Select(consumerAdoption => consumerAdoption.Id).ToList();

                List<Guid> storageIds = randomExistingConsumerAdoptions
                    .Select(consumerAdoption => consumerAdoption.Id).ToList();

                var existingIds = storageIds
                    .Where(storageId => batchIds.Contains(storageId))
                    .Select(storageId => storageId)
                    .ToList();

                var newConsumerAdoptions =
                    batch.Where(consumerAdoption => !existingIds.Contains(consumerAdoption.Id)).ToList();

                var existingConsumerAdoptions =
                    batch.Where(consumerAdoption => existingIds.Contains(consumerAdoption.Id)).ToList();

                consumerAdoptionServiceMock.Verify(service =>
                    service.ValidateConsumerAdoptionsAndAssignIdAndAuditOnAddAsync(newConsumerAdoptions),
                        Times.Once);

                this.storageBrokerMock.Verify(broker =>
                    broker.BulkInsertConsumerAdoptionsAsync(newConsumerAdoptions),
                        Times.Once);

                consumerAdoptionServiceMock.Verify(service =>
                    service.ValidateConsumerAdoptionsAndAssignAuditOnModifyAsync(existingConsumerAdoptions),
                        Times.Once);

                this.storageBrokerMock.Verify(broker =>
                    broker.BulkUpdateConsumerAdoptionsAsync(existingConsumerAdoptions),
                        Times.Once);
            }

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
