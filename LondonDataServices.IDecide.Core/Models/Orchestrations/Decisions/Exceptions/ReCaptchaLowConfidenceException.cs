// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Orchestrations.Decisions.Exceptions
{
    public class ReCaptchaLowConfidenceException : Xeption
    {
        public ReCaptchaLowConfidenceException(string message)
            : base(message)
        { }
    }
}
