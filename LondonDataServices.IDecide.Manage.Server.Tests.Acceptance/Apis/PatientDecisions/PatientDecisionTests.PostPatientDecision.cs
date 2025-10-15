// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Models.DecisionTypes;
using LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Models.PatientDecisions;
using LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Models.Patients;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Apis.PatientDecisions
{
    public partial class PatientDecisionTests
    {
        [Fact]
        public async Task ShouldPostPatientDecisionAsync()
        {
            // given
            Patient randomPatient = await PostRandomPatientAsync();
            DecisionType randomDecisionType = await PostRandomDecisionTypeAsync();

            Decision randomDecision =
                CreateRandomDecision(patient: randomPatient, decisionTypeId: randomDecisionType.Id);

            Decision inputDecision = randomDecision;
            Decision expectedDecision = inputDecision;

            // when
            await this.apiBroker.PostPatientDecisionAsync(inputDecision);

            Models.Decisions.Decision actualDecision =
                await this.apiBroker.GetDecisionByIdAsync(inputDecision.Id);

            // then
            actualDecision.Should().BeEquivalentTo(expectedDecision, options => options
                .Excluding(property => property.CreatedBy)
                .Excluding(property => property.CreatedDate)
                .Excluding(property => property.UpdatedBy)
                .Excluding(property => property.UpdatedDate)
                .Excluding(property => property.Patient)
                .Excluding(property => property.DecisionType));

            await this.apiBroker.DeleteDecisionByIdAsync(actualDecision.Id);
            await this.apiBroker.DeletePatientByIdAsync(randomPatient.Id);
            await this.apiBroker.DeleteDecisionTypeByIdAsync(randomDecisionType.Id);
        }
    }
}
