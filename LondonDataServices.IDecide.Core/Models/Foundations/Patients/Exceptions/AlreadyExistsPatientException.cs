// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections;
using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Foundations.Patients.Exceptions
{
    public class AlreadyExistsPatientException : Xeption
    {
        public AlreadyExistsPatientException(string message, Exception innerException, IDictionary data)
            : base(message, innerException, data)
        { }
    }
}
