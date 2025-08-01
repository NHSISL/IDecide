// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Foundations.Patients.Exceptions
{
    public class PatientServiceException : Xeption
    {
        public PatientServiceException(string message, Exception innerException)
           : base(message, innerException)
        { }
    }
}
