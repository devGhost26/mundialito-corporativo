using System;
using System.Collections.Generic;
using System.Text;

namespace Mundialito.Application.Abstractions
{
    public interface IIdempotencyStore
    {
        Task<IdempotentResponse?> GetAsync(string key, CancellationToken cancellationToken = default);

        Task<bool> TryReserveAsync(string key, string requestHash, CancellationToken cancellationToken = default);

        Task CompleteAsync(
            string key,
            int statusCode,
            string? contentType,
            string? body,
            string? location,
            CancellationToken cancellationToken = default);

        Task ReleaseAsync(string key, CancellationToken cancellationToken = default);
    }
}
