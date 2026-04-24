// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using User = LondonDataServices.IDecide.Core.Models.Foundations.Users.User;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.Users
{
    public partial class UserServiceTests
    {
        [Fact]
        public async Task ShouldModifyUserAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            string randomUserId = GetRandomString();
            User randomUser = CreateRandomModifyUser(randomDateTimeOffset, randomUserId);
            User inputUser = randomUser;
            User auditAppliedUser = inputUser.DeepClone();
            auditAppliedUser.UpdatedBy = randomUserId;
            auditAppliedUser.UpdatedDate = randomDateTimeOffset;
            User storageUser = auditAppliedUser.DeepClone();
            storageUser.UpdatedDate = storageUser.CreatedDate;
            User afterAuditUser = auditAppliedUser.DeepClone();
            User updatedUser = auditAppliedUser.DeepClone();
            User expectedUser = updatedUser.DeepClone();

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyModifyAuditValuesAsync(inputUser))
                    .ReturnsAsync(auditAppliedUser);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.GetCurrentUserIdAsync())
                    .ReturnsAsync(randomUserId);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectUserByIdAsync(inputUser.Id))
                    .ReturnsAsync(storageUser);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.EnsureAddAuditValuesRemainsUnchangedOnModifyAsync(auditAppliedUser, storageUser))
                    .ReturnsAsync(afterAuditUser);

            this.storageBrokerMock.Setup(broker =>
                broker.UpdateUserAsync(afterAuditUser))
                    .ReturnsAsync(updatedUser);

            // when
            User actualUser = await this.userService.ModifyUserAsync(inputUser);

            // then
            actualUser.Should().BeEquivalentTo(expectedUser);

            this.securityAuditBrokerMock.Verify(broker =>
                    broker.ApplyModifyAuditValuesAsync(inputUser),
                Times.Once);

            this.securityAuditBrokerMock.Verify(broker =>
                    broker.GetCurrentUserIdAsync(),
                Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                    broker.GetCurrentDateTimeOffsetAsync(),
                Times.Once);

            this.storageBrokerMock.Verify(broker =>
                    broker.SelectUserByIdAsync(inputUser.Id),
                Times.Once);

            this.securityAuditBrokerMock.Verify(broker =>
                    broker.EnsureAddAuditValuesRemainsUnchangedOnModifyAsync(auditAppliedUser, storageUser),
                Times.Once);

            this.storageBrokerMock.Verify(broker =>
                    broker.UpdateUserAsync(afterAuditUser),
                Times.Once);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
