// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using LondonDataServices.IDecide.Core.Models.Foundations.Decisions;
using LondonDataServices.IDecide.Core.Models.Foundations.Notifications;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace LondonDataServices.IDecide.Core.Models.Foundations.Patients
{
    public class Patient
    {
        public Guid Id { get; set; }
        public string NhsNumber { get; set; }
        public string Title { get; set; }
        public string GivenName { get; set; }
        public string Surname { get; set; }
        public DateTimeOffset DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string PostCode { get; set; }

        [NotMapped]
        public Address PostalAddress
        {
            get
            {
                var addressLines = (Address ?? string.Empty).Split(',');
                var addressLine1 = addressLines.ElementAtOrDefault(0) ?? string.Empty;
                var addressLine2 = addressLines.ElementAtOrDefault(1) ?? string.Empty;
                var addressLine3 = addressLines.ElementAtOrDefault(2) ?? string.Empty;
                var addressLine4 = addressLines.ElementAtOrDefault(3) ?? string.Empty;
                var addressLine5 = addressLines.ElementAtOrDefault(4) ?? string.Empty;

                return new Address
                {
                    RecipientName = $"{Title} {GivenName} {Surname}",
                    AddressLine1 = addressLine1,
                    AddressLine2 = addressLine2,
                    AddressLine3 = addressLine3,
                    AddressLine4 = addressLine4,
                    AddressLine5 = addressLine5,
                    PostCode = PostCode
                };
            }
        }

        public string ValidationCode { get; set; }
        public DateTimeOffset ValidationCodeExpiresOn { get; set; }
        public DateTimeOffset? ValidationCodeMatchedOn { get; set; }
        public int RetryCount { get; set; }
        public NotificationPreference NotificationPreference { get; set; }
        public string CreatedBy { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }

        [NotMapped]
        [JsonIgnore]
        public bool IsSensitive { get; set; }

        [BindNever]
        public List<Decision> Decisions { get; set; } = new List<Decision>();
    }
}
