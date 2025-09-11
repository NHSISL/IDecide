// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
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
        public async Task ShouldGetAllConsumerAdoptionsAsync()
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

            List<ConsumerAdoption> expectedConsumerAdoptions = new List<ConsumerAdoption> { randomConsumerAdoption };

            // when
            List<ConsumerAdoption> actualConsumerAdoptions = await this.apiBroker.GetAllConsumerAdoptionsAsync();

            // then
            actualConsumerAdoptions.Should().NotBeNull();

            foreach (ConsumerAdoption expectedConsumerAdoption in expectedConsumerAdoptions)
            {
                ConsumerAdoption actualConsumerAdoption = actualConsumerAdoptions
                    .Single(consumerAdoption => consumerAdoption.Id == expectedConsumerAdoption.Id);

                actualConsumerAdoption.Should().BeEquivalentTo(
                    expectedConsumerAdoption,
                    options => options
                        .Excluding(property => property.CreatedBy)
                        .Excluding(property => property.CreatedDate)
                        .Excluding(property => property.UpdatedBy)
                        .Excluding(property => property.UpdatedDate));
            }

            // cleanup
            foreach (ConsumerAdoption createdConsumerAdoption in expectedConsumerAdoptions)
            {
                await this.apiBroker.DeleteConsumerAdoptionByIdAsync(createdConsumerAdoption.Id);
            }

            await this.apiBroker.DeleteConsumerByIdAsync(randomConsumer.Id);
            await this.apiBroker.DeleteDecisionByIdAsync(randomDecision.Id);
            await this.apiBroker.DeleteDecisionTypeByIdAsync(randomDecisionType.Id);
            await this.apiBroker.DeletePatientByIdAsync(randomPatient.Id);
        }
    }
}
