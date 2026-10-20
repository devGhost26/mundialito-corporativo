using Mediator;
using Mundialito.Application.Abstractions;
using Mundialito.Domain.Common.Results;
using Mundialito.Domain.Teams;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mundialito.Application.Teams.CreateTeam
{
    internal sealed class CreateTeamCommandHandler(ITeamRepository teamRepository, IUnitOfWork unitOfWork)
        : ICommandHandler<CreateTeamCommand, Result<Guid>>
    {
        public async ValueTask<Result<Guid>> Handle(CreateTeamCommand command, CancellationToken cancellationToken)
        {
            var teamResult = Team.Create(command.Name, command.City);
            if (teamResult.IsFailure)
                return teamResult.Error;

            var team = teamResult.Value;

            if (await teamRepository.NameExistsAsync(team.Name, cancellationToken: cancellationToken))
                return TeamErrors.NameAlreadyExists(team.Name);

            teamRepository.Add(team);

            var commitResult = await unitOfWork.CommitAsync(cancellationToken);
            if (commitResult.IsFailure)
                return commitResult.Error;

            return team.Id;
        }
    }
}
