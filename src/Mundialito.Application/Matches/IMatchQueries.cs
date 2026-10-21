using Mundialito.Application.Common.Paging;
using Mundialito.Application.Matches.GetMatchById;
using Mundialito.Application.Matches.ListMatches;
using Mundialito.Domain.Matches;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mundialito.Application.Matches
{
    public interface IMatchQueries
    {
        Task<PagedResult<MatchListItem>> ListAsync(
            DateTime? fromUtc,
            DateTime? toUtc,
            Guid? teamId,
            MatchStatus? status,
            PageRequest page,
            CancellationToken cancellationToken);

        Task<MatchDetails?> GetByIdAsync(Guid matchId, CancellationToken cancellationToken);
    }
}
