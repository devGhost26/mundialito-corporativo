using Dapper;
using Mundialito.Application.Common.Paging;
using Mundialito.Application.Teams;
using Mundialito.Application.Teams.ListPlayers;
using Mundialito.Application.Teams.ListTeams;
using Mundialito.Domain.Teams;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mundialito.Infrastructure.Queries
{
    internal sealed class TeamQueries(SqlConnectionFactory connectionFactory) : ITeamQueries
    {
        private static readonly Dictionary<string, string> TeamSortColumns = new()
        {
            ["name"] = "t.Name",
            ["city"] = "t.City",
            ["createdAt"] = "t.CreatedAtUtc"
        };

        private static readonly Dictionary<string, string> PlayerSortColumns = new()
        {
            ["number"] = "p.Number",
            ["name"] = "p.LastName",
            ["position"] = "p.Position"
        };

        private const string TeamColumns = """
        t.Id,
        t.Name,
        t.City,
        (SELECT COUNT(*) FROM Players p WHERE p.TeamId = t.Id) AS PlayerCount,
        TODATETIMEOFFSET(t.CreatedAtUtc, '+00:00') AS CreatedAtUtc
        """;

        public async Task<PagedResult<TeamListItem>> ListAsync(
            string? name, string? city, PageRequest page, CancellationToken cancellationToken)
        {
            var conditions = new List<string>();
            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(name))
            {
                conditions.Add("t.Name LIKE @Name");
                parameters.Add("Name", SqlLike.Contains(name));
            }

            if (!string.IsNullOrWhiteSpace(city))
            {
                conditions.Add("t.City LIKE @City");
                parameters.Add("City", SqlLike.Contains(city));
            }

            parameters.Add("Offset", page.Offset);
            parameters.Add("PageSize", page.PageSize);

            var where = conditions.Count == 0 ? string.Empty : "WHERE " + string.Join(" AND ", conditions);

            var sql = $"""
            SELECT COUNT(*) FROM Teams t {where};

            SELECT {TeamColumns}
            FROM Teams t
            {where}
            {SqlSort.OrderBy(page, TeamSortColumns, "t.Id")}
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
            """;

            await using var connection = await connectionFactory.OpenAsync(cancellationToken);
            await using var multi = await connection.QueryMultipleAsync(
                new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));

            var total = await multi.ReadSingleAsync<int>();
            var items = (await multi.ReadAsync<TeamListItem>()).ToList();

            return new PagedResult<TeamListItem>(items, page.PageNumber, page.PageSize, total);
        }

        public async Task<TeamListItem?> GetByIdAsync(Guid teamId, CancellationToken cancellationToken)
        {
            var sql = $"SELECT {TeamColumns} FROM Teams t WHERE t.Id = @TeamId;";

            await using var connection = await connectionFactory.OpenAsync(cancellationToken);

            return await connection.QuerySingleOrDefaultAsync<TeamListItem>(
                new CommandDefinition(sql, new { TeamId = teamId }, cancellationToken: cancellationToken));
        }

        public async Task<PagedResult<PlayerListItem>?> ListPlayersAsync(
            Guid teamId, string? name, PlayerPosition? position, PageRequest page, CancellationToken cancellationToken)
        {
            var conditions = new List<string> { "p.TeamId = @TeamId" };
            var parameters = new DynamicParameters();
            parameters.Add("TeamId", teamId);

            if (!string.IsNullOrWhiteSpace(name))
            {
                conditions.Add("(p.FirstName LIKE @Name OR p.LastName LIKE @Name)");
                parameters.Add("Name", SqlLike.Contains(name));
            }

            if (position is not null)
            {
                conditions.Add("p.Position = @Position");
                parameters.Add("Position", (int)position.Value);
            }

            parameters.Add("Offset", page.Offset);
            parameters.Add("PageSize", page.PageSize);

            var where = "WHERE " + string.Join(" AND ", conditions);

            var sql = $"""
            SELECT COUNT(*) FROM Teams WHERE Id = @TeamId;

            SELECT COUNT(*) FROM Players p {where};

            SELECT p.Id, p.TeamId, p.FirstName, p.LastName, p.Number, p.Position
            FROM Players p
            {where}
            {SqlSort.OrderBy(page, PlayerSortColumns, "p.Id")}
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
            """;

            await using var connection = await connectionFactory.OpenAsync(cancellationToken);
            await using var multi = await connection.QueryMultipleAsync(
                new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));

            if (await multi.ReadSingleAsync<int>() == 0)
                return null;

            var total = await multi.ReadSingleAsync<int>();
            var items = (await multi.ReadAsync<PlayerListItem>()).ToList();

            return new PagedResult<PlayerListItem>(items, page.PageNumber, page.PageSize, total);
        }
    }
}
