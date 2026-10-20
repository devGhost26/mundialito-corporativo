using Mediator;
using Mundialito.Application.Abstractions;
using Mundialito.Domain.Common.Results;
using Mundialito.Domain.Teams;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mundialito.Application.Teams.AddPlayer
{
    internal sealed class AddPlayerCommandHandler(ITeamRepository teamRepository, IUnitOfWork unitOfWork)
        : ICommandHandler<AddPlayerCommand, Result<Guid>>
    {
        public async ValueTask<Result<Guid>> Handle(AddPlayerCommand command, CancellationToken cancellationToken)
        {
            var team = await teamRepository.GetByIdAsync(command.TeamId, cancellationToken);
            if (team is null)
                return TeamErrors.NotFound(command.TeamId);

            var playerResult = team.AddPlayer(command.FirstName, command.LastName, command.Number, command.Position);
            if (playerResult.IsFailure)
                return playerResult.Error;

            var commitResult = await unitOfWork.CommitAsync(cancellationToken);
            if (commitResult.IsFailure)
                return commitResult.Error;

            return playerResult.Value.Id;
        }
    }
}
