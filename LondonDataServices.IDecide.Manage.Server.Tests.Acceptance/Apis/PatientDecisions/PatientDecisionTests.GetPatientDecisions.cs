// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Models.Consumers;
using LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Models.Decisions;
using LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Models.DecisionTypes;
using LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Models.Patients;

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
            List<Decision> randomDecisions = await PostRandomDecisionsAsync(randomPatient, randomDecisionType);
            List<Decision> expectedDecisions = randomDecisions.DeepClone();
            string userId = TestAuthHandler.TestUserId;

            Consumer randomConsumerWithMatchingEntraId =
                await PostRandomConsumerWithMatchingEntraIdEntryAsync(userId);

            DateTimeOffset from = DateTimeOffset.Now.AddDays(-1);
            string decisionType = randomDecisionType.Name;

            // when
            List<Decision> actualDecisions = await this.apiBroker.GetPatientDecisionsAsync(from, decisionType);

            // then
            foreach (Decision expectedDecision in expectedDecisions)
            {
                Decision actualDecision =
                    actualDecisions.Find(decision => decision.Id == expectedDecision.Id);

                actualDecision.Should().BeEquivalentTo(expectedDecision, options => options
                    .Excluding(property => property.CreatedBy)
                    .Excluding(property => property.CreatedDate)
                    .Excluding(property => property.UpdatedBy)
                    .Excluding(property => property.UpdatedDate)
                    .Excluding(property => property.Patient)
                    .Excluding(property => property.DecisionType));
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
