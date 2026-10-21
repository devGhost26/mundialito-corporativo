using Mediator;
using Mundialito.Application.Common.Paging;
using Mundialito.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mundialito.Application.Standings.GetTopScorers
{
    public sealed record GetTopScorersQuery(
        int? PageNumber,
        int? PageSize,
        string? SortBy,
        string? SortDirection,
        Guid? TeamId) : IQuery<Result<PagedResult<TopScorerItem>>>
    {
        public static readonly IReadOnlyList<string> SortFields = ["goals", "name"];
    }
}
