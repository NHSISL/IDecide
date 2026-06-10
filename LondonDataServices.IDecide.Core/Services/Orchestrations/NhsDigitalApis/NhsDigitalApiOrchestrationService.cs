// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Brokers.DateTimes;
using LondonDataServices.IDecide.Core.Brokers.Loggings;
using LondonDataServices.IDecide.Core.Models.Foundations.Users;
using LondonDataServices.IDecide.Core.Models.Orchestrations.NhsDigitalApis;
using LondonDataServices.IDecide.Core.Services.Foundations.NhsDigitalApis;
using LondonDataServices.IDecide.Core.Services.Foundations.Users;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.Pds;

namespace LondonDataServices.IDecide.Core.Services.Orchestrations.NhsDigitalApis
{
    public partial class NhsDigitalApiOrchestrationService : INhsDigitalApiOrchestrationService
    {
        private readonly INhsDigitalApiService nhsDigitalApiService;
        private readonly IUserService userService;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public NhsDigitalApiOrchestrationService(
            INhsDigitalApiService nhsDigitalApiService,
            IUserService userService,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.nhsDigitalApiService = nhsDigitalApiService;
            this.userService = userService;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<User> ProcessCallbackAsync(
            string code,
            string state,
            CancellationToken cancellationToken) =>
            TryCatch(async () =>
            {
                ValidateProcessCallbackArguments(code, state);

                string userInfoJson =
                    await this.nhsDigitalApiService.GetUserInfoAsync(code, state, cancellationToken);

                ValidateUserInfoJson(userInfoJson);

                NhsDigitalUserInfo userInfo =
                    JsonSerializer.Deserialize<NhsDigitalUserInfo>(userInfoJson);

                ValidateUserInfo(userInfo);
                string rawUserInfo = JsonSerializer.Serialize(userInfo);

                DateTimeOffset now =
                    await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync();

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
                        LastLoginAt = now.UtcDateTime,
                        IsAuthorised = false
                    };

                    maybeUser = await this.userService.AddUserAsync(newUser);
                    ValidateUser(maybeUser);
                }
                else
                {
                    maybeUser.LastLoginAt = now.UtcDateTime;
                    maybeUser.RawUserInfo = rawUserInfo;
                    maybeUser = await this.userService.ModifyUserAsync(maybeUser);
                    ValidateUser(maybeUser);
                }

                if (maybeUser.IsAuthorised is false)
                {
                    await this.nhsDigitalApiService.LogoutAsync(cancellationToken);
                }

                return maybeUser;
            });

        public ValueTask LogoutAsync(CancellationToken cancellationToken) =>
            TryCatch(async () => await
                this.nhsDigitalApiService.LogoutAsync(cancellationToken));

        public ValueTask<string> BuildLoginUrlAsync(CancellationToken cancellationToken) =>
            TryCatch(async () => await
                this.nhsDigitalApiService.BuildLoginUrlAsync(cancellationToken));

        public ValueTask<string> GetAccessTokenAsync(CancellationToken cancellationToken) =>
            TryCatch(async () => await
                this.nhsDigitalApiService.GetAccessTokenAsync(cancellationToken));

        public ValueTask<string> SearchPatientPDSAsync(
            SearchCriteria searchCriteria,
            CancellationToken cancellationToken) =>
            TryCatch(async () =>
            {
                ValidateSearchPatientPDSArguments(searchCriteria);

                return await this.nhsDigitalApiService
                    .SearchPatientPDSAsync(searchCriteria, cancellationToken);
            });
    }
}
