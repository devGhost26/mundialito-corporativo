using Mundialito.Domain.Teams;

namespace Mundialito.Api.Contracts.Teams
{
    public sealed record AddPlayerRequest(string FirstName, string LastName, int Number, PlayerPosition Position);
}
