// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Foundations.Patients.Exceptions
{
    public class InvalidPatientException : Xeption
    {
        public InvalidPatientException(string message)
            : base(message)
        { }
    }
}