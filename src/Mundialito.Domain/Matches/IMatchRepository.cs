using System;
using System.Collections.Generic;
using System.Text;

namespace Mundialito.Domain.Matches
{
    public interface IMatchRepository
    {
        Task<Match?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<bool> ExistsForTeamAsync(Guid teamId, CancellationToken cancellationToken = default);

        void Add(Match match);

        void Remove(Match match);
    }
}
