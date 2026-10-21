using Mundialito.Application.Matches.ListMatches;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mundialito.Application.Matches.GetMatchById
{
    public sealed record MatchDetails(MatchListItem Match, IReadOnlyList<MatchGoalItem> Goals);
}
