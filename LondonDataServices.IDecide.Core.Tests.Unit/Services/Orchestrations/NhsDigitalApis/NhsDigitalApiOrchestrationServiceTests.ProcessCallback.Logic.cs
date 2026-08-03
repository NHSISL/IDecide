// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
            DateTimeOffset now = GetCurrentDateTime();

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

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(now);

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

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
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
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldProcessCallbackForExistingUnauthorisedUserAsync()
        {
            // given
            CancellationToken inputCancellationToken = GetCancellationToken();
            string inputCode = GetRandomString();
            string inputState = GetRandomString();
            DateTimeOffset now = GetCurrentDateTime();

            User randomUser = CreateRandomUser(isAuthorised: false);
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

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(now);

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

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
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
                service.LogoutAsync(inputCancellationToken),
                Times.Never);

            this.nhsDigitalApiServiceMock.VerifyNoOtherCalls();
            this.userServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldProcessCallbackForNewUserAsync()
        {
            // given
            CancellationToken inputCancellationToken = GetCancellationToken();
            string inputCode = GetRandomString();
            string inputState = GetRandomString();
            DateTimeOffset now = GetCurrentDateTime();

            User randomUser = CreateRandomUser(isAuthorised: false);
            User addedUser = randomUser.DeepClone();

            string randomUserInfoJson =
                CreateUserInfoJson(
                    nhsIdUserUid: randomUser.NhsIdUserUid,
                    name: randomUser.Name,
                    sub: randomUser.Sub);

            string returnedUserInfoJson = randomUserInfoJson.DeepClone();

            IQueryable<User> returnedUsers = new List<User>().AsQueryable();

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(now);

            this.identifierBrokerMock.Setup(broker =>
                broker.GetIdentifierAsync())
                    .ReturnsAsync(randomUser.Id);

            this.nhsDigitalApiServiceMock.Setup(service =>
                service.GetUserInfoAsync(inputCode, inputState, inputCancellationToken))
                    .ReturnsAsync(returnedUserInfoJson);

            this.userServiceMock.Setup(service =>
                service.RetrieveAllUsersAsync())
                    .ReturnsAsync(returnedUsers);

            this.userServiceMock.Setup(service =>
                service.AddUserAsync(It.IsAny<User>()))
                    .ReturnsAsync(addedUser);

            // when
            await this.nhsDigitalApiOrchestrationService
                .ProcessCallbackAsync(inputCode, inputState, inputCancellationToken);

            // then
            this.nhsDigitalApiServiceMock.Verify(service =>
                service.GetUserInfoAsync(inputCode, inputState, inputCancellationToken),
                Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                Times.Once);

            this.userServiceMock.Verify(service =>
                service.RetrieveAllUsersAsync(),
                Times.Once);

            this.userServiceMock.Verify(service =>
                service.AddUserAsync(It.IsAny<User>()),
                Times.Once);

            this.userServiceMock.Verify(service =>
                service.ModifyUserAsync(It.IsAny<User>()),
                Times.Never);

            this.nhsDigitalApiServiceMock.Verify(service =>
                service.LogoutAsync(inputCancellationToken),
                Times.Never);

            this.identifierBrokerMock.Verify(broker =>
                broker.GetIdentifierAsync(),
                Times.Once);

            this.nhsDigitalApiServiceMock.VerifyNoOtherCalls();
            this.userServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
        }
    }
}