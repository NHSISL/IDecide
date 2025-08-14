// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using LondonDataServices.IDecide.Core.Models.Foundations.Decisions;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace LondonDataServices.IDecide.Core.Models.Foundations.Notifications
{
    public class NotificationInfo
    {
        [BindNever]
        public Decision Decision { get; set; } = null!;

        [BindNever]
        public Patient Patient { get; set; } = null!;
    }
}
