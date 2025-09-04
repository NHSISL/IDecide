// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Manage.Server.Tests.Integration.Models.Decisions;
using LondonDataServices.IDecide.Manage.Server.Tests.Integration.Models.DecisionTypes;
using LondonDataServices.IDecide.Manage.Server.Tests.Integration.Models.Patients;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Integration.Apis.Decisions
{
    public partial class DecisionApiTests
    {
        [Fact]
        public async Task ShouldGetAllDecisionsAsync()
        {
            // given
            Patient randomPatient = await PostRandomPatientAsync();
            DecisionType randomDecisionType = await PostRandomDecisionTypeAsync();

            List<Decision> randomDecisions =
                await PostRandomDecisionsAsync(patientId: randomPatient.Id, decisionTypeId: randomDecisionType.Id);

            List<Decision> expectedDecisions = randomDecisions;

            // when
            List<Decision> actualDecisions = await this.apiBroker.GetAllDecisionsAsync();

            // then
            actualDecisions.Should().NotBeNull();

            foreach (Decision expectedDecision in expectedDecisions)
            {
                Decision actualDecision = actualDecisions
                    .Single(decision => decision.Id == expectedDecision.Id);

                actualDecision.Should().BeEquivalentTo(
                    expectedDecision,
                    options => options
                        .Excluding(property => property.CreatedBy)
                        .Excluding(property => property.CreatedDate)
                        .Excluding(property => property.UpdatedBy)
                        .Excluding(property => property.UpdatedDate));
            }

            // cleanup
            foreach (Decision createdDecision in expectedDecisions)
            {
                await this.apiBroker.DeleteDecisionByIdAsync(createdDecision.Id);
            }
        }
    }
}
