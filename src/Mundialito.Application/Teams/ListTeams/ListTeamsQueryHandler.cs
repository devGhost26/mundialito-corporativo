using Mediator;
using Mundialito.Application.Common.Paging;
using Mundialito.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mundialito.Application.Teams.ListTeams
{
    internal sealed class ListTeamsQueryHandler(ITeamQueries teamQueries)
        : IQueryHandler<ListTeamsQuery, Result<PagedResult<TeamListItem>>>
    {
        public async ValueTask<Result<PagedResult<TeamListItem>>> Handle(ListTeamsQuery query, CancellationToken cancellationToken)
        {
            var pageResult = PageRequest.Create(
                query.PageNumber, query.PageSize, query.SortBy, query.SortDirection, ListTeamsQuery.SortFields);

            if (pageResult.IsFailure)
                return pageResult.Error;

            return await teamQueries.ListAsync(query.Name, query.City, pageResult.Value, cancellationToken);
        }
    }
}
