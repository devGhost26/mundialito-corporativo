using Mediator;
using Mundialito.Domain.Common.Results;
using Mundialito.Domain.Teams;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mundialito.Application.Teams.AddPlayer
{
    public sealed record AddPlayerCommand(
        Guid TeamId,
        string FirstName,
        string LastName,
        int Number,
        PlayerPosition Position) : ICommand<Result<Guid>>;
}
