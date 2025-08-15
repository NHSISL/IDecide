// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Foundations.Patients.Exceptions
{
    public class PatientValidationException : Xeption
    {
        public PatientValidationException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}