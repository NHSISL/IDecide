// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using LondonDataServices.IDecide.Portal.Server.Tests.Integration.Models.Decisions;
using LondonDataServices.IDecide.Portal.Server.Tests.Integration.Models.DecisionTypes;
using LondonDataServices.IDecide.Portal.Server.Tests.Integration.Models.Patients;
using RESTFulSense.Exceptions;

namespace LondonDataServices.IDecide.Portal.Server.Tests.Integration.Apis.Decisions
{
    public partial class DecisionApiTests
    {
        [Fact]
        public async Task ShouldDeleteDecisionAsync()
        {
            // given
            Patient randomPatient = await PostRandomPatientAsync();
            DecisionType randomDecisionType = await PostRandomDecisionTypeAsync();

            Decision randomDecision =
                await PostRandomDecisionAsync(patientId: randomPatient.Id, decisionTypeId: randomDecisionType.Id);

            // when
            await this.apiBroker.DeleteDecisionByIdAsync(randomDecision.Id);

            // then
            ValueTask<Decision> getDecisionByIdTask =
                this.apiBroker.GetDecisionByIdAsync(randomDecision.Id);

            await Assert.ThrowsAsync<HttpResponseNotFoundException>(getDecisionByIdTask.AsTask);
            await this.apiBroker.DeletePatientByIdAsync(randomPatient.Id);
            await this.apiBroker.DeleteDecisionTypeByIdAsync(randomDecisionType.Id);
        }
    }
}
