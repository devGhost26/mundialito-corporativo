using Mediator;
using Mundialito.Application.Teams.ListTeams;
using Mundialito.Domain.Common.Results;
using Mundialito.Domain.Teams;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mundialito.Application.Teams.GetTeamById
{
    internal sealed class GetTeamByIdQueryHandler(ITeamQueries teamQueries)
        : IQueryHandler<GetTeamByIdQuery, Result<TeamListItem>>
    {
        public async ValueTask<Result<TeamListItem>> Handle(GetTeamByIdQuery query, CancellationToken cancellationToken)
        {
            var team = await teamQueries.GetByIdAsync(query.TeamId, cancellationToken);

            return team is null ? TeamErrors.NotFound(query.TeamId) : team;
        }
    }
}
