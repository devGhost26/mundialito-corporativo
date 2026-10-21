namespace Mundialito.Api.Contracts.Matches
{
    public sealed record ListMatchesRequest(
        int? PageNumber,
        int? PageSize,
        string? SortBy,
        string? SortDirection,
        DateTime? From,
        DateTime? To,
        Guid? TeamId,
        string? Status);
}
