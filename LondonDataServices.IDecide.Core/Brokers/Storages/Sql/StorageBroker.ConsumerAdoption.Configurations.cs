// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using LondonDataServices.IDecide.Core.Models.Foundations.ConsumerAdoptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LondonDataServices.IDecide.Core.Brokers.Storages.Sql
{
    public partial class StorageBroker
    {
        private static void AddConsumerAdoptionConfigurations(EntityTypeBuilder<ConsumerAdoption> model)
        {
            model
                .ToTable("ConsumerAdoptions");

            model
                .Property(status => status.Id)
                .IsRequired();

            model
                .Property(status => status.ConsumerId)
                .IsRequired();

            model
                .Property(status => status.DecisionId)
                .IsRequired();

            model
                .HasIndex(status => new { status.ConsumerId, status.DecisionId })
                .IsUnique();

            model
                .Property(status => status.CreatedBy)
                .HasMaxLength(255)
                .IsRequired();

            model
                .Property(status => status.AdoptionDate)
                .IsRequired();

            model
                .Property(status => status.CreatedDate)
                .IsRequired();

            model
                .Property(status => status.UpdatedBy)
                .HasMaxLength(255)
                .IsRequired();

            model
                .Property(status => status.UpdatedDate)
                .IsRequired();

            model
                .HasOne(consumerAdoption => consumerAdoption.Consumer)
                .WithMany(consumer => consumer.ConsumerAdoptions)
                .HasForeignKey(consumerAdoption => consumerAdoption.ConsumerId)
                .OnDelete(DeleteBehavior.NoAction);

            model
                .HasOne(consumerAdoption => consumerAdoption.Decision)
                .WithMany(decision => decision.ConsumerAdoptions)
                .HasForeignKey(consumerAdoption => consumerAdoption.DecisionId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
