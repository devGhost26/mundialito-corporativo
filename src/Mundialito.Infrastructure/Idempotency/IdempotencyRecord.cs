using System;
using System.Collections.Generic;
using System.Text;

namespace Mundialito.Infrastructure.Idempotency
{
    internal sealed class IdempotencyRecord
    {
        public const int KeyMaxLength = 100;

        public string Key { get; set; } = string.Empty;
        public string RequestHash { get; set; } = string.Empty;
        public int? StatusCode { get; set; }
        public string? ContentType { get; set; }
        public string? ResponseBody { get; set; }
        public string? Location { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? CompletedAtUtc { get; set; }
    }
}
