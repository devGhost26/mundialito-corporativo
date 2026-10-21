using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Mundialito.Application.Abstractions;
using Mundialito.Domain.Common;
using Mundialito.Domain.Common.Results;
using Mundialito.Infrastructure.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Mundialito.Infrastructure.Persistence
{
    internal sealed class UnitOfWork(
        AppDbContext dbContext,
        DomainEventDispatcher dispatcher,
        ILogger<UnitOfWork> logger) : IUnitOfWork
    {
        public async Task<Result> CommitAsync(CancellationToken cancellationToken = default)
        {
            var entities = dbContext.ChangeTracker
                .Entries<Entity>()
                .Select(entry => entry.Entity)
                .Where(entity => entity.DomainEvents.Count != 0)
                .ToList();

            var domainEvents = entities.SelectMany(entity => entity.DomainEvents).ToList();
            dbContext.DomainEvents.AddRange(
                domainEvents.Select(domainEvent => DomainEventRecord.From(domainEvent, Activity.Current)));

            await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var changes = await dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                logger.LogDebug("Transacción confirmada con {Changes} cambios", changes);
            }
            catch (DbUpdateException ex) when (IsUniqueViolation(ex))
            {
                await transaction.RollbackAsync(cancellationToken);

                logger.LogWarning("Transacción revertida: el registro ya existe");

                return Error.Conflict("Database.Duplicate", "El registro ya existe.");
            }

            entities.ForEach(entity => entity.ClearDomainEvents());

            await dispatcher.DispatchAsync(domainEvents, cancellationToken);

            return Result.Success();
        }

        private static bool IsUniqueViolation(DbUpdateException ex) =>
            ex.InnerException is SqlException { Number: 2601 or 2627 };
    }
}
