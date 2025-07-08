// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Foundations.Audits;

namespace LondonDataServices.IDecide.Core.Clients.Audits
{
    public interface IAuditClient
    {
        ValueTask<Audit> LogAuditAsync(
            string auditType,
            string title,
            string message,
            string fileName,
            string correlationId,
            string logLevel = "Information");

        ValueTask BulkLogAuditsAsync(List<Audit> audits);
    }
}
