using Mediator;
using Mundialito.Application.Common.Paging;
using Mundialito.Domain.Common.Results;
using Mundialito.Domain.Matches;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mundialito.Application.Matches.ListMatches
{
    internal sealed class ListMatchesQueryHandler(IMatchQueries matchQueries)
        : IQueryHandler<ListMatchesQuery, Result<PagedResult<MatchListItem>>>
    {
        private static readonly Error InvalidDateRange =
            Error.Validation("Match.InvalidDateRange", "La fecha 'from' no puede ser posterior a 'to'.");

        private static readonly Error InvalidStatus =
            Error.Validation("Match.InvalidStatus", "status debe ser 'Scheduled' o 'Played'.");

        public async ValueTask<Result<PagedResult<MatchListItem>>> Handle(ListMatchesQuery query, CancellationToken cancellationToken)
        {
            var pageResult = PageRequest.Create(
                query.PageNumber, query.PageSize, query.SortBy, query.SortDirection, ListMatchesQuery.SortFields);

            if (pageResult.IsFailure)
                return pageResult.Error;

            var from = ToUtc(query.From);
            var to = ToUtc(query.To);
            if (from > to)
                return InvalidDateRange;

            MatchStatus? status = null;
            if (!string.IsNullOrWhiteSpace(query.Status))
            {
                if (!Enum.TryParse<MatchStatus>(query.Status, ignoreCase: true, out var parsed) || !Enum.IsDefined(parsed))
                    return InvalidStatus;

                status = parsed;
            }

            return await matchQueries.ListAsync(from, to, query.TeamId, status, pageResult.Value, cancellationToken);
        }

        private static DateTime? ToUtc(DateTime? value) => value switch
        {
            null => null,
            { Kind: DateTimeKind.Unspecified } => DateTime.SpecifyKind(value.Value, DateTimeKind.Utc),
            _ => value.Value.ToUniversalTime()
        };
    }
}
