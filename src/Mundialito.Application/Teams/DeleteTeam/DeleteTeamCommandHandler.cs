using Mediator;
using Mundialito.Application.Abstractions;
using Mundialito.Domain.Common.Results;
using Mundialito.Domain.Matches;
using Mundialito.Domain.Teams;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mundialito.Application.Teams.DeleteTeam
{
    internal sealed class DeleteTeamCommandHandler(
        ITeamRepository teamRepository,
        IMatchRepository matchRepository,
        IUnitOfWork unitOfWork) : ICommandHandler<DeleteTeamCommand, Result>
    {
        public async ValueTask<Result> Handle(DeleteTeamCommand command, CancellationToken cancellationToken)
        {
            var team = await teamRepository.GetByIdAsync(command.TeamId, cancellationToken);
            if (team is null)
                return Result.Success();

            if (await matchRepository.ExistsForTeamAsync(team.Id, cancellationToken))
                return TeamErrors.HasMatches;

            teamRepository.Remove(team);

            return await unitOfWork.CommitAsync(cancellationToken);
        }
    }
}
