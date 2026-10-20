using Mediator;
using Mundialito.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mundialito.Application.Teams.CreateTeam
{
    public sealed record CreateTeamCommand(string Name, string? City) : ICommand<Result<Guid>>;
}
