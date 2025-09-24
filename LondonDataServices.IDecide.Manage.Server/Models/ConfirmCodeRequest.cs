// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

namespace LondonDataServices.IDecide.Portal.Server.Models
{
    public class ConfirmCodeRequest
    {
        public string NhsNumber { get; set; }
        public string Code { get; set; }
    }
}
