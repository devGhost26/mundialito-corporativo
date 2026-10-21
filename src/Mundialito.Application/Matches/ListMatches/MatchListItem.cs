using Mundialito.Domain.Matches;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mundialito.Application.Matches.ListMatches
{
    public sealed record MatchListItem
    {
        public Guid Id { get; init; }
        public Guid HomeTeamId { get; init; }
        public string HomeTeamName { get; init; } = string.Empty;
        public Guid AwayTeamId { get; init; }
        public string AwayTeamName { get; init; } = string.Empty;
        public DateTimeOffset ScheduledAtUtc { get; init; }
        public string? Venue { get; init; }
        public MatchStatus Status { get; init; }
        public int? HomeGoals { get; init; }
        public int? AwayGoals { get; init; }
    }
}
