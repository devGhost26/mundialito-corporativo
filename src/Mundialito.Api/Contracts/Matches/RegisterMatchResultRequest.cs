namespace Mundialito.Api.Contracts.Matches
{
    public sealed record RegisterMatchResultRequest(int HomeGoals, int AwayGoals, IReadOnlyList<GoalRequest>? Goals);
}
