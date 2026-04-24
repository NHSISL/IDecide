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
        public async Task ShouldAddUserAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            string randomUserId = GetRandomString();
            User randomUser = CreateRandomUser(randomDateTimeOffset);
            User inputUser = randomUser;
            User auditAppliedUser = inputUser.DeepClone();
            auditAppliedUser.CreatedBy = randomUserId;
            auditAppliedUser.CreatedDate = randomDateTimeOffset;
            auditAppliedUser.UpdatedBy = randomUserId;
            auditAppliedUser.UpdatedDate = randomDateTimeOffset;
            User storageUser = auditAppliedUser.DeepClone();
            User expectedUser = storageUser.DeepClone();

            this.securityAuditBrokerMock.Setup(broker =>
                broker.ApplyAddAuditValuesAsync(inputUser))
                    .ReturnsAsync(auditAppliedUser);

            this.securityAuditBrokerMock.Setup(broker =>
                broker.GetCurrentUserIdAsync())
                    .ReturnsAsync(randomUserId);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.storageBrokerMock.Setup(broker =>
                broker.InsertUserAsync(auditAppliedUser))
                    .ReturnsAsync(storageUser);

            // when
            User actualUser = await this.userService.AddUserAsync(inputUser);

            // then
            actualUser.Should().BeEquivalentTo(expectedUser);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.ApplyAddAuditValuesAsync(inputUser),
                    Times.Once);

            this.securityAuditBrokerMock.Verify(broker =>
                broker.GetCurrentUserIdAsync(),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once());

            this.storageBrokerMock.Verify(broker =>
                broker.InsertUserAsync(auditAppliedUser),
                    Times.Once);

            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
