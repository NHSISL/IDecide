// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using LondonDataServices.IDecide.Core.Models.Foundations.NhsDigitalApis.Exceptions;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.Pds;
using Xeptions;

namespace LondonDataServices.IDecide.Core.Services.Foundations.NhsDigitalApis
{
    public partial class NhsDigitalApiService
    {
        private static void ValidateSearchCriteriaIsNotNull(SearchCriteria searchCriteria)
        {
            if (searchCriteria is null)
            {
                throw new NullNhsDigitalApiSearchCriteriaException(
                    message: "Search criteria is null.");
            }
        }

        private static void ValidateGetUserInfoArguments(string code, string state)
        {
            Validate(
                createException: () => new InvalidNhsDigitalApiUserInfoException(
                    message: "Invalid NhsDigitalApi user info arguments. " +
                        "Please correct the errors and try again."),
                (Rule: IsInvalid(code), Parameter: nameof(code)),
                (Rule: IsInvalid(state), Parameter: nameof(state)));
        }

        private static dynamic IsInvalid(string value) => new
        {
            Condition = string.IsNullOrWhiteSpace(value),
            Message = "Value is required."
        };

        private static void Validate<T>(
            Func<T> createException,
            params (dynamic Rule, string Parameter)[] validations)
            where T : Xeption
        {
            T invalidException = createException();

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

