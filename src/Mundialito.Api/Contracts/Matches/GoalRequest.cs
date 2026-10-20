namespace Mundialito.Api.Contracts.Matches
{
    public sealed record GoalRequest(Guid PlayerId, int Minute);
}
