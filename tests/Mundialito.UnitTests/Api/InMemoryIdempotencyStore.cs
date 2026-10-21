using Mundialito.Application.Abstractions;

namespace Mundialito.UnitTests.Api
{
    internal sealed class InMemoryIdempotencyStore : IIdempotencyStore
    {
        private readonly Dictionary<string, IdempotentResponse> _records = [];

        public Task<IdempotentResponse?> GetAsync(string key, CancellationToken cancellationToken = default) =>
            Task.FromResult(_records.GetValueOrDefault(key));

        public Task<bool> TryReserveAsync(string key, string requestHash, CancellationToken cancellationToken = default) =>
            Task.FromResult(_records.TryAdd(key, new IdempotentResponse(requestHash, false, null, null, null, null)));

        public Task CompleteAsync(
            string key, int statusCode, string? contentType, string? body, string? location,
            CancellationToken cancellationToken = default)
        {
            _records[key] = _records[key] with
            {
                IsCompleted = true,
                StatusCode = statusCode,
                ContentType = contentType,
                Body = body,
                Location = location
            };

            return Task.CompletedTask;
        }

        public Task ReleaseAsync(string key, CancellationToken cancellationToken = default)
        {
            _records.Remove(key);
            return Task.CompletedTask;
        }
    }
}
