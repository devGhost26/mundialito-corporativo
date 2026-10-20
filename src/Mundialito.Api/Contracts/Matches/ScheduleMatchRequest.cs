namespace Mundialito.Api.Contracts.Matches
{
    public sealed record ScheduleMatchRequest(Guid HomeTeamId, Guid AwayTeamId, DateTime ScheduledAtUtc, string? Venue);
}
