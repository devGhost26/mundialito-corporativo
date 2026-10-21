using Mundialito.Application.Abstractions;
using Mundialito.Domain.Common;
using System.Diagnostics;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace Mundialito.Infrastructure.Events
{
    internal sealed class DomainEventRecord
    {
        private static readonly JsonSerializerOptions PayloadOptions = new()
        {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
        };

        public Guid Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Payload { get; set; } = string.Empty;
        public DateTime OccurredOnUtc { get; set; }
        public string? TraceId { get; set; }
        public string? CorrelationId { get; set; }

        public static DomainEventRecord From(IDomainEvent domainEvent, Activity? activity) => new()
        {
            Id = domainEvent.EventId,
            Type = domainEvent.GetType().Name,
            Payload = JsonSerializer.Serialize(domainEvent, domainEvent.GetType(), PayloadOptions),
            OccurredOnUtc = domainEvent.OccurredOnUtc,
            TraceId = activity?.TraceId.ToString(),
            CorrelationId = activity?.GetBaggageItem(CorrelationContext.BaggageKey)
        };
    }
}
