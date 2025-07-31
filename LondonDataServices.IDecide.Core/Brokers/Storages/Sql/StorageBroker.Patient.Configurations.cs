// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using LondonDataServices.IDecide.Core.Models.Foundations.Patients;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LondonDataServices.IDecide.Core.Brokers.Storages.Sql
{
    public partial class StorageBroker
    {
        private static void AddPatientConfigurations(EntityTypeBuilder<Patient> model)
        {
            model
                .ToTable("Patients", "Decision");

            model
                .Property(patient => patient.Id)
                .IsRequired();

            model
                .Property(patient => patient.NhsNumber)
                .HasMaxLength(10)
                .IsRequired();

            model
                .Property(patient => patient.ValidationCode)
                .HasMaxLength(5)
                .IsRequired();

            model
                .Property(patient => patient.ValidationCodeExpiresOn)
                .IsRequired();

            model
                .Property(patient => patient.RetryCount)
                .IsRequired();

            model
               .HasMany(patient => patient.Decisions)
               .WithOne(decision => decision.Patient)
               .HasForeignKey(decision => decision.PatientId)
               .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
