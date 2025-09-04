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
                .ToTable("Consumers", "Consumer");

            model
                .Property(consumer => consumer.Id)
                .IsRequired();

            model
                .Property(consumer => consumer.Name)
                .HasMaxLength(255)
                .IsRequired();

            model
                .HasIndex(consumer => consumer.Name)
                .IsUnique();

            model
                .Property(consumer => consumer.AccessToken)
                .HasMaxLength(36)
                .IsRequired();

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
