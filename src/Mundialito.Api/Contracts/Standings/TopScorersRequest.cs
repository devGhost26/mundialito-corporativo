namespace Mundialito.Api.Contracts.Standings
{
    public sealed record TopScorersRequest(int? PageNumber, int? PageSize, string? SortBy, string? SortDirection, Guid? TeamId);
}
