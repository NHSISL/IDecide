// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using LondonDataServices.IDecide.Core.Models.Foundations.Consumers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LondonDataServices.IDecide.Core.Brokers.Storages.Sql
{
    public partial class StorageBroker
    {
        private static void AddConsumerConfigurations(EntityTypeBuilder<Consumer> model)
        {
            model
                .ToTable("Consumers");

            model
                .Property(consumer => consumer.Id)
                .IsRequired();

            model
                .Property(consumer => consumer.EntraId)
                .HasMaxLength(255)
                .IsRequired();

            model
                .Property(consumer => consumer.Name)
                .HasMaxLength(255)
                .IsRequired();

            model
                .HasIndex(consumer => consumer.Name)
                .IsUnique();

            model
                .Property(consumer => consumer.ContactPerson);

            model
                .Property(consumer => consumer.ContactNumber);

            model
                .Property(consumer => consumer.ContactEmail);

            model
                .Property(consumer => consumer.CreatedBy)
                .HasMaxLength(255)
                .IsRequired();

            model
                .Property(consumer => consumer.CreatedDate)
                .IsRequired();

            model
                .Property(consumer => consumer.UpdatedBy)
                .HasMaxLength(255)
                .IsRequired();

            model
                .Property(consumer => consumer.UpdatedDate)
                .IsRequired();
        }
    }
}
