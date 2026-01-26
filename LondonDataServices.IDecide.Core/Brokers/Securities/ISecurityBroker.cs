// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using ISL.Providers.Captcha.Abstractions.Models;
using LondonDataServices.IDecide.Core.Models.Foundations.NhsLogins;
using LondonDataServices.IDecide.Core.Models.Securities;

namespace LondonDataServices.IDecide.Core.Brokers.Securities
{
    public interface ISecurityBroker
    {
        ValueTask<User> GetCurrentUserAsync();
        ValueTask<bool> IsCurrentUserAuthenticatedAsync();
        ValueTask<bool> IsInRoleAsync(string roleName);
        ValueTask<bool> HasClaimAsync(string claimType, string claimValue);
        ValueTask<bool> HasClaimAsync(string claimType);
        ValueTask<CaptchaResult> ValidateCaptchaAsync();
        ValueTask<string> GetIpAddressAsync();
        ValueTask<string> GetHeaderAsync(string key);
        ValueTask<string> GetNhsLoginAccessTokenAsync();
        ValueTask<NhsLoginUserInfo> GetNhsLoginUserInfoAsync(string accessToken);
    }
}
