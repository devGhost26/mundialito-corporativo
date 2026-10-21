using Dapper;                                       
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Mundialito.Application.Abstractions;
using Mundialito.Infrastructure.Persistence;
using Mundialito.Infrastructure.Queries;           

namespace Mundialito.Infrastructure.Idempotency
{
    internal sealed class IdempotencyStore(IServiceScopeFactory scopeFactory, SqlConnectionFactory connectionFactory)
        : IIdempotencyStore
    {
        private const string GetSql = """
            SELECT
                RequestHash,
                CAST(CASE WHEN CompletedAtUtc IS NULL THEN 0 ELSE 1 END AS bit) AS IsCompleted,
                StatusCode,
                ContentType,
                ResponseBody AS Body,
                Location
            FROM IdempotencyRecords
            WHERE [Key] = @Key;
            """;

        public async Task<IdempotentResponse?> GetAsync(string key, CancellationToken cancellationToken = default)
        {
            await using var connection = await connectionFactory.OpenAsync(cancellationToken);

            return await connection.QuerySingleOrDefaultAsync<IdempotentResponse>(
                new CommandDefinition(GetSql, new { Key = key }, cancellationToken: cancellationToken));
        }

        public async Task<bool> TryReserveAsync(string key, string requestHash, CancellationToken cancellationToken = default)
        {
            using var scope = scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            dbContext.IdempotencyRecords.Add(new IdempotencyRecord
            {
                Key = key,
                RequestHash = requestHash,
                CreatedAtUtc = DateTime.UtcNow
            });

            var result = await unitOfWork.CommitAsync(cancellationToken);

            return result.IsSuccess;
        }

        public async Task CompleteAsync(
            string key,
            int statusCode,
            string? contentType,
            string? body,
            string? location,
            CancellationToken cancellationToken = default)
        {
            using var scope = scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var record = await dbContext.IdempotencyRecords.FirstOrDefaultAsync(r => r.Key == key, cancellationToken);
            if (record is null)
                return;

            record.StatusCode = statusCode;
            record.ContentType = contentType;
            record.ResponseBody = body;
            record.Location = location;
            record.CompletedAtUtc = DateTime.UtcNow;

            await unitOfWork.CommitAsync(cancellationToken);
        }

        public async Task ReleaseAsync(string key, CancellationToken cancellationToken = default)
        {
            using var scope = scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var record = await dbContext.IdempotencyRecords.FirstOrDefaultAsync(r => r.Key == key, cancellationToken);
            if (record is null)
                return;

            dbContext.IdempotencyRecords.Remove(record);

            await unitOfWork.CommitAsync(cancellationToken);
        }
    }
}