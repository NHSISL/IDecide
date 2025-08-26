// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

namespace LondonDataServices.IDecide.Core.Models.Orchestrations.Decisions
{
    public class DecisionConfigurations
    {
        public int MaxRetryCount { get; set; }
        public int PatientValidationCodeExpireAfterMinutes { get; set; }
    }
}