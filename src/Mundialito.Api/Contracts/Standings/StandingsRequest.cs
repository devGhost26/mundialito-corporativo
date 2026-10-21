namespace Mundialito.Api.Contracts.Standings
{
    public sealed record StandingsRequest(int? PageNumber, int? PageSize, string? SortBy, string? SortDirection);
}
