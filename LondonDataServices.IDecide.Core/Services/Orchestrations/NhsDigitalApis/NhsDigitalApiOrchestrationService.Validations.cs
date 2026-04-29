// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using LondonDataServices.IDecide.Core.Models.Foundations.Users;
using LondonDataServices.IDecide.Core.Models.Orchestrations.NhsDigitalApis;
using LondonDataServices.IDecide.Core.Models.Orchestrations.NhsDigitalApis.Exceptions;

namespace LondonDataServices.IDecide.Core.Services.Orchestrations.NhsDigitalApis
{
    public partial class NhsDigitalApiOrchestrationService
    {
        private static void ValidateProcessCallbackArguments(string code, string state)
        {
            Validate(
                createException: () => new InvalidNhsDigitalApiOrchestrationArgumentException(
                    message: "Invalid NhsDigitalApi orchestration argument. " +
                        "Please correct the errors and try again."),
                (Rule: IsInvalid(code), Parameter: nameof(code)),
                (Rule: IsInvalid(state), Parameter: nameof(state)));
        }

        private static void ValidateUserInfo(NhsDigitalUserInfo userInfo)
        {
            Validate(
                createException: () => new InvalidNhsDigitalApiOrchestrationArgumentException(
                    message: "Invalid NhsDigitalApi orchestration argument. " +
                        "Please correct the errors and try again."),
                (Rule: IsInvalid(userInfo?.NhsIdUserUid), Parameter: nameof(NhsDigitalUserInfo.NhsIdUserUid)),
                (Rule: IsInvalid(userInfo?.Name), Parameter: nameof(NhsDigitalUserInfo.Name)),
                (Rule: IsInvalid(userInfo?.Sub), Parameter: nameof(NhsDigitalUserInfo.Sub)));
        }

        private static void ValidateUser(User user)
        {
            if (user is null)
            {
                throw new InvalidNhsDigitalApiOrchestrationArgumentException(
                    message: "Invalid NhsDigitalApi orchestration argument. " +
                        "Please correct the errors and try again.");
            }
        }

        private static dynamic IsInvalid(string value) => new
        {
            Condition = string.IsNullOrWhiteSpace(value),
            Message = "Value is required."
        };

        private static void Validate(
            Func<InvalidNhsDigitalApiOrchestrationArgumentException> createException,
            params (dynamic Rule, string Parameter)[] validations)
        {
            InvalidNhsDigitalApiOrchestrationArgumentException invalidException = createException();

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidException.ThrowIfContainsErrors();
        }
    }
}
