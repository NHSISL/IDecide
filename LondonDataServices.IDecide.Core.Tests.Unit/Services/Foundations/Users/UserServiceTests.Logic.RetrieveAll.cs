// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using User = LondonDataServices.IDecide.Core.Models.Foundations.Users.User;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.Users
{
    public partial class UserServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveAllUsersAsync()
        {
            // given
            IQueryable<User> randomUsers = CreateRandomUsers();
            IQueryable<User> storageUsers = randomUsers;
            IQueryable<User> expectedUsers = storageUsers;

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllUsersAsync())
                    .ReturnsAsync(storageUsers);

            // when
            IQueryable<User> actualUsers =
                await this.userService.RetrieveAllUsersAsync();

            // then
            actualUsers.Should().BeEquivalentTo(expectedUsers);

            this.storageBrokerMock.Verify(broker =>
                    broker.SelectAllUsersAsync(),
                Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityAuditBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
