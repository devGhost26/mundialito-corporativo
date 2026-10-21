using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mundialito.Infrastructure.Events
{
    internal sealed class DomainEventRecordConfiguration : IEntityTypeConfiguration<DomainEventRecord>
    {
        public void Configure(EntityTypeBuilder<DomainEventRecord> builder)
        {
            builder.ToTable("DomainEvents");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).ValueGeneratedNever();

            builder.Property(e => e.Type).HasMaxLength(200).IsRequired();
            builder.Property(e => e.Payload).IsRequired();
            builder.Property(e => e.OccurredOnUtc).IsRequired();
            builder.Property(e => e.TraceId).HasMaxLength(32);
            builder.Property(e => e.CorrelationId).HasMaxLength(64);

            builder.HasIndex(e => e.OccurredOnUtc);
            builder.HasIndex(e => e.TraceId);
        }
    }
}
