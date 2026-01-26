// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using LondonDataServices.IDecide.Core.Models.Foundations.NhsLogins;
using LondonDataServices.IDecide.Core.Models.Foundations.NhsLogins.Exceptions;

namespace LondonDataServices.IDecide.Core.Services.Foundations.NhsLogins
{
    public partial class NhsLoginService
    {
        private static void ValidateAccessToken(string accessToken)
        {
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                throw new NullNhsLoginException(
                    message: "Access token is required.");
            }
        }

        private void ValidateSuccessStatusCode(NhsLoginUserInfo userInfo)
        {
            if (userInfo is null)
            {
                throw new NhsLoginUserInfoException(
                    message: "NHS Login userinfo endpoint did not return a successful response.");
            }
        }
    }
}
