using Mundialito.Application.Common.Paging;
using Mundialito.Application.Standings.GetStandings;
using Mundialito.Application.Standings.GetTopScorers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mundialito.Application.Standings
{
    public interface IStandingsQueries
    {
        Task<PagedResult<StandingItem>> GetStandingsAsync(PageRequest page, CancellationToken cancellationToken);

        Task<PagedResult<TopScorerItem>> GetTopScorersAsync(Guid? teamId, PageRequest page, CancellationToken cancellationToken);
    }
}
