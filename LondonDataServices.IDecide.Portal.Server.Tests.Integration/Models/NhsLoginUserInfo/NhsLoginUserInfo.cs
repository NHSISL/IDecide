// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Text.Json.Serialization;

namespace LondonDataServices.IDecide.Portal.Server.Tests.Integration.Models.NhsLoginUserInfo
{
    public class NhsLoginUserInfo
    {
        [JsonPropertyName("birthdate")]
        public DateTime Birthdate { get; set; }

        [JsonPropertyName("family_name")]
        public string FamilyName { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("phone_number")]
        public string PhoneNumber { get; set; }

        [JsonPropertyName("given_name")]
        public string GivenName { get; set; }
    }
}