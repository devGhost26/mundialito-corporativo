using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mundialito.Domain.Teams;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mundialito.Infrastructure.Persistence.Configurations
{
    internal sealed class PlayerConfiguration : IEntityTypeConfiguration<Player>
    {
        public void Configure(EntityTypeBuilder<Player> builder)
        {
            builder.ToTable("Players");

            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).ValueGeneratedNever();

            builder.Property(p => p.FirstName).HasMaxLength(Player.NameMaxLength).IsRequired();
            builder.Property(p => p.LastName).HasMaxLength(Player.NameMaxLength).IsRequired();
            builder.Property(p => p.Number).IsRequired();
            builder.Property(p => p.Position).IsRequired();

            builder.HasIndex(p => new { p.TeamId, p.Number }).IsUnique();

            builder.Ignore(p => p.DomainEvents);
        }
    }
}
