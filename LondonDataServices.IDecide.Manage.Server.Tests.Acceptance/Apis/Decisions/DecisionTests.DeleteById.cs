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
        public async Task ShouldDeleteDecisionByIdAsync()
        {
            // given
            Patient randomPatient = await PostRandomPatientAsync();
            DecisionType randomDecisionType = await PostRandomDecisionTypeAsync();

            Decision randomDecision = await PostRandomDecisionAsync(
                patientId: randomPatient.Id, decisionTypeId: randomDecisionType.Id);

            Decision inputDecision = randomDecision;
            Decision expectedDecision = inputDecision;

            // when
            Decision deletedDecision =
                await this.apiBroker.DeleteDecisionByIdAsync(inputDecision.Id);

            List<Decision> actualResult =
                await this.apiBroker.GetSpecificDecisionByIdAsync(inputDecision.Id);

            // then
            actualResult.Count().Should().Be(0);
        }
    }
}