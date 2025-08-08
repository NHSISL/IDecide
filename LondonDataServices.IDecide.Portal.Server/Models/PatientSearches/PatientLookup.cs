// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;

namespace LondonDataServices.IDecide.Portal.Server.Models.PatientSearches
{
    public class PatientLookup
    {
        public SearchCriteria SearchCriteria { get; set; }
        public List<Patient> Patients { get; set; }
    }
}
