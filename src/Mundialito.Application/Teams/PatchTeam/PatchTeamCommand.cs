using Mediator;
using Mundialito.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mundialito.Application.Teams.PatchTeam
{
    public sealed record PatchTeamCommand(Guid TeamId, string? Name, string? City) : ICommand<Result>;
}
