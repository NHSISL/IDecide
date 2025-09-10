// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;

namespace LondonDataServices.IDecide.Core.Models.Orchestrations.Decisions
{
    public class DecisionConfigurations
    {
        public int MaxRetryCount { get; set; }
        public int PatientValidationCodeExpireAfterMinutes { get; set; }
        public int ValidatedCodeValidForMinutes { get; set; }
        public List<string> DecisionWorkflowRoles { get; set; }
        public string AgentOverrideCode { get; set; }
    }
}
