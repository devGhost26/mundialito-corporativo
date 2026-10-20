namespace Mundialito.Api.Contracts.Teams
{
    public sealed record ListPlayersRequest(
        int? PageNumber,
        int? PageSize,
        string? SortBy,
        string? SortDirection,
        string? Name,
        string? Position);
}
