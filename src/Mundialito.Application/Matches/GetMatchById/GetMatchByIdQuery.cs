using Mediator;
using Mundialito.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mundialito.Application.Matches.GetMatchById
{
    public sealed record GetMatchByIdQuery(Guid MatchId) : IQuery<Result<MatchDetails>>;
}
