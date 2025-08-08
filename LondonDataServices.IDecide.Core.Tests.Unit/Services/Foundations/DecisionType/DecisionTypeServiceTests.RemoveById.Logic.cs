// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using LondonDataServices.IDecide.Core.Models.Foundations.DecisionType;
using Moq;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.DecisionType
{
    public partial class DecisionTypeServiceTests
    {
        [Fact]
        public async Task ShouldRemoveDecisionTypeByIdAsync()
        {
            // given
            Guid randomId = Guid.NewGuid();
            Guid inputDecisionTypeId = randomId;
            DecisionType randomDecisionType = CreateRandomDecisionType();
            DecisionType storageDecisionType = randomDecisionType;
            DecisionType expectedInputDecisionType = storageDecisionType;
            DecisionType deletedDecisionType = expectedInputDecisionType;
            DecisionType expectedDecisionType = deletedDecisionType.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectDecisionTypeByIdAsync(inputDecisionTypeId))
                    .ReturnsAsync(storageDecisionType);

            this.storageBrokerMock.Setup(broker =>
                broker.DeleteDecisionTypeAsync(expectedInputDecisionType))
                    .ReturnsAsync(deletedDecisionType);

            // when
            DecisionType actualDecisionType = await this.decisionTypeService
                .RemoveDecisionTypeByIdAsync(inputDecisionTypeId);

            // then
            actualDecisionType.Should().BeEquivalentTo(expectedDecisionType);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectDecisionTypeByIdAsync(inputDecisionTypeId),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteDecisionTypeAsync(expectedInputDecisionType),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}