using Mediator;
using Mundialito.Application.Abstractions;
using Mundialito.Domain.Common.Results;
using Mundialito.Domain.Matches;
using Mundialito.Domain.Teams;

namespace Mundialito.Application.Matches.ScheduleMatch
{
    internal sealed class ScheduleMatchCommandHandler(
    ITeamRepository teamRepository,
    IMatchRepository matchRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<ScheduleMatchCommand, Result<Guid>>
    {
        public async ValueTask<Result<Guid>> Handle(ScheduleMatchCommand command, CancellationToken cancellationToken)
        {
            var matchResult = Match.Schedule(
                command.HomeTeamId,
                command.AwayTeamId,
                command.ScheduledAtUtc.ToUniversalTime(),
                command.Venue);

            if (matchResult.IsFailure)
                return matchResult.Error;

            if (!await teamRepository.ExistsAsync(command.HomeTeamId, cancellationToken))
                return TeamErrors.NotFound(command.HomeTeamId);

            if (!await teamRepository.ExistsAsync(command.AwayTeamId, cancellationToken))
                return TeamErrors.NotFound(command.AwayTeamId);

            matchRepository.Add(matchResult.Value);

            var commitResult = await unitOfWork.CommitAsync(cancellationToken);
            if (commitResult.IsFailure)
                return commitResult.Error;

            return matchResult.Value.Id;
        }
    }
}
