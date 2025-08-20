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
        private static void AddConsumerStatusConfigurations(EntityTypeBuilder<ConsumerStatus> model)
        {
            model
                .ToTable("ConsumerStatuses", "Consumer");

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
                .HasOne(consumerStatus => consumerStatus.Consumer)
                .WithMany(consumer => consumer.ConsumerStatuses)
                .HasForeignKey(consumerStatus => consumerStatus.ConsumerId)
                .OnDelete(DeleteBehavior.NoAction);

            model
                .HasOne(consumerStatus => consumerStatus.Decision)
                .WithMany(decision => decision.ConsumerStatuses)
                .HasForeignKey(consumerStatus => consumerStatus.DecisionId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
