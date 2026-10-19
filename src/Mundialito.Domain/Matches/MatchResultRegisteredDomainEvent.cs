using Mundialito.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mundialito.Domain.Matches
{
    public sealed record MatchResultRegisteredDomainEvent(
        Guid MatchId,
        Guid HomeTeamId,
        Guid AwayTeamId,
        int HomeGoals,
        int AwayGoals) : IDomainEvent
    {
        public Guid EventId { get; } = Guid.NewGuid();
        public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
    }
}
