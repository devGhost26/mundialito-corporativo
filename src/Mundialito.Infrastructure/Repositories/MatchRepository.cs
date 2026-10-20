using Microsoft.EntityFrameworkCore;
using Mundialito.Domain.Matches;
using Mundialito.Infrastructure.Persistence;

namespace Mundialito.Infrastructure.Repositories
{
    internal sealed class MatchRepository(AppDbContext dbContext) : IMatchRepository
    {
        public Task<Match?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            dbContext.Matches
                .Include(m => m.Goals)
                .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

        public Task<bool> ExistsForTeamAsync(Guid teamId, CancellationToken cancellationToken = default) =>
            dbContext.Matches.AnyAsync(m => m.HomeTeamId == teamId || m.AwayTeamId == teamId, cancellationToken);

        public void Add(Match match) => dbContext.Matches.Add(match);

        public void Remove(Match match) => dbContext.Matches.Remove(match);
    }
}
