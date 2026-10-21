using Mediator;
using Mundialito.Api.Contracts.Standings;
using Mundialito.Api.Extensions;
using Mundialito.Application.Standings.GetStandings;
using Mundialito.Application.Standings.GetTopScorers;

namespace Mundialito.Api.Endpoints
{
    public static class StandingEndpoints
    {
        public static IEndpointRouteBuilder MapStandingEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api").WithTags("Standings");

            group.MapGet("/standings", GetStandings);
            group.MapGet("/top-scorers", GetTopScorers);

            return app;
        }

        private static async Task<IResult> GetStandings(
            [AsParameters] StandingsRequest request, ISender sender, CancellationToken cancellationToken)
        {
            var query = new GetStandingsQuery(request.PageNumber, request.PageSize, request.SortBy, request.SortDirection);

            return (await sender.Send(query, cancellationToken)).ToOk();
        }

        private static async Task<IResult> GetTopScorers(
            [AsParameters] TopScorersRequest request, ISender sender, CancellationToken cancellationToken)
        {
            var query = new GetTopScorersQuery(
                request.PageNumber, request.PageSize, request.SortBy, request.SortDirection, request.TeamId);

            return (await sender.Send(query, cancellationToken)).ToOk();
        }
    }
}
