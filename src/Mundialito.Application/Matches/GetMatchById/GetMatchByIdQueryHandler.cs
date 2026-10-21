using Mediator;
using Mundialito.Domain.Common.Results;
using Mundialito.Domain.Matches;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mundialito.Application.Matches.GetMatchById
{
    internal sealed class GetMatchByIdQueryHandler(IMatchQueries matchQueries)
        : IQueryHandler<GetMatchByIdQuery, Result<MatchDetails>>
    {
        public async ValueTask<Result<MatchDetails>> Handle(GetMatchByIdQuery query, CancellationToken cancellationToken)
        {
            var match = await matchQueries.GetByIdAsync(query.MatchId, cancellationToken);

            return match is null ? MatchErrors.NotFound(query.MatchId) : match;
        }
    }
}
