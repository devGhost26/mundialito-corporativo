using System;
using System.Collections.Generic;
using System.Text;

namespace Mundialito.Application.Teams.ListTeams
{
    public sealed record TeamListItem
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string? City { get; init; }
        public int PlayerCount { get; init; }
        public DateTimeOffset CreatedAtUtc { get; init; }
    }
}
