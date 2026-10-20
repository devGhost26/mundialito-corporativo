using Mediator;
using Mundialito.Application.Abstractions;
using Mundialito.Domain.Common.Results;
using Mundialito.Domain.Matches;
using Mundialito.Domain.Teams;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mundialito.Application.Matches.RegisterMatchResult
{
    internal sealed class RegisterMatchResultCommandHandler(
        IMatchRepository matchRepository,
        ITeamRepository teamRepository,
        IUnitOfWork unitOfWork) : ICommandHandler<RegisterMatchResultCommand, Result>
    {
        public async ValueTask<Result> Handle(RegisterMatchResultCommand command, CancellationToken cancellationToken)
        {
            var match = await matchRepository.GetByIdAsync(command.MatchId, cancellationToken);
            if (match is null)
                return MatchErrors.NotFound(command.MatchId);

            var players = await teamRepository.GetPlayersAsync(
                command.Goals.Select(g => g.PlayerId), cancellationToken);

            var teamByPlayer = players.ToDictionary(p => p.Id, p => p.TeamId);

            var scoredGoals = new List<ScoredGoal>();
            foreach (var goal in command.Goals)
            {
                if (!teamByPlayer.TryGetValue(goal.PlayerId, out var teamId))
                    return PlayerErrors.NotFound(goal.PlayerId);

                scoredGoals.Add(new ScoredGoal(goal.PlayerId, teamId, goal.Minute));
            }

            var registerResult = match.RegisterResult(command.HomeGoals, command.AwayGoals, scoredGoals);
            if (registerResult.IsFailure)
                return registerResult;

            return await unitOfWork.CommitAsync(cancellationToken);
        }
    }
}
