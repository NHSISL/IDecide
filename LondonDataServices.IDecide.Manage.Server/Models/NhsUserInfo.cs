// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LondonDataServices.IDecide.Manage.Server.Models
{
    public class NhsUserInfo
    {
        [JsonPropertyName("nhsid_useruid")]
        public string NhsIdUserUid { get; set; } = default!;

        [JsonPropertyName("name")]
        public string Name { get; set; } = default!;

        [JsonPropertyName("nhsid_nrbac_roles")]
        public List<NhsNrbacRole> NhsIdNrbacRoles { get; set; } = new();

        [JsonPropertyName("sub")]
        public string Sub { get; set; } = default!;
    }

    public class NhsNrbacRole
    {
        [JsonPropertyName("person_orgid")]
        public string PersonOrgId { get; set; } = default!;

        [JsonPropertyName("person_roleid")]
        public string PersonRoleId { get; set; } = default!;

        [JsonPropertyName("org_code")]
        public string OrgCode { get; set; } = default!;

        [JsonPropertyName("role_name")]
        public string RoleName { get; set; } = default!;

        [JsonPropertyName("role_code")]
        public string RoleCode { get; set; } = default!;

        [JsonPropertyName("activities")]
        public List<string> Activities { get; set; } = new();

        [JsonPropertyName("activity_codes")]
        public List<string> ActivityCodes { get; set; } = new();
    }
}
