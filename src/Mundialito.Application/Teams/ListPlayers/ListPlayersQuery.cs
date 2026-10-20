using Mediator;
using Mundialito.Application.Common.Paging;
using Mundialito.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mundialito.Application.Teams.ListPlayers
{
    public sealed record ListPlayersQuery(
        Guid TeamId,
        int? PageNumber,
        int? PageSize,
        string? SortBy,
        string? SortDirection,
        string? Name,
        string? Position) : IQuery<Result<PagedResult<PlayerListItem>>>
    {
        public static readonly IReadOnlyList<string> SortFields = ["number", "name", "position"];
    }
}
