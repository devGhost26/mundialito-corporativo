using System;
using System.Collections.Generic;
using System.Text;

namespace Mundialito.Application.Standings.GetStandings
{
    public sealed record StandingItem
    {
        public int Position { get; init; }
        public Guid TeamId { get; init; }
        public string TeamName { get; init; } = string.Empty;
        public int Played { get; init; }
        public int Won { get; init; }
        public int Drawn { get; init; }
        public int Lost { get; init; }
        public int GoalsFor { get; init; }
        public int GoalsAgainst { get; init; }
        public int GoalDifference { get; init; }
        public int Points { get; init; }
    }
}
