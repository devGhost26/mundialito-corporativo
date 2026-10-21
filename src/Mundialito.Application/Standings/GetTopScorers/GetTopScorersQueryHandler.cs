using Mediator;
using Mundialito.Application.Common.Paging;
using Mundialito.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mundialito.Application.Standings.GetTopScorers
{
    internal sealed class GetTopScorersQueryHandler(IStandingsQueries standingsQueries)
        : IQueryHandler<GetTopScorersQuery, Result<PagedResult<TopScorerItem>>>
    {
        public async ValueTask<Result<PagedResult<TopScorerItem>>> Handle(GetTopScorersQuery query, CancellationToken cancellationToken)
        {
            var pageResult = PageRequest.Create(
                query.PageNumber, query.PageSize, query.SortBy, query.SortDirection,
                GetTopScorersQuery.SortFields, SortDirection.Descending);

            if (pageResult.IsFailure)
                return pageResult.Error;

            return await standingsQueries.GetTopScorersAsync(query.TeamId, pageResult.Value, cancellationToken);
        }
    }
}
