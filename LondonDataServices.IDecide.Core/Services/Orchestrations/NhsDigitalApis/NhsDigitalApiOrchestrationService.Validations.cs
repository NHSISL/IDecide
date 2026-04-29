// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using LondonDataServices.IDecide.Core.Models.Foundations.Users;
using LondonDataServices.IDecide.Core.Models.Orchestrations.NhsDigitalApis;

namespace LondonDataServices.IDecide.Core.Services.Orchestrations.NhsDigitalApis
{
    public partial class NhsDigitalApiOrchestrationService
    {
        private static void ValidateProcessCallbackArguments(string code, string state)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                throw new ArgumentNullException(nameof(code), "Code is required.");
            }

            if (string.IsNullOrWhiteSpace(state))
            {
                throw new ArgumentNullException(nameof(state), "State is required.");
            }
        }

        private static void ValidateUserInfo(NhsDigitalUserInfo userInfo)
        {
            if (userInfo is null)
            {
                throw new ArgumentNullException(nameof(userInfo), "User info is null.");
            }

            if (string.IsNullOrWhiteSpace(userInfo.NhsIdUserUid))
            {
                throw new ArgumentException("NhsIdUserUid is required.", nameof(userInfo));
            }

            if (string.IsNullOrWhiteSpace(userInfo.Name))
            {
                throw new ArgumentException("Name is required.", nameof(userInfo));
            }

            if (string.IsNullOrWhiteSpace(userInfo.Sub))
            {
                throw new ArgumentException("Sub is required.", nameof(userInfo));
            }
        }

        private static void ValidateUser(User user)
        {
            if (user is null)
            {
                throw new ArgumentNullException(nameof(user), "User is null.");
            }
        }
    }
}
