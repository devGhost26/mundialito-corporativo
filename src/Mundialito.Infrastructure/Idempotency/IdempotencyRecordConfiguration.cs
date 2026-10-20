using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mundialito.Infrastructure.Idempotency
{
    internal sealed class IdempotencyRecordConfiguration : IEntityTypeConfiguration<IdempotencyRecord>
    {
        public void Configure(EntityTypeBuilder<IdempotencyRecord> builder)
        {
            builder.ToTable("IdempotencyRecords");

            builder.HasKey(r => r.Key);
            builder.Property(r => r.Key).HasMaxLength(IdempotencyRecord.KeyMaxLength);

            builder.Property(r => r.RequestHash).HasMaxLength(64).IsRequired();
            builder.Property(r => r.ContentType).HasMaxLength(100);
            builder.Property(r => r.Location).HasMaxLength(300);
            builder.Property(r => r.CreatedAtUtc).IsRequired();
        }
    }
}
