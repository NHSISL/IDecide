// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Brokers.Loggings;
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

        public ValueTask ProcessCallbackAsync(string code, string state, CancellationToken cancellationToken)
        {
            // VALIDATION: ValidateProcessCallbackArguments(code, state)
            //             - ValidateCodeIsNotNullOrWhiteSpace(code)
            //             - ValidateStateIsNotNullOrWhiteSpace(state)

            // Step 1: Call NhsDigitalApiService.GetUserInfoAsync(code, state, cancellationToken)
            //         to exchange the authorisation code for user info.

            // VALIDATION: ValidateUserInfoIsNotNull(userInfo)
            //             - ValidateUserInfoNhsIdUserUidIsNotNullOrWhiteSpace(userInfo.NhsIdUserUid)
            //             - ValidateUserInfoNameIsNotNullOrWhiteSpace(userInfo.Name)
            //             - ValidateUserInfoSubIsNotNullOrWhiteSpace(userInfo.Sub)

            // Step 2: Serialise the returned user info to JSON for storage as RawUserInfo.

            // Step 3: Call UserService.RetrieveAllUsersAsync() and find a User
            //         where NhsIdUserUid matches userInfo.NhsIdUserUid.

            // Step 4: If no matching user found (new user):
            //             Create a new User with NhsIdUserUid, Name, Sub, RawUserInfo,
            //             LastLoginAt = UtcNow, IsAuthorised = false.
            //             Call UserService.AddUserAsync(user).

            // Step 5: If matching user found (existing user):
            //             Update user.LastLoginAt = UtcNow and user.RawUserInfo.
            //             Call UserService.ModifyUserAsync(user).

            // VALIDATION: ValidateUserIsNotNull(user)
            //             - Ensure AddUserAsync / ModifyUserAsync returned a valid User.

            // Step 6: If user.IsAuthorised is false:
            //             Call NhsDigitalApiService.LogoutAsync(cancellationToken).
            //             Return a result indicating the user is unauthorised
            //             (controller will clear session, sign out, and redirect to /unauthorised).

            // Step 7: If user.IsAuthorised is true:
            //             Return a result containing the user info (Name, NhsIdUserUid, Sub)
            //             so the controller can build claims, sign in, and redirect to /.

            throw new NotImplementedException();
        }
    }
}
