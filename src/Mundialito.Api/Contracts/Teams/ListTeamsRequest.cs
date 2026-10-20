namespace Mundialito.Api.Contracts.Teams
{
    public sealed record ListTeamsRequest(
        int? PageNumber,
        int? PageSize,
        string? SortBy,
        string? SortDirection,
        string? Name,
        string? City);
}
