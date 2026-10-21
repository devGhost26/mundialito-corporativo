using Dapper;
using Mundialito.Application.Common.Paging;
using Mundialito.Application.Matches;
using Mundialito.Application.Matches.GetMatchById;
using Mundialito.Application.Matches.ListMatches;
using Mundialito.Domain.Matches;

namespace Mundialito.Infrastructure.Queries
{
    internal sealed class MatchQueries(SqlConnectionFactory connectionFactory) : IMatchQueries
    {
        private static readonly Dictionary<string, string> SortColumns = new()
        {
            ["date"] = "m.ScheduledAtUtc",
            ["status"] = "m.Status"
        };

        private const string MatchColumns = """
        m.Id,
        m.HomeTeamId,
        h.Name AS HomeTeamName,
        m.AwayTeamId,
        a.Name AS AwayTeamName,
        TODATETIMEOFFSET(m.ScheduledAtUtc, '+00:00') AS ScheduledAtUtc,
        m.Venue,
        m.Status,
        m.HomeGoals,
        m.AwayGoals
        """;

        private const string MatchJoins = """
        FROM Matches m
        INNER JOIN Teams h ON h.Id = m.HomeTeamId
        INNER JOIN Teams a ON a.Id = m.AwayTeamId
        """;

        public async Task<PagedResult<MatchListItem>> ListAsync(
            DateTime? fromUtc,
            DateTime? toUtc,
            Guid? teamId,
            MatchStatus? status,
            PageRequest page,
            CancellationToken cancellationToken)
        {
            var conditions = new List<string>();
            var parameters = new DynamicParameters();

            if (fromUtc is not null)
            {
                conditions.Add("m.ScheduledAtUtc >= @FromUtc");
                parameters.Add("FromUtc", fromUtc);
            }

            if (toUtc is not null)
            {
                conditions.Add("m.ScheduledAtUtc <= @ToUtc");
                parameters.Add("ToUtc", toUtc);
            }

            if (teamId is not null)
            {
                conditions.Add("(m.HomeTeamId = @TeamId OR m.AwayTeamId = @TeamId)");
                parameters.Add("TeamId", teamId);
            }

            if (status is not null)
            {
                conditions.Add("m.Status = @Status");
                parameters.Add("Status", (int)status.Value);
            }

            parameters.Add("Offset", page.Offset);
            parameters.Add("PageSize", page.PageSize);

            var where = conditions.Count == 0 ? string.Empty : "WHERE " + string.Join(" AND ", conditions);

            var sql = $"""
            SELECT COUNT(*) FROM Matches m {where};

            SELECT {MatchColumns}
            {MatchJoins}
            {where}
            {SqlSort.OrderBy(page, SortColumns, "m.Id")}
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
            """;

            await using var connection = await connectionFactory.OpenAsync(cancellationToken);
            await using var multi = await connection.QueryMultipleAsync(
                new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));

            var total = await multi.ReadSingleAsync<int>();
            var items = (await multi.ReadAsync<MatchListItem>()).ToList();

            return new PagedResult<MatchListItem>(items, page.PageNumber, page.PageSize, total);
        }

        public async Task<MatchDetails?> GetByIdAsync(Guid matchId, CancellationToken cancellationToken)
        {
            var sql = $"""
            SELECT {MatchColumns}
            {MatchJoins}
            WHERE m.Id = @MatchId;

            SELECT g.PlayerId, p.FirstName + ' ' + p.LastName AS PlayerName, g.TeamId, g.Minute
            FROM Goals g
            INNER JOIN Players p ON p.Id = g.PlayerId
            WHERE g.MatchId = @MatchId
            ORDER BY g.Minute;
            """;

            await using var connection = await connectionFactory.OpenAsync(cancellationToken);
            await using var multi = await connection.QueryMultipleAsync(
                new CommandDefinition(sql, new { MatchId = matchId }, cancellationToken: cancellationToken));

            var match = await multi.ReadSingleOrDefaultAsync<MatchListItem>();
            if (match is null)
                return null;

            var goals = (await multi.ReadAsync<MatchGoalItem>()).ToList();

            return new MatchDetails(match, goals);
        }
    }
}
