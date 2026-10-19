using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mundialito.Domain.Matches;
using Mundialito.Domain.Teams;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mundialito.Infrastructure.Persistence.Configurations
{
    internal sealed class MatchConfiguration : IEntityTypeConfiguration<Match>
    {
        public void Configure(EntityTypeBuilder<Match> builder)
        {
            builder.ToTable("Matches");

            builder.HasKey(m => m.Id);
            builder.Property(m => m.Id).ValueGeneratedNever();

            builder.HasOne<Team>().WithMany().HasForeignKey(m => m.HomeTeamId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne<Team>().WithMany().HasForeignKey(m => m.AwayTeamId).OnDelete(DeleteBehavior.Restrict);

            builder.Property(m => m.ScheduledAtUtc)
                .IsRequired()
                .HasConversion(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

            builder.Property(m => m.Venue).HasMaxLength(Match.VenueMaxLength);
            builder.Property(m => m.Status).IsRequired();
            builder.Property(m => m.HomeGoals);
            builder.Property(m => m.AwayGoals);

            builder.HasMany(m => m.Goals)
                .WithOne()
                .HasForeignKey(g => g.MatchId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(m => m.Goals).UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.HasIndex(m => m.ScheduledAtUtc);
            builder.HasIndex(m => m.Status);

            builder.Ignore(m => m.DomainEvents);
        }
    }
}
