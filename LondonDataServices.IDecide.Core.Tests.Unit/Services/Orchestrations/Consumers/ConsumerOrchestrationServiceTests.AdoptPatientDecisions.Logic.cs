// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Foundations.ConsumerAdoptions;
using LondonDataServices.IDecide.Core.Models.Foundations.Consumers;
using LondonDataServices.IDecide.Core.Models.Foundations.Decisions;
using LondonDataServices.IDecide.Core.Models.Foundations.Notifications;
using LondonDataServices.IDecide.Core.Models.Securities;
using Moq;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Orchestrations.Consumers
{
    public partial class ConsumerOrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldAdoptPatientDecisionsAsync()
        {
            // given
            List<Decision> randomDecisions = CreateRandomDecisions();
            List<Decision> inputDecisions = randomDecisions;
            User randomUser = CreateRandomUser();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            Consumer randomConsumer = CreateRandomConsumer();
            Guid consumerId = Guid.Parse(randomUser.UserId);
            randomConsumer.Id = consumerId;
            List<ConsumerAdoption> consumerAdoptions = new List<ConsumerAdoption>();

            foreach (var decision in inputDecisions)
            {
                consumerAdoptions.Add(new ConsumerAdoption
                {
                    ConsumerId = consumerId,
                    DecisionId = decision.Id,
                    AdoptionDate = randomDateTimeOffset
                });
            }

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomUser);

            this.consumerServiceMock.Setup(service =>
                service.RetrieveConsumerByIdAsync(consumerId))
                    .ReturnsAsync(randomConsumer);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.consumerAdoptionServiceMock.Setup(service =>
                service.BulkAddOrModifyConsumerAdoptionsAsync(consumerAdoptions, It.IsAny<int>()))
                    .Returns(ValueTask.CompletedTask);

            foreach (var decision in inputDecisions)
            {
                this.notificationServiceMock.Setup(service =>
                    service.SendSubscriberUsageNotificationAsync(
                    It.Is<NotificationInfo>(info =>
                        info.Decision == decision &&
                        info.Patient == decision.Patient)))
                    .Returns(ValueTask.CompletedTask);
            }

            // when
            await this.consumerOrchestrationService.AdoptPatientDecisions(inputDecisions);

            // then
            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Exactly(inputDecisions.Count));

            this.consumerAdoptionServiceMock.Verify(service =>
                service.BulkAddOrModifyConsumerAdoptionsAsync(
                    It.Is<List<ConsumerAdoption>>(adoptions =>
                        adoptions.Count == consumerAdoptions.Count &&
                        Enumerable.Range(0, consumerAdoptions.Count).All(i =>
                            adoptions[i].ConsumerId == consumerAdoptions[i].ConsumerId &&
                            adoptions[i].DecisionId == consumerAdoptions[i].DecisionId &&
                            adoptions[i].AdoptionDate == consumerAdoptions[i].AdoptionDate)),
                    It.IsAny<int>()),
                    Times.Once);

            foreach (var decision in inputDecisions)
            {
                this.notificationServiceMock.Verify(service =>
                    service.SendSubscriberUsageNotificationAsync(
                        It.Is<NotificationInfo>(info =>
                            info.Decision == decision &&
                            info.Patient == decision.Patient)),
                        Times.Once);
            }

            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.consumerAdoptionServiceMock.VerifyNoOtherCalls();
            this.patientServiceMock.VerifyNoOtherCalls();
            this.notificationServiceMock.VerifyNoOtherCalls();
        }
    }
}
