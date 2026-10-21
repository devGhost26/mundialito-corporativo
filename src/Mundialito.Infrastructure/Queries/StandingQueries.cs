using Dapper;
using Mundialito.Application.Common.Paging;
using Mundialito.Application.Standings;
using Mundialito.Application.Standings.GetStandings;
using Mundialito.Application.Standings.GetTopScorers;
using Mundialito.Domain.Matches;
using Mundialito.Domain.Standings;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mundialito.Infrastructure.Queries
{
    internal sealed class StandingsQueries(SqlConnectionFactory connectionFactory) : IStandingsQueries
    {
        private static readonly Dictionary<string, string> StandingSortColumns = new()
        {
            ["position"] = "s.Position",
            ["points"] = "s.Points",
            ["goalsFor"] = "s.GoalsFor",
            ["name"] = "s.TeamName"
        };

        private static readonly Dictionary<string, string> ScorerSortColumns = new()
        {
            ["goals"] = "Goals",
            ["name"] = "PlayerName"
        };

        public async Task<PagedResult<StandingItem>> GetStandingsAsync(PageRequest page, CancellationToken cancellationToken)
        {
            var sql = $"""
            SELECT COUNT(*) FROM Teams;

            WITH Results AS (
                SELECT m.HomeTeamId AS TeamId, m.HomeGoals AS GoalsFor, m.AwayGoals AS GoalsAgainst
                FROM Matches m
                WHERE m.Status = @Played
                UNION ALL
                SELECT m.AwayTeamId, m.AwayGoals, m.HomeGoals
                FROM Matches m
                WHERE m.Status = @Played
            ),
            Totals AS (
                SELECT
                    t.Id AS TeamId,
                    t.Name AS TeamName,
                    COUNT(r.TeamId) AS Played,
                    COALESCE(SUM(CASE WHEN r.GoalsFor > r.GoalsAgainst THEN 1 END), 0) AS Won,
                    COALESCE(SUM(CASE WHEN r.GoalsFor = r.GoalsAgainst THEN 1 END), 0) AS Drawn,
                    COALESCE(SUM(CASE WHEN r.GoalsFor < r.GoalsAgainst THEN 1 END), 0) AS Lost,
                    COALESCE(SUM(r.GoalsFor), 0) AS GoalsFor,
                    COALESCE(SUM(r.GoalsAgainst), 0) AS GoalsAgainst
                FROM Teams t
                LEFT JOIN Results r ON r.TeamId = t.Id
                GROUP BY t.Id, t.Name
            ),
            Scored AS (
                SELECT *,
                    GoalsFor - GoalsAgainst AS GoalDifference,
                    Won * @PointsPerWin + Drawn * @PointsPerDraw + Lost * @PointsPerLoss AS Points
                FROM Totals
            ),
            Ranked AS (
                SELECT *,
                    CAST(ROW_NUMBER() OVER (
                        ORDER BY Points DESC, GoalDifference DESC, GoalsFor DESC, TeamName ASC) AS int) AS Position
                FROM Scored
            )
            SELECT s.Position, s.TeamId, s.TeamName, s.Played, s.Won, s.Drawn, s.Lost,
                   s.GoalsFor, s.GoalsAgainst, s.GoalDifference, s.Points
            FROM Ranked s
            {SqlSort.OrderBy(page, StandingSortColumns, "s.Position")}
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
            """;

            var parameters = new
            {
                Played = (int)MatchStatus.Played,
                TournamentRules.PointsPerWin,
                TournamentRules.PointsPerDraw,
                TournamentRules.PointsPerLoss,
                page.Offset,
                page.PageSize
            };

            await using var connection = await connectionFactory.OpenAsync(cancellationToken);
            await using var multi = await connection.QueryMultipleAsync(
                new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));

            var total = await multi.ReadSingleAsync<int>();
            var items = (await multi.ReadAsync<StandingItem>()).ToList();

            return new PagedResult<StandingItem>(items, page.PageNumber, page.PageSize, total);
        }

        public async Task<PagedResult<TopScorerItem>> GetTopScorersAsync(
            Guid? teamId, PageRequest page, CancellationToken cancellationToken)
        {
            var where = teamId is null ? string.Empty : "WHERE g.TeamId = @TeamId";

            var sql = $"""
            SELECT COUNT(DISTINCT g.PlayerId) FROM Goals g {where};

            SELECT
                g.PlayerId,
                p.FirstName + ' ' + p.LastName AS PlayerName,
                g.TeamId,
                t.Name AS TeamName,
                COUNT(*) AS Goals
            FROM Goals g
            INNER JOIN Players p ON p.Id = g.PlayerId
            INNER JOIN Teams t ON t.Id = g.TeamId
            {where}
            GROUP BY g.PlayerId, p.FirstName, p.LastName, g.TeamId, t.Name
            {SqlSort.OrderBy(page, ScorerSortColumns, "PlayerName", "g.PlayerId")}
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
            """;

            var parameters = new { TeamId = teamId, page.Offset, page.PageSize };

            await using var connection = await connectionFactory.OpenAsync(cancellationToken);
            await using var multi = await connection.QueryMultipleAsync(
                new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));

            var total = await multi.ReadSingleAsync<int>();
            var items = (await multi.ReadAsync<TopScorerItem>()).ToList();

            return new PagedResult<TopScorerItem>(items, page.PageNumber, page.PageSize, total);
        }
    }
}
