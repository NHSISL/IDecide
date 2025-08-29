// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

namespace LondonDataServices.IDecide.Portal.Tests.Integration.Models.ReIdentifications
{
    public class IdentificationItem
    {
        public string RowNumber { get; set; } = string.Empty;
        public string Identifier { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool HasAccess { get; set; } = false;
        public bool IsReidentified { get; set; } = false;
    }
}
