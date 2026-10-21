using Mediator;
using Mundialito.Api.Contracts.Matches;
using Mundialito.Api.Extensions;
using Mundialito.Application.Matches.GetMatchById;
using Mundialito.Application.Matches.ListMatches;
using Mundialito.Application.Matches.RegisterMatchResult;
using Mundialito.Application.Matches.ScheduleMatch;

namespace Mundialito.Api.Endpoints
{
    public static class MatchEndpoints
    {
        public static IEndpointRouteBuilder MapMatchEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/matches").WithTags("Matches");
            group.MapGet("/", ListMatches);
            group.MapGet("/{matchId:guid}", GetMatchById);
            group.MapPost("/", ScheduleMatch);
            group.MapPost("/{matchId:guid}/result", RegisterResult);

            return app;
        }
        private static async Task<IResult> ListMatches(
            [AsParameters] ListMatchesRequest request, ISender sender, CancellationToken cancellationToken)
        {
            var query = new ListMatchesQuery(
                request.PageNumber, request.PageSize, request.SortBy, request.SortDirection,
                request.From, request.To, request.TeamId, request.Status);

            return (await sender.Send(query, cancellationToken)).ToOk();
        }

        private static async Task<IResult> GetMatchById(Guid matchId, ISender sender, CancellationToken cancellationToken) =>
            (await sender.Send(new GetMatchByIdQuery(matchId), cancellationToken)).ToOk();
        private static async Task<IResult> ScheduleMatch(ScheduleMatchRequest request, ISender sender, CancellationToken cancellationToken)
        {
            var command = new ScheduleMatchCommand(request.HomeTeamId, request.AwayTeamId, request.ScheduledAtUtc, request.Venue);
            var result = await sender.Send(command, cancellationToken);

            return result.IsSuccess
                ? Results.Created($"/api/matches/{result.Value}", new { id = result.Value })
                : result.Error.ToProblem();
        }

        private static async Task<IResult> RegisterResult(Guid matchId, RegisterMatchResultRequest request, ISender sender, CancellationToken cancellationToken)
        {
            var goals = (request.Goals ?? [])
                .Select(g => new GoalInput(g.PlayerId, g.Minute))
                .ToList();

            var command = new RegisterMatchResultCommand(matchId, request.HomeGoals, request.AwayGoals, goals);
            var result = await sender.Send(command, cancellationToken);

            return result.IsSuccess
                ? Results.Ok(new { matchId })
                : result.Error.ToProblem();
        }
    }
}
