// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Foundations.Patients.Exceptions
{
    public class NullPatientException : Xeption
    {
        public NullPatientException(string message)
            : base(message)
        { }
    }
}