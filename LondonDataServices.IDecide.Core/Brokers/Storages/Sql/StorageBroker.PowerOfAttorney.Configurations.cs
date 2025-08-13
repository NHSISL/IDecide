// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Foundations.PowersOfAttorney;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LondonDataServices.IDecide.Core.Brokers.Storages.Sql
{
    public partial class StorageBroker
    {
        private static void AddPowerOfAttorneyConfigurations(EntityTypeBuilder<PowerOfAttorney> model)
        {
            model
                .ToTable("PowersOfAttorney", "PowerOfAttorney");

            model
                .Property(powerOfAttorney => powerOfAttorney.Id)
                .IsRequired();

            model
                .Property(powerOfAttorney => powerOfAttorney.PatientId)
                .IsRequired();

            model
                .Property(powerOfAttorney => powerOfAttorney.Title)
                .HasMaxLength(10)
                .IsRequired();

            model
                .Property(powerOfAttorney => powerOfAttorney.FirstName)
                .HasMaxLength(255)
                .IsRequired();

            model
                .Property(powerOfAttorney => powerOfAttorney.Surname)
                .HasMaxLength(255)
                .IsRequired();

            model
                .Property(powerOfAttorney => powerOfAttorney.Telephone)
                .HasMaxLength(20)
                .IsRequired();

            model
                .Property(powerOfAttorney => powerOfAttorney.Email)
                .HasMaxLength(255)
                .IsRequired();

            model
                .Property(powerOfAttorney => powerOfAttorney.RelationshipType)
                .HasMaxLength(255)
                .IsRequired();

            model
                .HasOne(powerOfAttorney => powerOfAttorney.Patient)
                .WithOne(patient => patient.PowerOfAttorney)
                .HasForeignKey<PowerOfAttorney>(powerOfAttorney => powerOfAttorney.PatientId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
