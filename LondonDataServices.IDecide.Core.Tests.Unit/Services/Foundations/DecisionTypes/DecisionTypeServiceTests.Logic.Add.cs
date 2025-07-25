// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using LondonDataServices.IDecide.Core.Models.Foundations.DecisionTypes;
using LondonDataServices.IDecide.Core.Models.Securities;
using Moq;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.DecisionTypes
{
    public partial class DecisionTypeServiceTests
    {
        [Fact]
        public async Task ShouldAddDecisionTypeAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            User randomUser = CreateRandomUser();

            DecisionType randomDecisionType = CreateRandomDecisionType(
                randomDateTimeOffset, userId: randomUser.UserId);

            DecisionType inputDecisionType = randomDecisionType;
            DecisionType storageDecisionType = inputDecisionType;
            DecisionType expectedDecisionType = storageDecisionType.DeepClone();

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomUser);

            this.storageBrokerMock.Setup(broker =>
                broker.InsertDecisionTypeAsync(inputDecisionType))
                    .ReturnsAsync(storageDecisionType);

            // when
            DecisionType actualDecisionType = await this.decisionTypeService
                .AddDecisionTypeAsync(inputDecisionType);

            // then
            actualDecisionType.Should().BeEquivalentTo(expectedDecisionType);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertDecisionTypeAsync(inputDecisionType),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once());

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
