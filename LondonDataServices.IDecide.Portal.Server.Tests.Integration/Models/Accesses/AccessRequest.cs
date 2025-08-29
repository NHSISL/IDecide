// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using LondonDataServices.IDecide.Portal.Tests.Integration.Models.CsvIdentificationRequests;
using LondonDataServices.IDecide.Portal.Tests.Integration.Models.ImpersonationContexts;
using LondonDataServices.IDecide.Portal.Tests.Integration.Models.ReIdentifications;

namespace LondonDataServices.IDecide.Portal.Tests.Integration.Models.Accesses
{
    public class AccessRequest
    {
        public IdentificationRequest IdentificationRequest { get; set; }
        public CsvIdentificationRequest CsvIdentificationRequest { get; set; }
        public ImpersonationContext ImpersonationContext { get; set; }
    }
}
