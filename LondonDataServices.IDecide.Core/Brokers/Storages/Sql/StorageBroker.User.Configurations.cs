// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using LondonDataServices.IDecide.Core.Models.Foundations.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LondonDataServices.IDecide.Core.Brokers.Storages.Sql
{
    public partial class StorageBroker
    {
        private static void AddUserConfigurations(EntityTypeBuilder<User> model)
        {
            model
                .ToTable("Users");

            model
                .Property(user => user.Id)
                .IsRequired();

            model
                .Property(user => user.NhsIdUserUid)
                .HasMaxLength(50)
                .IsRequired();

            model
                .HasIndex(user => user.NhsIdUserUid)
                .IsUnique();

            model
                .Property(user => user.Name)
                .HasMaxLength(200)
                .IsRequired();

            model
                .Property(user => user.Sub)
                .IsRequired();

            model
                .Property(user => user.RawUserInfo)
                .IsRequired(false);

            model
                .Property(user => user.CreatedAt)
                .IsRequired();

            model
                .Property(user => user.LastLoginAt)
                .IsRequired(false);
        }
    }
}