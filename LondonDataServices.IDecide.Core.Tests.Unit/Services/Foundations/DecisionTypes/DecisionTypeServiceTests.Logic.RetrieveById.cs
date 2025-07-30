using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using StandardlyTestProject.Api.Models.Foundations.DecisionTypes;
using Xunit;

namespace StandardlyTestProject.Api.Tests.Unit.Services.Foundations.DecisionTypes
{
    public partial class DecisionTypeServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveDecisionTypeByIdAsync()
        {
            // given
            DecisionType randomDecisionType = CreateRandomDecisionType();
            DecisionType inputDecisionType = randomDecisionType;
            DecisionType storageDecisionType = randomDecisionType;
            DecisionType expectedDecisionType = storageDecisionType.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectDecisionTypeByIdAsync(inputDecisionType.Id))
                    .ReturnsAsync(storageDecisionType);

            // when
            DecisionType actualDecisionType =
                await this.decisionTypeService.RetrieveDecisionTypeByIdAsync(inputDecisionType.Id);

            // then
            actualDecisionType.Should().BeEquivalentTo(expectedDecisionType);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectDecisionTypeByIdAsync(inputDecisionType.Id),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}