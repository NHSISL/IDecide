// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Securities;

namespace LondonDataServices.IDecide.Core.Brokers.Securities
{
    public interface ISecurityBroker
    {
        ValueTask<User> GetCurrentUserAsync();
        ValueTask<bool> IsCurrentUserAuthenticatedAsync();
        ValueTask<bool> IsInRoleAsync(string roleName);
        ValueTask<bool> HasClaimTypeAsync(string claimType, string claimValue);
        ValueTask<bool> HasClaimTypeAsync(string claimType);
        ValueTask<bool> ValidateCaptchaAsync();
        ValueTask<string> GetIpAddressAsync();
    }
}
