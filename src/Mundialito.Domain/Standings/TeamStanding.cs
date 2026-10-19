using System;
using System.Collections.Generic;
using System.Text;

namespace Mundialito.Domain.Standings
{
    public sealed class TeamStanding
    {
        public TeamStanding(Guid teamId, string teamName)
        {
            TeamId = teamId;
            TeamName = teamName;
        }

        public Guid TeamId { get; }
        public string TeamName { get; }

        public int Played { get; private set; }
        public int Won { get; private set; }
        public int Drawn { get; private set; }
        public int Lost { get; private set; }
        public int GoalsFor { get; private set; }
        public int GoalsAgainst { get; private set; }

        public int GoalDifference => GoalsFor - GoalsAgainst;

        public int Points =>
            Won * TournamentRules.PointsPerWin +
            Drawn * TournamentRules.PointsPerDraw +
            Lost * TournamentRules.PointsPerLoss;

        public void AddResult(int goalsFor, int goalsAgainst)
        {
            Played++;
            GoalsFor += goalsFor;
            GoalsAgainst += goalsAgainst;

            if (goalsFor > goalsAgainst)
                Won++;
            else if (goalsFor == goalsAgainst)
                Drawn++;
            else
                Lost++;
        }
    }
}
