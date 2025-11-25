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
                var batch = inputConsumerAdoptions
                    .Skip(i)
                    .Take(batchSize)
                    .ToList();

                var batchDecisionIds = batch
                    .Select(ca => ca.DecisionId)
                    .Distinct()
                    .ToList();

                var batchConsumerIds = batch
                    .Select(ca => ca.ConsumerId)
                    .Distinct()
                    .ToList();

                var storageKeys = randomExistingConsumerAdoptions
                    .Select(consumerAdoption => new { consumerAdoption.DecisionId, consumerAdoption.ConsumerId })
                    .ToList();

                var existingKeys = storageKeys
                    .Where(ca => batchDecisionIds.Contains(ca.DecisionId) &&
                        batchConsumerIds.Contains(ca.ConsumerId))
                    .ToList();

                var newConsumerAdoptions = batch
                    .Where(consumerAdoption => !existingKeys.Any(existingKey =>
                        existingKey.DecisionId == consumerAdoption.DecisionId &&
                            existingKey.ConsumerId == consumerAdoption.ConsumerId))
                    .ToList();

                var existingConsumerAdoptions = batch
                    .Where(consumerAdoption => existingKeys.Any(existingKey =>
                        existingKey.DecisionId == consumerAdoption.DecisionId &&
                            existingKey.ConsumerId == consumerAdoption.ConsumerId))
                    .ToList();

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

                var batchKeys = batch
                    .Select(consumerAdoption => new { consumerAdoption.DecisionId, consumerAdoption.ConsumerId })
                    .ToList();

                var storageKeys = randomExistingConsumerAdoptions
                    .Select(consumerAdoption => new { consumerAdoption.DecisionId, consumerAdoption.ConsumerId })
                    .ToList();

                var existingKeys = storageKeys
                    .Where(storageKey => batchKeys.Any(batchKey =>
                        batchKey.DecisionId == storageKey.DecisionId &&
                            batchKey.ConsumerId == storageKey.ConsumerId))
                    .ToList();

                var newConsumerAdoptions = batch
                    .Where(adoption => !existingKeys.Any(existingKey =>
                        existingKey.DecisionId == adoption.DecisionId &&
                            existingKey.ConsumerId == adoption.ConsumerId))
                    .ToList();

                var existingConsumerAdoptions = batch
                    .Where(adoption => existingKeys.Any(existingKey =>
                        existingKey.DecisionId == adoption.DecisionId &&
                            existingKey.ConsumerId == adoption.ConsumerId))
                    .ToList();

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
