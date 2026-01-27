// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using LondonDataServices.IDecide.Core.Models.Foundations.NhsLogins;
using LondonDataServices.IDecide.Core.Models.Foundations.NhsLogins.Exceptions;
using Xeptions;

namespace LondonDataServices.IDecide.Core.Services.Foundations.NhsLogins
{
    public partial class NhsLoginService
    {
        private static void ValidateAccessToken(string accessToken)
        {
            Validate(
                createException: () => new InvalidArgumentsNhsLoginServiceException(
                    message: "Invalid NHS Login argument. Please correct the errors and try again."),
                (Rule: IsInvalidAccessToken(accessToken), Parameter: nameof(accessToken)));
        }

        private static void ValidateSuccessStatusCode(NhsLoginUserInfo userInfo)
        {
            if (userInfo is null)
            {
                throw new NhsLoginNullResponseException(
                    message: "NHS Login userinfo endpoint did not return a successful response.");
            }
        }

        private static dynamic IsInvalidAccessToken(string accessToken) => new
        {
            Condition = string.IsNullOrWhiteSpace(accessToken),
            Message = "Access token is required."
        };

        private static void Validate<T>(  
            Func<T> createException,  
            params (dynamic Rule, string Parameter)[] validations)  
            where T : Xeption  
        {  
            T invalidDataException = createException();  

            foreach ((dynamic rule, string parameter) in validations)  
            {  
                if (rule.Condition)  
                {  
                    invalidDataException.UpsertDataList(  
                        key: parameter,  
                        value: rule.Message);  
                }  
            }  

            invalidDataException.ThrowIfContainsErrors();  
        }  
    }
}
