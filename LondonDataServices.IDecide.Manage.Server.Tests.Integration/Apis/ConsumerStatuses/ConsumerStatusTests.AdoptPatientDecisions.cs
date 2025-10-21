// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Manage.Server.Tests.Integration.Models.ConsumerAdoptions;
using LondonDataServices.IDecide.Manage.Server.Tests.Integration.Models.Consumers;
using LondonDataServices.IDecide.Manage.Server.Tests.Integration.Models.ConsumerStatuses;
using LondonDataServices.IDecide.Manage.Server.Tests.Integration.Models.DecisionTypes;
using LondonDataServices.IDecide.Manage.Server.Tests.Integration.Models.Patients;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Integration.Apis.ConsumerStatuses
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
                await this.apiBroker.DeleteConsumerAdoptionByIdAsync(consumerAdoption.Id);
            }

            await this.apiBroker.DeleteConsumerByIdAsync(randomConsumerWithMatchingEntraId.Id);

            foreach (var decision in randomDecisions)
            {
                await this.apiBroker.DeleteDecisionByIdAsync(decision.Id);
            }
        }
    }
}
