// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using LondonDataServices.IDecide.Core.Models.Foundations.DecisionTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LondonDataServices.IDecide.Core.Brokers.Storages.Sql
{
    public partial class StorageBroker
    {
        private static void AddDecisionTypeConfigurations(EntityTypeBuilder<DecisionType> model)
        {
            model
                .ToTable("DecisionTypes");

            model
                .Property(decisionType => decisionType.Id)
                .IsRequired();

            model
                .Property(decisionType => decisionType.Name)
                .HasMaxLength(255)
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
        }
    }
}
