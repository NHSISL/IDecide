// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using LondonDataServices.IDecide.Core.Models.Foundations.Users;
using Moq;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Orchestrations.NhsDigitalApis
{
    public partial class NhsDigitalApiOrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldProcessCallbackForExistingAuthorisedUserAsync()
        {
            // given
            CancellationToken inputCancellationToken = GetCancellationToken();
            string inputCode = GetRandomString();
            string inputState = GetRandomString();

            User randomUser = CreateRandomUser(isAuthorised: true);
            User existingUser = randomUser.DeepClone();
            User updatedUser = randomUser.DeepClone();

            string randomUserInfoJson =
                CreateUserInfoJson(
                    nhsIdUserUid: existingUser.NhsIdUserUid,
                    name: existingUser.Name,
                    sub: existingUser.Sub);

            string returnedUserInfoJson = randomUserInfoJson.DeepClone();

            List<User> randomUsers = new List<User> { existingUser };
            IQueryable<User> returnedUsers = randomUsers.AsQueryable();

            this.nhsDigitalApiServiceMock.Setup(service =>
                service.GetUserInfoAsync(inputCode, inputState, inputCancellationToken))
                    .ReturnsAsync(returnedUserInfoJson);

            this.userServiceMock.Setup(service =>
                service.RetrieveAllUsersAsync())
                    .ReturnsAsync(returnedUsers);

            this.userServiceMock.Setup(service =>
                service.ModifyUserAsync(It.Is(SameUserAs(existingUser))))
                    .ReturnsAsync(updatedUser);

            // when
            await this.nhsDigitalApiOrchestrationService
                .ProcessCallbackAsync(inputCode, inputState, inputCancellationToken);

            // then
            this.nhsDigitalApiServiceMock.Verify(service =>
                service.GetUserInfoAsync(inputCode, inputState, inputCancellationToken),
                Times.Once);

            this.userServiceMock.Verify(service =>
                service.RetrieveAllUsersAsync(),
                Times.Once);

            this.userServiceMock.Verify(service =>
                service.ModifyUserAsync(It.Is(SameUserAs(existingUser))),
                Times.Once);

            this.userServiceMock.Verify(service =>
                service.AddUserAsync(It.IsAny<User>()),
                Times.Never);

            this.nhsDigitalApiServiceMock.Verify(service =>
                service.LogoutAsync(It.IsAny<CancellationToken>()),
                Times.Never);

            this.nhsDigitalApiServiceMock.VerifyNoOtherCalls();
            this.userServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
