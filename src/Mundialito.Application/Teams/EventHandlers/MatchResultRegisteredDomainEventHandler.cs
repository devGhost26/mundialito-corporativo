using Microsoft.Extensions.Logging;
using Mundialito.Application.Abstractions;
using Mundialito.Domain.Matches;

namespace Mundialito.Application.Teams.EventHandlers
{
    internal sealed class MatchResultRegisteredDomainEventHandler(ILogger<MatchResultRegisteredDomainEventHandler> logger)
        : IDomainEventHandler<MatchResultRegisteredDomainEvent>
    {
        public Task HandleAsync(MatchResultRegisteredDomainEvent domainEvent, CancellationToken cancellationToken = default)
        {
            logger.LogInformation(
                "Resultado registrado: partido {MatchId}, {HomeTeamId} {HomeGoals} - {AwayGoals} {AwayTeamId} (EventId {EventId}, {OccurredOnUtc})",
                domainEvent.MatchId, domainEvent.HomeTeamId, domainEvent.HomeGoals,
                domainEvent.AwayGoals, domainEvent.AwayTeamId, domainEvent.EventId, domainEvent.OccurredOnUtc);

            return Task.CompletedTask;
        }
    }
}
