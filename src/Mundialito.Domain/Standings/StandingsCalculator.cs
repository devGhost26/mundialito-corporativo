using Mundialito.Domain.Matches;
using Mundialito.Domain.Teams;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mundialito.Domain.Standings
{
    public static class StandingsCalculator
    {
        public static IReadOnlyList<TeamStanding> Calculate(IEnumerable<Team> teams, IEnumerable<Match> matches)
        {
            var table = teams.ToDictionary(t => t.Id, t => new TeamStanding(t.Id, t.Name));

            foreach (var match in matches)
            {
                if (match.Status != MatchStatus.Played)
                    continue;

                if (match.HomeGoals is not int homeGoals || match.AwayGoals is not int awayGoals)
                    continue;

                if (table.TryGetValue(match.HomeTeamId, out var home))
                    home.AddResult(homeGoals, awayGoals);

                if (table.TryGetValue(match.AwayTeamId, out var away))
                    away.AddResult(awayGoals, homeGoals);
            }

            return table.Values
                .OrderByDescending(s => s.Points)
                .ThenByDescending(s => s.GoalDifference)
                .ThenByDescending(s => s.GoalsFor)
                .ThenBy(s => s.TeamName)
                .ToList();
        }
    }
}
