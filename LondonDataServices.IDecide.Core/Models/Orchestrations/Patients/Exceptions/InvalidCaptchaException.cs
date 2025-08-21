// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Orchestrations.Patients.Exceptions
{
    public class InvalidCaptchaException : Xeption
    {
        public InvalidCaptchaException(string message)
            : base(message)
        { }
    }
}
