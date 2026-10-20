using System;
using System.Collections.Generic;
using System.Text;

namespace Mundialito.Application.Abstractions
{
    public sealed record IdempotentResponse(
        string RequestHash,
        bool IsCompleted,
        int? StatusCode,
        string? ContentType,
        string? Body,
        string? Location);
}
