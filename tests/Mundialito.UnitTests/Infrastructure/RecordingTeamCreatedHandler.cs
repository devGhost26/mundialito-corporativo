using Mundialito.Application.Abstractions;
using Mundialito.Domain.Teams;

namespace Mundialito.UnitTests.Infrastructure
{
    internal sealed class RecordingTeamCreatedHandler : IDomainEventHandler<TeamCreatedDomainEvent>
    {
        public List<TeamCreatedDomainEvent> Received { get; } = [];

        public Task HandleAsync(TeamCreatedDomainEvent domainEvent, CancellationToken cancellationToken = default)
        {
            Received.Add(domainEvent);
            return Task.CompletedTask;
        }
    }
}
