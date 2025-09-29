// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

namespace LondonDataServices.IDecide.Manage.Server.Models
{
    public class PatientCodeRequest
    {
        public string NhsNumber { get; set; }
        public string VerificationCode { get; set; }
        public string NotificationPreference { get; set; }
        public bool GenerateNewCode { get; set; } = false;
    }
}
