// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Portal.Server.Tests.Integration.Models.Decisions;
using LondonDataServices.IDecide.Portal.Server.Tests.Integration.Models.DecisionTypes;
using LondonDataServices.IDecide.Portal.Server.Tests.Integration.Models.Patients;

namespace LondonDataServices.IDecide.Portal.Server.Tests.Integration.Apis.Decisions
{
    public partial class DecisionApiTests
    {
        [Fact]
        public async Task ShouldPutDecisionAsync()
        {
            // given
            Patient randomPatient = await PostRandomPatientAsync();
            DecisionType randomDecisionType = await PostRandomDecisionTypeAsync();

            Decision randomDecision =
                await PostRandomDecisionAsync(patientId: randomPatient.Id, decisionTypeId: randomDecisionType.Id);

            Decision modifiedDecision = UpdateDecisionWithRandomValues(randomDecision);

            // when
            await this.apiBroker.PutDecisionAsync(modifiedDecision);
            Decision actualDecision = await this.apiBroker.GetDecisionByIdAsync(randomDecision.Id);

            // then
            actualDecision.Should().BeEquivalentTo(
                modifiedDecision,
                options => options
                    .Excluding(decision => decision.CreatedBy)
                    .Excluding(decision => decision.CreatedDate)
                    .Excluding(decision => decision.UpdatedBy)
                    .Excluding(decision => decision.UpdatedDate));

            await this.apiBroker.DeleteDecisionByIdAsync(actualDecision.Id);
            await this.apiBroker.DeletePatientByIdAsync(randomPatient.Id);
            await this.apiBroker.DeleteDecisionTypeByIdAsync(randomDecisionType.Id);
        }
    }
}
