// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Models.ConsumerAdoptions;
using LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Models.Consumers;
using LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Models.ConsumerStatuses;
using LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Models.DecisionTypes;
using LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Models.Patients;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Apis.ConsumerStatuses
{
    public partial class ConsumerStatusTests
    {
        [Fact]
        public async Task ShouldAdoptPatientDecisions()
        {
            // given
            Patient randomPatient = await PostRandomPatientAsync();
            DecisionType randomDecisionType = await PostRandomDecisionTypeAsync();
            List<Decision> randomDecisions = await PostRandomDecisionsAsync(randomPatient, randomDecisionType);
            List<Decision> inputDecisions = randomDecisions;
            List<Consumer> randomConsumers = await PostRandomConsumersAsync();
            string userId = TestAuthHandler.TestUserId;
            DateTimeOffset now = DateTimeOffset.Now;

            Consumer randomConsumerWithMatchingEntraId =
                await PostRandomConsumerWithMatchingEntraIdEntryAsync(userId);

            // when
            await this.apiBroker.AdoptPatientDecisionsAsync(inputDecisions);

            // then
            List<ConsumerAdoption> consumerAdoptions =
                await this.apiBroker.GetAllConsumerAdoptionsAsync();

            foreach (var consumerAdoption in consumerAdoptions)
            {
                consumerAdoption.ConsumerId.Should().Be(randomConsumerWithMatchingEntraId.Id);
                randomDecisions.Should().ContainSingle(decision => decision.Id == consumerAdoption.DecisionId);
                consumerAdoption.AdoptionDate.Should().BeAfter(now);
            }

            foreach (var consumerAdoption in consumerAdoptions)
            {
                await this.apiBroker.DeleteConsumerAdoptionByIdAsync(consumerAdoption.Id);
            }

            await this.apiBroker.DeleteConsumerByIdAsync(randomConsumerWithMatchingEntraId.Id);

            foreach (var consumer in randomConsumers)
            {
                await this.apiBroker.DeleteConsumerByIdAsync(consumer.Id);
            }

            foreach (var decision in randomDecisions)
            {
                await this.apiBroker.DeleteDecisionByIdAsync(decision.Id);
            }

            await this.apiBroker.DeleteDecisionTypeByIdAsync(randomDecisionType.Id);
            await this.apiBroker.DeletePatientByIdAsync(randomPatient.Id);
        }
    }
}
