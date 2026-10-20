using Mediator;
using Mundialito.Application.Teams.ListTeams;
using Mundialito.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mundialito.Application.Teams.GetTeamById
{
    public sealed record GetTeamByIdQuery(Guid TeamId) : IQuery<Result<TeamListItem>>;
}
