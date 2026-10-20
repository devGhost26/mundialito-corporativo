using Mediator;
using Mundialito.Application.Abstractions;
using Mundialito.Domain.Common.Results;
using Mundialito.Domain.Teams;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mundialito.Application.Teams.UpdateTeam
{
    internal sealed class UpdateTeamCommandHandler(ITeamRepository teamRepository, IUnitOfWork unitOfWork)
        : ICommandHandler<UpdateTeamCommand, Result>
    {
        public async ValueTask<Result> Handle(UpdateTeamCommand command, CancellationToken cancellationToken)
        {
            var team = await teamRepository.GetByIdAsync(command.TeamId, cancellationToken);
            if (team is null)
                return TeamErrors.NotFound(command.TeamId);

            var updateResult = team.Update(command.Name, command.City);
            if (updateResult.IsFailure)
                return updateResult;

            if (await teamRepository.NameExistsAsync(team.Name, team.Id, cancellationToken))
                return TeamErrors.NameAlreadyExists(team.Name);

            return await unitOfWork.CommitAsync(cancellationToken);
        }
    }
}
