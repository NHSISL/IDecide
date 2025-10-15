// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Models.Consumers;
using LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Models.DecisionTypes;
using LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Models.Patients;
using DecisionEntity = LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Models.Decisions.Decision;
using PatientDecision = LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Models.PatientDecisions.Decision;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Apis.PatientDecisions
{
    public partial class PatientDecisionTests
    {
        [Fact]
        public async Task ShouldGetPatientDecisions()
        {
            // given
            Patient randomPatient = await PostRandomPatientAsync();
            DecisionType randomDecisionType = await PostRandomDecisionTypeAsync();
            string userId = "65b5ccfb-b501-4ad5-8dd7-2a33ff64eaa3";

            Consumer randomConsumerWithMatchingEntraId =
                await PostRandomConsumerWithMatchingEntraIdEntryAsync(userId);

            int decisionCount = GetRandomNumber();
            List<PatientDecision> expectedDecisions = new List<PatientDecision>();

            for (int i = 0; i < decisionCount; i++)
            {
                DecisionEntity randomDecision = await PostRandomDecisionAsync(randomPatient, randomDecisionType.Id);
                randomDecision.Should().NotBeNull();
                PatientDecision randomPatientDecision = ToPatientDecision(randomDecision, randomPatient);
                expectedDecisions.Add(randomPatientDecision);
            }

            DateTimeOffset from = DateTimeOffset.Now.AddDays(-1);
            string decisionType = randomDecisionType.Name;

            // when
            List<PatientDecision> actualDecisions = await this.apiBroker.GetPatientDecisionsAsync(from, decisionType);

            // then
            foreach (PatientDecision expectedDecision in expectedDecisions)
            {
                PatientDecision actualDecision =
                    actualDecisions.Find(decision => decision.Id == expectedDecision.Id);

                actualDecision.Should().BeEquivalentTo(expectedDecision, options => options
                    .Excluding(property => property.Patient));
            }

            foreach (var decision in expectedDecisions)
            {
                await this.apiBroker.DeleteDecisionByIdAsync(decision.Id);
            }

            await this.apiBroker.DeleteConsumerByIdAsync(randomConsumerWithMatchingEntraId.Id);
            await this.apiBroker.DeleteDecisionTypeByIdAsync(randomDecisionType.Id);
            await this.apiBroker.DeletePatientByIdAsync(randomPatient.Id);
        }
    }
}
