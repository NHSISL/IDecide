// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Brokers.Loggings;
using LondonDataServices.IDecide.Core.Models.Foundations.Users;
using LondonDataServices.IDecide.Core.Models.Orchestrations.NhsDigitalApis;
using LondonDataServices.IDecide.Core.Services.Foundations.NhsDigitalApis;
using LondonDataServices.IDecide.Core.Services.Foundations.Users;

namespace LondonDataServices.IDecide.Core.Services.Orchestrations.NhsDigitalApis
{
    public partial class NhsDigitalApiOrchestrationService : INhsDigitalApiOrchestrationService
    {
        private readonly INhsDigitalApiService nhsDigitalApiService;
        private readonly IUserService userService;
        private readonly ILoggingBroker loggingBroker;

        public NhsDigitalApiOrchestrationService(
            INhsDigitalApiService nhsDigitalApiService,
            IUserService userService,
            ILoggingBroker loggingBroker)
        {
            this.nhsDigitalApiService = nhsDigitalApiService;
            this.userService = userService;
            this.loggingBroker = loggingBroker;
        }

        public async ValueTask ProcessCallbackAsync(
            string code,
            string state,
            CancellationToken cancellationToken)
        {
            await TryCatch(async () =>
            {
                ValidateProcessCallbackArguments(code, state);

                string userInfoJson =
                    await this.nhsDigitalApiService.GetUserInfoAsync(code, state, cancellationToken);

                NhsDigitalUserInfo userInfo =
                    JsonSerializer.Deserialize<NhsDigitalUserInfo>(userInfoJson);

                // ValidateUserInfo(userInfo)
                ValidateUserInfo(userInfo);

                string rawUserInfo = JsonSerializer.Serialize(userInfo);

                IQueryable<User> allUsers = await this.userService.RetrieveAllUsersAsync();

                User maybeUser = allUsers
                    .FirstOrDefault(u => u.NhsIdUserUid == userInfo.NhsIdUserUid);

                if (maybeUser is null)
                {
                    var newUser = new User
                    {
                        NhsIdUserUid = userInfo.NhsIdUserUid,
                        Name = userInfo.Name,
                        Sub = userInfo.Sub,
                        RawUserInfo = rawUserInfo,
                        LastLoginAt = DateTime.UtcNow,
                        IsAuthorised = false
                    };

                    maybeUser = await this.userService.AddUserAsync(newUser);
                }
                else
                {
                    maybeUser.LastLoginAt = DateTime.UtcNow;
                    maybeUser.RawUserInfo = rawUserInfo;
                    maybeUser = await this.userService.ModifyUserAsync(maybeUser);
                }

                // ValidateUser(maybeUser)

                if (maybeUser.IsAuthorised is false)
                {
                    await this.nhsDigitalApiService.LogoutAsync(cancellationToken);
                }
            });
        }
    }
}
