// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace LondonDataServices.IDecide.Core.Models.Foundations.NhsDigitalApis.Exceptions
{
    public class NullNhsDigitalApiSearchCriteriaException : Xeption
    {
        public NullNhsDigitalApiSearchCriteriaException(string message)
            : base(message)
        { }
    }
}
