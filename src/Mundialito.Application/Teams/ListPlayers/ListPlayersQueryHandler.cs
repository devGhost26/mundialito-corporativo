using Mediator;
using Mundialito.Application.Common.Paging;
using Mundialito.Domain.Common.Results;
using Mundialito.Domain.Teams;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mundialito.Application.Teams.ListPlayers
{
    internal sealed class ListPlayersQueryHandler(ITeamQueries teamQueries)
        : IQueryHandler<ListPlayersQuery, Result<PagedResult<PlayerListItem>>>
    {
        public async ValueTask<Result<PagedResult<PlayerListItem>>> Handle(ListPlayersQuery query, CancellationToken cancellationToken)
        {
            var pageResult = PageRequest.Create(
                query.PageNumber, query.PageSize, query.SortBy, query.SortDirection, ListPlayersQuery.SortFields);

            if (pageResult.IsFailure)
                return pageResult.Error;

            PlayerPosition? position = null;
            if (!string.IsNullOrWhiteSpace(query.Position))
            {
                if (!Enum.TryParse<PlayerPosition>(query.Position, ignoreCase: true, out var parsed) || !Enum.IsDefined(parsed))
                    return PlayerErrors.InvalidPosition;

                position = parsed;
            }

            var players = await teamQueries.ListPlayersAsync(
                query.TeamId, query.Name, position, pageResult.Value, cancellationToken);

            return players is null ? TeamErrors.NotFound(query.TeamId) : players;
        }
    }
}
