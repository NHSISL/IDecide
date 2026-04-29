// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Text.Json.Serialization;

namespace LondonDataServices.IDecide.Core.Models.Orchestrations.NhsDigitalApis
{
    public class NhsDigitalUserInfo
    {
        [JsonPropertyName("nhsid_useruid")]
        public string NhsIdUserUid { get; set; } = default!;
        [JsonPropertyName("name")]
        public string Name { get; set; } = default!;
        [JsonPropertyName("sub")]
        public string Sub { get; set; } = default!;
    }
}
