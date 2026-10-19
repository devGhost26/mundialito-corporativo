using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mundialito.Domain.Teams;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mundialito.Infrastructure.Persistence.Configurations
{
    internal sealed class TeamConfiguration : IEntityTypeConfiguration<Team>
    {
        public void Configure(EntityTypeBuilder<Team> builder)
        {
            builder.ToTable("Teams");

            builder.HasKey(t => t.Id);
            builder.Property(t => t.Id).ValueGeneratedNever();

            builder.Property(t => t.Name).HasMaxLength(Team.NameMaxLength).IsRequired();
            builder.HasIndex(t => t.Name).IsUnique();

            builder.Property(t => t.City).HasMaxLength(Team.CityMaxLength);
            builder.Property(t => t.CreatedAtUtc).IsRequired();

            builder.HasMany(t => t.Players)
                .WithOne()
                .HasForeignKey(p => p.TeamId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(t => t.Players).UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.Ignore(t => t.DomainEvents);
        }
    }
}
