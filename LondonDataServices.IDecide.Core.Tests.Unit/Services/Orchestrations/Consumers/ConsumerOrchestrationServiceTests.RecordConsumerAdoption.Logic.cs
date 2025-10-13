// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Foundations.ConsumerAdoptions;
using LondonDataServices.IDecide.Core.Models.Foundations.Consumers;
using LondonDataServices.IDecide.Core.Models.Securities;
using Moq;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Orchestrations.Consumers
{
    public partial class ConsumerOrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldRecordConsumerAdoptionAsync()
        {
            // given
            List<Guid> decisionIds = CreateRandomDecisionIds();
            User randomUser = CreateRandomUser();
            IQueryable<Consumer> randomConsumers = CreateRandomConsumers();
            randomConsumers.First().EntraId = randomUser.UserId;
            Consumer matchedConsumer = randomConsumers.First();
            Guid consumerId = matchedConsumer.Id;
            DateTimeOffset adoptionDate = GetRandomDateTimeOffset();
            List<Guid> generatedIds = new List<Guid>();
            List<ConsumerAdoption> expectedConsumerAdoptions = new List<ConsumerAdoption>();

            this.securityBrokerMock.Setup(broker =>
                broker.IsCurrentUserAuthenticatedAsync())
                    .ReturnsAsync(true);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomUser);

            this.consumerServiceMock.Setup(service =>
                service.RetrieveAllConsumersAsync())
                    .ReturnsAsync(randomConsumers);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(adoptionDate);

            foreach (var decisionId in decisionIds)
            {
                Guid generatedId = Guid.NewGuid();

                this.identifierBrokerMock.Setup(broker =>
                    broker.GetIdentifierAsync())
                    .ReturnsAsync(generatedId);

                generatedIds.Add(generatedId);

                expectedConsumerAdoptions.Add(new ConsumerAdoption
                {
                    Id = generatedId,
                    ConsumerId = consumerId,
                    DecisionId = decisionId,
                    AdoptionDate = adoptionDate
                });
            }

            this.consumerAdoptionServiceMock.Setup(service =>
                service.BulkAddOrModifyConsumerAdoptionsAsync(expectedConsumerAdoptions, It.IsAny<int>()))
                    .Returns(ValueTask.CompletedTask);

            // when
            await this.consumerOrchestrationService.RecordConsumerAdoption(decisionIds);

            // then
            this.securityBrokerMock.Verify(broker =>
                broker.IsCurrentUserAuthenticatedAsync(),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.consumerServiceMock.Verify(service =>
                service.RetrieveAllConsumersAsync(),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.identifierBrokerMock.Verify(broker =>
                broker.GetIdentifierAsync(),
                    Times.Exactly(decisionIds.Count));

            this.consumerAdoptionServiceMock.Verify(service =>
                service.BulkAddOrModifyConsumerAdoptionsAsync(
                    It.Is<List<ConsumerAdoption>>(consumerAdoptions =>
                        consumerAdoptions.Count == expectedConsumerAdoptions.Count &&
                        Enumerable.Range(0, expectedConsumerAdoptions.Count).All(i =>
                            consumerAdoptions[i].ConsumerId == expectedConsumerAdoptions[i].ConsumerId &&
                            consumerAdoptions[i].DecisionId == expectedConsumerAdoptions[i].DecisionId &&
                            consumerAdoptions[i].AdoptionDate == expectedConsumerAdoptions[i].AdoptionDate)),
                    It.IsAny<int>()),
                    Times.Once);

            this.securityBrokerMock.VerifyNoOtherCalls();
            this.consumerServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
            this.consumerAdoptionServiceMock.VerifyNoOtherCalls();
        }
    }
}
