using System;
using System.Collections.Generic;
using System.Text;

namespace Mundialito.Domain.Teams
{
    public interface ITeamRepository
    {
        Task<Team?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<bool> NameExistsAsync(string name, Guid? excludeTeamId = null, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Player>> GetPlayersAsync(IEnumerable<Guid> playerIds, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
        void Add(Team team);

        void Remove(Team team);
    }
}
