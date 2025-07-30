// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using LondonDataServices.IDecide.Core.Models.Foundations.Decisions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LondonDataServices.IDecide.Core.Brokers.Storages.Sql
{
    public partial class StorageBroker
    {
        private static void AddDecisionConfigurations(EntityTypeBuilder<Decision> model)
        {
            model
                .ToTable("Decisions", "Decision");

            model
                .Property(decision => decision.Id)
                .IsRequired();

            model
                .Property(decision => decision.PatientNhsNumber)
                .HasMaxLength(255)
                .IsRequired();

            model
                .Property(decision => decision.DecisionChoice)
                .HasMaxLength(255)
                .IsRequired();

            model
                .Property(decision => decision.CreatedBy)
                .HasMaxLength(255)
                .IsRequired();

            model
                .Property(decision => decision.CreatedDate)
                .IsRequired();

            model
                .Property(decision => decision.UpdatedBy)
                .HasMaxLength(255)
                .IsRequired();

            model
                .Property(decision => decision.UpdatedDate)
                .IsRequired();

            model
               .Property(decision => decision.DecisionTypeId)
                .IsRequired();

            model
                .HasOne(decision => decision.DecisionType)
                .WithMany(decisionType => decisionType.Decisions)
                .HasForeignKey(decision => decision.DecisionTypeId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
