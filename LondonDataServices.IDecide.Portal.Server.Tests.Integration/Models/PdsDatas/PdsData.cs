// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;

namespace LondonDataServices.IDecide.Portal.Tests.Integration.Models.PdsDatas
{
    public class PdsData
    {
        public Guid Id { get; set; }
        public string PseudoNhsNumber { get; set; }
        public string OrgCode { get; set; }
        public string OrganisationName { get; set; }
        public DateTimeOffset? RelationshipWithOrganisationEffectiveFromDate { get; set; }
        public DateTimeOffset? RelationshipWithOrganisationEffectiveToDate { get; set; }
    }
}
