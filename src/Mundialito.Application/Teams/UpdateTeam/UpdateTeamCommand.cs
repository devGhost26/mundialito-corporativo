using Mediator;
using Mundialito.Application.Abstractions;
using Mundialito.Domain.Common.Results;
using Mundialito.Domain.Teams;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mundialito.Application.Teams.UpdateTeam
{
    public sealed record UpdateTeamCommand(Guid TeamId, string Name, string? City) : ICommand<Result>;
}
