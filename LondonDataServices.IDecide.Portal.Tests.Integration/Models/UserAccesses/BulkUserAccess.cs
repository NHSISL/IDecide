﻿// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;

namespace LondonDataServices.IDecide.Portal.Tests.Integration.Models.UserAccesses
{
    public class BulkUserAccess
    {
        public string UserId { get; set; }
        public string GivenName { get; set; }
        public string Surname { get; set; }
        public string DisplayName { get; set; }
        public string JobTitle { get; set; }
        public string Email { get; set; }
        public string UserPrincipalName { get; set; }
        public List<string> OrgCodes { get; set; }
    }
}
