using Mediator;
using Mundialito.Api.Contracts.Teams;
using Mundialito.Api.Extensions;
using Mundialito.Api.Idempotency;
using Mundialito.Application.Teams.AddPlayer;
using Mundialito.Application.Teams.CreateTeam;
using Mundialito.Application.Teams.DeleteTeam;
using Mundialito.Application.Teams.PatchTeam;
using Mundialito.Application.Teams.UpdateTeam;
using Mundialito.Application.Teams.GetTeamById;
using Mundialito.Application.Teams.ListPlayers;
using Mundialito.Application.Teams.ListTeams;
namespace Mundialito.Api.Endpoints
{
    public static class TeamEndpoints
    {
        public static IEndpointRouteBuilder MapTeamEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/teams").WithTags("Teams");
            group.MapGet("/", ListTeams);
            group.MapGet("/{teamId:guid}", GetTeamById);
            group.MapGet("/{teamId:guid}/players", ListPlayers);
            group.MapPost("/", CreateTeam).WithIdempotency();
            group.MapPut("/{teamId:guid}", UpdateTeam);
            group.MapPatch("/{teamId:guid}", PatchTeam);
            group.MapDelete("/{teamId:guid}", DeleteTeam);
            group.MapPost("/{teamId:guid}/players", AddPlayer).WithIdempotency();

            return app;
        }

        private static async Task<IResult> ListTeams(
            [AsParameters] ListTeamsRequest request, ISender sender, CancellationToken cancellationToken)
        {
            var query = new ListTeamsQuery(
                request.PageNumber, request.PageSize, request.SortBy, request.SortDirection, request.Name, request.City);

            return (await sender.Send(query, cancellationToken)).ToOk();
        }

        private static async Task<IResult> GetTeamById(Guid teamId, ISender sender, CancellationToken cancellationToken) =>
            (await sender.Send(new GetTeamByIdQuery(teamId), cancellationToken)).ToOk();

        private static async Task<IResult> ListPlayers(
            Guid teamId, [AsParameters] ListPlayersRequest request, ISender sender, CancellationToken cancellationToken)
        {
            var query = new ListPlayersQuery(
                teamId, request.PageNumber, request.PageSize, request.SortBy, request.SortDirection,
                request.Name, request.Position);

            return (await sender.Send(query, cancellationToken)).ToOk();
        }
        private static async Task<IResult> CreateTeam(CreateTeamRequest request, ISender sender, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new CreateTeamCommand(request.Name, request.City), cancellationToken);

            return result.IsSuccess
                ? Results.Created($"/api/teams/{result.Value}", new { id = result.Value })
                : result.Error.ToProblem();
        }

        private static async Task<IResult> UpdateTeam(Guid teamId, UpdateTeamRequest request, ISender sender, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new UpdateTeamCommand(teamId, request.Name, request.City), cancellationToken);

            return result.IsSuccess ? Results.NoContent() : result.Error.ToProblem();
        }

        private static async Task<IResult> PatchTeam(Guid teamId, PatchTeamRequest request, ISender sender, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new PatchTeamCommand(teamId, request.Name, request.City), cancellationToken);

            return result.IsSuccess ? Results.NoContent() : result.Error.ToProblem();
        }

        private static async Task<IResult> DeleteTeam(Guid teamId, ISender sender, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new DeleteTeamCommand(teamId), cancellationToken);

            return result.IsSuccess ? Results.NoContent() : result.Error.ToProblem();
        }

        private static async Task<IResult> AddPlayer(Guid teamId, AddPlayerRequest request, ISender sender, CancellationToken cancellationToken)
        {
            var command = new AddPlayerCommand(teamId, request.FirstName, request.LastName, request.Number, request.Position);
            var result = await sender.Send(command, cancellationToken);

            return result.IsSuccess
                ? Results.Created($"/api/teams/{teamId}/players/{result.Value}", new { id = result.Value })
                : result.Error.ToProblem();
        }
    }
}
