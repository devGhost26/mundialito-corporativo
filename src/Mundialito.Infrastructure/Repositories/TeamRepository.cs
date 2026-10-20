using Microsoft.EntityFrameworkCore;
using Mundialito.Domain.Teams;
using Mundialito.Infrastructure.Persistence;

namespace Mundialito.Infrastructure.Repositories
{
    internal sealed class TeamRepository(AppDbContext dbContext) : ITeamRepository
    {
        public Task<Team?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            dbContext.Teams
                .Include(t => t.Players)
                .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

        public Task<bool> NameExistsAsync(string name, Guid? excludeTeamId = null, CancellationToken cancellationToken = default)
        {
            var normalized = name.Trim();

            return dbContext.Teams.AnyAsync(
                t => t.Name == normalized && (excludeTeamId == null || t.Id != excludeTeamId),
                cancellationToken);
        }

        public async Task<IReadOnlyList<Player>> GetPlayersAsync(IEnumerable<Guid> playerIds, CancellationToken cancellationToken = default)
        {
            var ids = playerIds.Distinct().ToList();

            return await dbContext.Players
                .Where(p => ids.Contains(p.Id))
                .ToListAsync(cancellationToken);
        }

        public void Add(Team team) => dbContext.Teams.Add(team);

        public void Remove(Team team) => dbContext.Teams.Remove(team);
    }
}
