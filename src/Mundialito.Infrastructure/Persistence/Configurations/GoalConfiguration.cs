using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mundialito.Domain.Matches;
using Mundialito.Domain.Teams;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mundialito.Infrastructure.Persistence.Configurations
{
    internal sealed class GoalConfiguration : IEntityTypeConfiguration<Goal>
    {
        public void Configure(EntityTypeBuilder<Goal> builder)
        {
            builder.ToTable("Goals");

            builder.HasKey(g => g.Id);
            builder.Property(g => g.Id).ValueGeneratedNever();

            builder.HasOne<Player>().WithMany().HasForeignKey(g => g.PlayerId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne<Team>().WithMany().HasForeignKey(g => g.TeamId).OnDelete(DeleteBehavior.Restrict);

            builder.Property(g => g.Minute).IsRequired();

            builder.Ignore(g => g.DomainEvents);
        }
    }
}
