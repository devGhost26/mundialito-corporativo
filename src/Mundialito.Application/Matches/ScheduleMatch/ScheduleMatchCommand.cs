using Mediator;
using Mundialito.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mundialito.Application.Matches.ScheduleMatch
{
    public sealed record ScheduleMatchCommand(
        Guid HomeTeamId,
        Guid AwayTeamId,
        DateTime ScheduledAtUtc,
        string? Venue) : ICommand<Result<Guid>>;
}
