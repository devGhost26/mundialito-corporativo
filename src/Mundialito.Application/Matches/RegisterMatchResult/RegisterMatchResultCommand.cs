using Mediator;
using Mundialito.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mundialito.Application.Matches.RegisterMatchResult
{
    public sealed record RegisterMatchResultCommand(
        Guid MatchId,
        int HomeGoals,
        int AwayGoals,
        IReadOnlyList<GoalInput> Goals) : ICommand<Result>;
}
