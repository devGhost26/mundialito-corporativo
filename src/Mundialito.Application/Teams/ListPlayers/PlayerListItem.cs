using Mundialito.Domain.Teams;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mundialito.Application.Teams.ListPlayers
{
    public sealed record PlayerListItem
    {
        public Guid Id { get; init; }
        public Guid TeamId { get; init; }
        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;
        public int Number { get; init; }
        public PlayerPosition Position { get; init; }
    }
}
