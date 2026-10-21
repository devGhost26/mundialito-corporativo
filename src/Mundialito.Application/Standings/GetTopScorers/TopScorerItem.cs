using System;
using System.Collections.Generic;
using System.Text;

namespace Mundialito.Application.Standings.GetTopScorers
{
    public sealed record TopScorerItem
    {
        public Guid PlayerId { get; init; }
        public string PlayerName { get; init; } = string.Empty;
        public Guid TeamId { get; init; }
        public string TeamName { get; init; } = string.Empty;
        public int Goals { get; init; }
    }
}
