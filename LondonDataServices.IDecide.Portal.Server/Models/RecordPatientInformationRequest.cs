// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

namespace LondonDataServices.IDecide.Portal.Server.Models
{
    public class RecordPatientInformationRequest
    {
        public string NhsNumber { get; set; }
        public string NotificationPreference { get; set; }
        public bool GenerateNewCode { get; set; } = false;
    }
}
