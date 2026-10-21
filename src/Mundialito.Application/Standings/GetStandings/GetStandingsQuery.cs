using Mediator;
using Mundialito.Application.Common.Paging;
using Mundialito.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mundialito.Application.Standings.GetStandings
{
    public sealed record GetStandingsQuery(
        int? PageNumber,
        int? PageSize,
        string? SortBy,
        string? SortDirection) : IQuery<Result<PagedResult<StandingItem>>>
    {
        public static readonly IReadOnlyList<string> SortFields = ["position", "points", "goalsFor", "name"];
    }
}
