using System;
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
        public async Task ShouldAddDecisionTypeAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset =
                GetRandomDateTimeOffset();

            DecisionType randomDecisionType = CreateRandomDecisionType(randomDateTimeOffset);
            DecisionType inputDecisionType = randomDecisionType;
            DecisionType storageDecisionType = inputDecisionType;
            DecisionType expectedDecisionType = storageDecisionType.DeepClone();

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Returns(randomDateTimeOffset);

            this.storageBrokerMock.Setup(broker =>
                broker.InsertDecisionTypeAsync(inputDecisionType))
                    .ReturnsAsync(storageDecisionType);

            // when
            DecisionType actualDecisionType = await this.decisionTypeService
                .AddDecisionTypeAsync(inputDecisionType);

            // then
            actualDecisionType.Should().BeEquivalentTo(expectedDecisionType);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once());

            this.storageBrokerMock.Verify(broker =>
                broker.InsertDecisionTypeAsync(inputDecisionType),
                    Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}