using System;
using System.Collections.Generic;
using System.Text;

namespace Mundialito.Application.Matches.GetMatchById
{
    public sealed record MatchGoalItem
    {
        public Guid PlayerId { get; init; }
        public string PlayerName { get; init; } = string.Empty;
        public Guid TeamId { get; init; }
        public int Minute { get; init; }
    }
}
