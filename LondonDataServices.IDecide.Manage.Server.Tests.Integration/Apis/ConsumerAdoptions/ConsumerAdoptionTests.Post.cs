// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Manage.Server.Tests.Integration.Models.ConsumerAdoptions;
using LondonDataServices.IDecide.Manage.Server.Tests.Integration.Models.Consumers;
using LondonDataServices.IDecide.Manage.Server.Tests.Integration.Models.Decisions;
using LondonDataServices.IDecide.Manage.Server.Tests.Integration.Models.DecisionTypes;
using LondonDataServices.IDecide.Manage.Server.Tests.Integration.Models.Patients;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Integration.Apis.ConsumerAdoptions
{
    public partial class ConsumerAdoptionApiTests
    {
        [Fact]
        public async Task ShouldPostConsumerAdoptionAsync()
        {
            // given
            Consumer randomConsumer = await PostRandomConsumerAsync();
            Patient randomPatient = await PostRandomPatientAsync();
            DecisionType randomDecisionType = await PostRandomDecisionTypeAsync();

            Decision randomDecision =
                await PostRandomDecisionAsync(patientId: randomPatient.Id, decisionTypeId: randomDecisionType.Id);

            ConsumerAdoption randomConsumerAdoption = CreateRandomConsumerAdoption(
                consumerId: randomConsumer.Id,
                decisionId: randomDecision.Id);

            ConsumerAdoption expectedConsumerAdoption = randomConsumerAdoption;

            // when 
            await this.apiBroker.PostConsumerAdoptionAsync(randomConsumerAdoption);

            ConsumerAdoption actualConsumerAdoption =
                await this.apiBroker.GetConsumerAdoptionByIdAsync(randomConsumerAdoption.Id);

            // then
            actualConsumerAdoption.Should().BeEquivalentTo(
                expectedConsumerAdoption,
                options => options
                    .Excluding(consumerAdoption => consumerAdoption.CreatedBy)
                    .Excluding(consumerAdoption => consumerAdoption.CreatedDate)
                    .Excluding(consumerAdoption => consumerAdoption.UpdatedBy)
                    .Excluding(consumerAdoption => consumerAdoption.UpdatedDate));

            await this.apiBroker.DeleteConsumerAdoptionByIdAsync(actualConsumerAdoption.Id);
            await this.apiBroker.DeleteConsumerByIdAsync(randomConsumer.Id);
            await this.apiBroker.DeleteDecisionByIdAsync(randomDecision.Id);
            await this.apiBroker.DeleteDecisionTypeByIdAsync(randomDecisionType.Id);
            await this.apiBroker.DeletePatientByIdAsync(randomPatient.Id);
        }
    }
}
