using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LondonDataServices.IDecide.Core.Models.Foundations.NhsLogins
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
