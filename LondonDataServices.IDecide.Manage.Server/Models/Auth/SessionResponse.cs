// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

namespace LondonDataServices.IDecide.Manage.Server.Models.Auth
{
    public class SessionResponse
    {
        public string Sub { get; set; }
        public string Upn { get; set; }
        public string Name { get; set; }
        public string[] Roles { get; set; }
    }
}
