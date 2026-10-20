using Mundialito.Application.Common.Paging;
using Mundialito.Application.Teams.ListPlayers;
using Mundialito.Application.Teams.ListTeams;
using Mundialito.Domain.Teams;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mundialito.Application.Teams
{
    public interface ITeamQueries
    {
        Task<PagedResult<TeamListItem>> ListAsync(
            string? name, string? city, PageRequest page, CancellationToken cancellationToken);

        Task<TeamListItem?> GetByIdAsync(Guid teamId, CancellationToken cancellationToken);

        Task<PagedResult<PlayerListItem>?> ListPlayersAsync(
            Guid teamId, string? name, PlayerPosition? position, PageRequest page, CancellationToken cancellationToken);
    }
}
