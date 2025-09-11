// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using LondonDataServices.IDecide.Manage.Server.Tests.Integration.Models.ConsumerAdoptions;
using LondonDataServices.IDecide.Manage.Server.Tests.Integration.Models.Consumers;
using LondonDataServices.IDecide.Manage.Server.Tests.Integration.Models.Decisions;
using LondonDataServices.IDecide.Manage.Server.Tests.Integration.Models.DecisionTypes;
using LondonDataServices.IDecide.Manage.Server.Tests.Integration.Models.Patients;
using RESTFulSense.Exceptions;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Integration.Apis.ConsumerAdoptions
{
    public partial class ConsumerAdoptionApiTests
    {
        [Fact]
        public async Task ShouldDeleteConsumerAdoptionAsync()
        {
            // given
            Consumer randomConsumer = await PostRandomConsumerAsync();
            Patient randomPatient = await PostRandomPatientAsync();
            DecisionType randomDecisionType = await PostRandomDecisionTypeAsync();

            Decision randomDecision =
                await PostRandomDecisionAsync(patientId: randomPatient.Id, decisionTypeId: randomDecisionType.Id);

            ConsumerAdoption randomConsumerAdoption = await PostRandomConsumerAdoptionAsync(
                consumerId: randomConsumer.Id,
                decisionId: randomDecision.Id);

            // when
            await this.apiBroker.DeleteConsumerAdoptionByIdAsync(randomConsumerAdoption.Id);

            // then
            ValueTask<ConsumerAdoption> getConsumerAdoptionByIdTask =
                this.apiBroker.GetConsumerAdoptionByIdAsync(randomConsumerAdoption.Id);

            await Assert.ThrowsAsync<HttpResponseNotFoundException>(getConsumerAdoptionByIdTask.AsTask);
            await this.apiBroker.DeleteConsumerByIdAsync(randomConsumer.Id);
            await this.apiBroker.DeleteDecisionByIdAsync(randomDecision.Id);
            await this.apiBroker.DeleteDecisionTypeByIdAsync(randomDecisionType.Id);
            await this.apiBroker.DeletePatientByIdAsync(randomPatient.Id);
        }
    }
}
