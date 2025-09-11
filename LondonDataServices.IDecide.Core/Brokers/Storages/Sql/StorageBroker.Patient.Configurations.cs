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
                .ToTable("Patients");

            model
                .Property(patient => patient.Id)
                .IsRequired();

            model
                .Property(patient => patient.NhsNumber)
                .HasMaxLength(10)
                .IsRequired();

            model
                .Property(patient => patient.Title)
                .HasMaxLength(35);

            model
                .Property(patient => patient.GivenName)
                .HasMaxLength(255)
                .IsRequired();

            model
               .Property(patient => patient.Surname)
               .HasMaxLength(255)
               .IsRequired();

            model
               .Property(patient => patient.DateOfBirth)
               .IsRequired();

            model
               .Property(patient => patient.Gender)
               .HasMaxLength(50)
               .IsRequired();

            model
               .Property(patient => patient.Email)
               .HasMaxLength(255);

            model
               .Property(patient => patient.Phone)
               .HasMaxLength(15);

            model
               .Property(patient => patient.PostCode)
               .HasMaxLength(8);

            model
                .Property(patient => patient.ValidationCode)
                .HasMaxLength(5)
                .IsRequired();

            model
                .Property(patient => patient.ValidationCodeExpiresOn)
                .IsRequired();

            model
                .Property(patient => patient.RetryCount)
                .HasDefaultValue(0)
                .IsRequired();

            model
                .Property(decisionType => decisionType.CreatedBy)
                .HasMaxLength(255)
                .IsRequired();

            model
                .Property(decisionType => decisionType.CreatedDate)
                .IsRequired();

            model
                .Property(decisionType => decisionType.UpdatedBy)
                .HasMaxLength(255)
                .IsRequired();

            model
                .Property(decisionType => decisionType.UpdatedDate)
                .IsRequired();

            model
               .HasMany(patient => patient.Decisions)
               .WithOne(decision => decision.Patient)
               .HasForeignKey(decision => decision.PatientId)
               .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
