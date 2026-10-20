using Microsoft.Extensions.Logging;
using Mundialito.Application.Abstractions;
using Mundialito.Domain.Teams;

namespace Mundialito.Application.Teams.EventHandlers
{
    internal sealed class TeamCreatedDomainEventHandler(ILogger<TeamCreatedDomainEventHandler> logger)
        : IDomainEventHandler<TeamCreatedDomainEvent>
    {
        public Task HandleAsync(TeamCreatedDomainEvent domainEvent, CancellationToken cancellationToken = default)
        {
            logger.LogInformation(
                "Equipo creado: {TeamId} {TeamName} (EventId {EventId}, {OccurredOnUtc})",
                domainEvent.TeamId, domainEvent.Name, domainEvent.EventId, domainEvent.OccurredOnUtc);

            return Task.CompletedTask;
        }
    }
}
