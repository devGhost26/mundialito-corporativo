using Mediator;
using Mundialito.Api.Contracts.Teams;
using Mundialito.Api.Extensions;
using Mundialito.Application.Teams.CreateTeam;

namespace Mundialito.Api.Endpoints
{
    public static class TeamEndpoints
    {
        public static IEndpointRouteBuilder MapTeamEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/teams").WithTags("Teams");

            group.MapPost("/", CreateTeam);

            return app;
        }

        private static async Task<IResult> CreateTeam(CreateTeamRequest request, ISender sender, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new CreateTeamCommand(request.Name, request.City), cancellationToken);

            return result.IsSuccess
                ? Results.Created($"/api/teams/{result.Value}", new { id = result.Value })
                : result.Error.ToProblem();
        }
    }
}
