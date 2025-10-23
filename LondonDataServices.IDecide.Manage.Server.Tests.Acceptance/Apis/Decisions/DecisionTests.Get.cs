// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Models.Decisions;
using LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Models.DecisionTypes;
using LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Models.Patients;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Apis.Decisions
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
            foreach (Decision expectedDecision in expectedDecisions)
            {
                Decision actualDecision =
                    actualDecisions.Single(approval => approval.Id == expectedDecision.Id);

                actualDecision.Should().BeEquivalentTo(expectedDecision, options => options
                    .Excluding(property => property.CreatedBy)
                    .Excluding(property => property.CreatedDate)
                    .Excluding(property => property.UpdatedBy)
                    .Excluding(property => property.UpdatedDate)
                    .Excluding(property => property.DecisionType)
                    .Excluding(property => property.DecisionTypeName)
                    .Excluding(property => property.Patient)
                    .Excluding(property => property.PatientNhsNumber));

                await this.apiBroker.DeleteDecisionByIdAsync(actualDecision.Id);
            }

            await this.apiBroker.DeletePatientByIdAsync(randomPatient.Id);
            await this.apiBroker.DeleteDecisionTypeByIdAsync(randomDecisionType.Id);
        }
    }
}