using Mediator;
using Mundialito.Application.Common.Paging;
using Mundialito.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mundialito.Application.Teams.ListTeams
{
    public sealed record ListTeamsQuery(
       int? PageNumber,
       int? PageSize,
       string? SortBy,
       string? SortDirection,
       string? Name,
       string? City) : IQuery<Result<PagedResult<TeamListItem>>>
    {
        public static readonly IReadOnlyList<string> SortFields = ["name", "city", "createdAt"];
    }
}
