using Mediator;
using Mundialito.Application.Common.Paging;
using Mundialito.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mundialito.Application.Matches.ListMatches
{
    public sealed record ListMatchesQuery(
        int? PageNumber,
        int? PageSize,
        string? SortBy,
        string? SortDirection,
        DateTime? From,
        DateTime? To,
        Guid? TeamId,
        string? Status) : IQuery<Result<PagedResult<MatchListItem>>>
    {
        public static readonly IReadOnlyList<string> SortFields = ["date", "status"];
    }
}
