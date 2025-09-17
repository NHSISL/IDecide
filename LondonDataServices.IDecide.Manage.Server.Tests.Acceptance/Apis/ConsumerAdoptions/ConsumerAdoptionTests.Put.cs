// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Models.ConsumerAdoptions;
using LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Models.Consumers;
using LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Models.Decisions;
using LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Models.DecisionTypes;
using LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Models.Patients;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Acceptance.Apis.ConsumerAdoptions
{
    public partial class ConsumerAdoptionApiTests
    {
        [Fact]
        public async Task ShouldPutConsumerAdoptionAsync()
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

            ConsumerAdoption modifiedConsumerAdoption =
                UpdateConsumerAdoptionWithRandomValues(randomConsumerAdoption);

            // when
            await this.apiBroker.PutConsumerAdoptionAsync(modifiedConsumerAdoption);

            ConsumerAdoption actualConsumerAdoption = await this.apiBroker
                .GetConsumerAdoptionByIdAsync(randomConsumerAdoption.Id);

            // then
            actualConsumerAdoption.Should().BeEquivalentTo(modifiedConsumerAdoption, options => options
                .Excluding(property => property.CreatedBy)
                .Excluding(property => property.CreatedDate)
                .Excluding(property => property.UpdatedBy)
                .Excluding(property => property.UpdatedDate));

            await this.apiBroker.DeleteConsumerAdoptionByIdAsync(actualConsumerAdoption.Id);
            await this.apiBroker.DeleteConsumerByIdAsync(randomConsumer.Id);
            await this.apiBroker.DeleteDecisionByIdAsync(randomDecision.Id);
            await this.apiBroker.DeleteDecisionTypeByIdAsync(randomDecisionType.Id);
            await this.apiBroker.DeletePatientByIdAsync(randomPatient.Id);
        }
    }
}