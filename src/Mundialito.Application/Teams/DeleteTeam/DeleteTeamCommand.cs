using Mediator;
using Mundialito.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mundialito.Application.Teams.DeleteTeam
{
    public sealed record DeleteTeamCommand(Guid TeamId) : ICommand<Result>;
}
