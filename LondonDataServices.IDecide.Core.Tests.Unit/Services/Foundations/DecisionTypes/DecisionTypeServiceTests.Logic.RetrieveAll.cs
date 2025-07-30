using System.Linq;
using FluentAssertions;
using Moq;
using StandardlyTestProject.Api.Models.Foundations.DecisionTypes;
using Xunit;

namespace StandardlyTestProject.Api.Tests.Unit.Services.Foundations.DecisionTypes
{
    public partial class DecisionTypeServiceTests
    {
        [Fact]
        public void ShouldReturnDecisionTypes()
        {
            // given
            IQueryable<DecisionType> randomDecisionTypes = CreateRandomDecisionTypes();
            IQueryable<DecisionType> storageDecisionTypes = randomDecisionTypes;
            IQueryable<DecisionType> expectedDecisionTypes = storageDecisionTypes;

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllDecisionTypes())
                    .Returns(storageDecisionTypes);

            // when
            IQueryable<DecisionType> actualDecisionTypes =
                this.decisionTypeService.RetrieveAllDecisionTypes();

            // then
            actualDecisionTypes.Should().BeEquivalentTo(expectedDecisionTypes);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllDecisionTypes(),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}