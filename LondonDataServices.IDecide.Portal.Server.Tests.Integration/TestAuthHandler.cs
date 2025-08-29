// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LondonDataServices.IDecide.Portal.Server.Tests.Integration
{
    public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public static string SecurityOid = "65b5ccfb-b501-4ad5-8dd7-2a33ff64eaa3";
        public static string GivenName = "TestGivenName";
        public static string Surname = "TesSurname";
        public static string DisplayName = "TestDisplayName";
        public static string Email = "TestEmail@test.com";
        public static string JobTitle = "TestJobTitle";

        public static List<Claim> Claims = new List<Claim>
        {
            new Claim("oid", SecurityOid.ToString()),
            new Claim(ClaimTypes.GivenName, GivenName),
            new Claim(ClaimTypes.Surname, Surname),
            new Claim("displayName", DisplayName),
            new Claim(ClaimTypes.Email, Email),
            new Claim("jobTitle", JobTitle),
            new Claim(ClaimTypes.Name, "TestUser"),
            new Claim(ClaimTypes.Role, "Administrators")
        };

        public TestAuthHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder)
            : base(options, logger, encoder) { }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var identity = new ClaimsIdentity(Claims, "TestScheme");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "TestScheme");

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
