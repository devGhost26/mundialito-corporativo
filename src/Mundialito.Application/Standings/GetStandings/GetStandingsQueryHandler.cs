using Mediator;
using Mundialito.Application.Common.Paging;
using Mundialito.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mundialito.Application.Standings.GetStandings
{
    internal sealed class GetStandingsQueryHandler(IStandingsQueries standingsQueries)
        : IQueryHandler<GetStandingsQuery, Result<PagedResult<StandingItem>>>
    {
        public async ValueTask<Result<PagedResult<StandingItem>>> Handle(GetStandingsQuery query, CancellationToken cancellationToken)
        {
            var pageResult = PageRequest.Create(
                query.PageNumber, query.PageSize, query.SortBy, query.SortDirection, GetStandingsQuery.SortFields);

            if (pageResult.IsFailure)
                return pageResult.Error;

            return await standingsQueries.GetStandingsAsync(pageResult.Value, cancellationToken);
        }
    }
}
