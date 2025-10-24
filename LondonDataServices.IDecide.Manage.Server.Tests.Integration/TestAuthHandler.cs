﻿// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Integration
{
    public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private static Guid securityOid = Guid.Parse("65b5ccfb-b501-4ad5-8dd7-2a33ff64eaa3");
        private static string givenName = "TestGivenName";
        private static string surname = "TesSurname";
        private static string displayName = "TestDisplayName";
        private static string email = "TestEmail@test.com";
        private static string jobTitle = "TestJobTitle";

        public static string TestUserId =>
            securityOid.ToString();

        private static List<Claim> claims = new List<Claim>
        {
            new Claim("oid", securityOid.ToString()),
            new Claim(ClaimTypes.GivenName, givenName),
            new Claim(ClaimTypes.Surname, surname),
            new Claim("displayName", displayName),
            new Claim(ClaimTypes.Email, email),
            new Claim("jobTitle", jobTitle),
            new Claim(ClaimTypes.Name, "TestUser"),
            new Claim(ClaimTypes.Role, "Administrator"),
            new Claim(ClaimTypes.Role, "LondonDataServices.IDecide.Manage.Server.Administrators")
        };

        public TestAuthHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder)
            : base(options, logger, encoder) { }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var identity = new ClaimsIdentity(claims, "TestScheme");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "TestScheme");

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
