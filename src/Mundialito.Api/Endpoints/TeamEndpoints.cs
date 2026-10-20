using Mediator;
using Mundialito.Api.Contracts.Teams;
using Mundialito.Api.Extensions;
using Mundialito.Application.Teams.AddPlayer;
using Mundialito.Application.Teams.CreateTeam;
using Mundialito.Application.Teams.DeleteTeam;
using Mundialito.Application.Teams.PatchTeam;
using Mundialito.Application.Teams.UpdateTeam;

namespace Mundialito.Api.Endpoints
{
    public static class TeamEndpoints
    {
        public static IEndpointRouteBuilder MapTeamEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/teams").WithTags("Teams");

            group.MapPost("/", CreateTeam);
            group.MapPut("/{teamId:guid}", UpdateTeam);
            group.MapPatch("/{teamId:guid}", PatchTeam);
            group.MapDelete("/{teamId:guid}", DeleteTeam);
            group.MapPost("/{teamId:guid}/players", AddPlayer);

            return app;
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
