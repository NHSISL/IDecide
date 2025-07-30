// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using ISL.Providers.PDS.Abstractions.Models;

namespace LondonDataServices.IDecide.Core.Models.Foundations.Pds
{
    public class PatientLookup
    {
        public SearchCriteria SearchCriteria { get; set; }
        public PatientBundle Patients {  get; set; }
    }
}
