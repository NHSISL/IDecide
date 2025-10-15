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
using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
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
            IQueryable<Consumer> randomConsumers = CreateRandomConsumers();
            randomConsumers.First().EntraId = randomUser.UserId;
            Consumer matchedConsumer = randomConsumers.First();
            List<ConsumerAdoption> consumerAdoptions = new List<ConsumerAdoption>();
            List<Guid> generatedIds = new List<Guid>();

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                .ReturnsAsync(randomUser);

            this.consumerServiceMock.Setup(service =>
                service.RetrieveAllConsumersAsync())
                .ReturnsAsync(randomConsumers);

            DateTimeOffset adoptionDate = GetRandomDateTimeOffset();

            foreach (var decision in inputDecisions)
            {
                Guid generatedId = Guid.NewGuid();
                generatedIds.Add(generatedId);

                this.identifierBrokerMock.Setup(broker =>
                    broker.GetIdentifierAsync())
                    .ReturnsAsync(generatedId);

                this.dateTimeBrokerMock.Setup(broker =>
                    broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(adoptionDate);

                consumerAdoptions.Add(new ConsumerAdoption
                {
                    Id = generatedId,
                    ConsumerId = matchedConsumer.Id,
                    DecisionId = decision.Id,
                    AdoptionDate = adoptionDate
                });
            }

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

            this.consumerServiceMock.Verify(service =>
                service.RetrieveAllConsumersAsync(),
                    Times.Once);

            this.identifierBrokerMock.Verify(broker =>
                broker.GetIdentifierAsync(),
                    Times.Exactly(inputDecisions.Count));

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

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

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
            this.consumerServiceMock.VerifyNoOtherCalls();
            this.consumerAdoptionServiceMock.VerifyNoOtherCalls();
            this.patientServiceMock.VerifyNoOtherCalls();
            this.notificationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldAdoptPatientDecisionsAsyncWithDecisionsWithoutPatient()
        {
            // given
            List<Decision> randomDecisions = CreateRandomDecisionsWithoutPatient();
            List<Decision> inputDecisions = randomDecisions;
            User randomUser = CreateRandomUser();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            IQueryable<Consumer> randomConsumers = CreateRandomConsumers();
            randomConsumers.First().EntraId = randomUser.UserId;
            Consumer matchedConsumer = randomConsumers.First();
            Guid consumerId = matchedConsumer.Id;
            List<ConsumerAdoption> consumerAdoptions = new List<ConsumerAdoption>();
            List<Guid> generatedIds = new List<Guid>();
            var patientById = new Dictionary<Guid, Patient>();

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                .ReturnsAsync(randomUser);

            this.consumerServiceMock.Setup(service =>
                service.RetrieveAllConsumersAsync())
                .ReturnsAsync(randomConsumers);

            foreach (var decision in inputDecisions)
            {
                Guid generatedId = Guid.NewGuid();
                generatedIds.Add(generatedId);

                this.identifierBrokerMock.Setup(broker =>
                    broker.GetIdentifierAsync())
                    .ReturnsAsync(generatedId);

                this.dateTimeBrokerMock.Setup(broker =>
                    broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

                consumerAdoptions.Add(new ConsumerAdoption
                {
                    Id = generatedId,
                    ConsumerId = consumerId,
                    DecisionId = decision.Id,
                    AdoptionDate = randomDateTimeOffset
                });

                var patient = CreateRandomPatient();
                patientById[decision.PatientId] = patient;

                this.patientServiceMock.Setup(service =>
                    service.RetrievePatientByIdAsync(decision.PatientId))
                    .ReturnsAsync(patient);

                this.notificationServiceMock.Setup(service =>
                    service.SendSubscriberUsageNotificationAsync(
                        It.Is<NotificationInfo>(info =>
                            info.Decision == decision &&
                            info.Patient == patient)))
                    .Returns(ValueTask.CompletedTask);
            }

            this.consumerAdoptionServiceMock.Setup(service =>
                service.BulkAddOrModifyConsumerAdoptionsAsync(consumerAdoptions, It.IsAny<int>()))
                    .Returns(ValueTask.CompletedTask);

            // when
            await this.consumerOrchestrationService.AdoptPatientDecisions(inputDecisions);

            // then
            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.consumerServiceMock.Verify(service =>
                service.RetrieveAllConsumersAsync(),
                    Times.Once);

            this.identifierBrokerMock.Verify(broker =>
                broker.GetIdentifierAsync(),
                    Times.Exactly(inputDecisions.Count));

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

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
                var expectedPatient = patientById[decision.PatientId];

                this.notificationServiceMock.Verify(service =>
                    service.SendSubscriberUsageNotificationAsync(
                        It.Is<NotificationInfo>(info =>
                            info.Decision == decision &&
                            info.Patient == expectedPatient)),
                        Times.Once);

                this.patientServiceMock.Verify(service =>
                    service.RetrievePatientByIdAsync(decision.PatientId),
                        Times.Once);
            }

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
            this.consumerServiceMock.VerifyNoOtherCalls();
            this.consumerAdoptionServiceMock.VerifyNoOtherCalls();
            this.patientServiceMock.VerifyNoOtherCalls();
            this.notificationServiceMock.VerifyNoOtherCalls();
        }
    }
}
