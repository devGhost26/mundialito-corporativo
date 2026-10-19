using Mundialito.Domain.Common;
using Mundialito.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mundialito.Domain.Teams
{
    public sealed class Team : Entity
    {
        public const int NameMaxLength = 100;
        public const int CityMaxLength = 100;

        private readonly List<Player> _players = [];

        private Team() { }

        private Team(Guid id, string name, string? city, DateTime createdAtUtc) : base(id)
        {
            Name = name;
            City = city;
            CreatedAtUtc = createdAtUtc;
        }

        public string Name { get; private set; } = string.Empty;
        public string? City { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }

        public IReadOnlyCollection<Player> Players => _players.AsReadOnly();

        public static Result<Team> Create(string name, string? city)
        {
            var validation = Validate(name, city);
            if (validation.IsFailure)
                return validation.Error;

            var team = new Team(Guid.CreateVersion7(), name.Trim(), Normalize(city), DateTime.UtcNow);

            team.Raise(new TeamCreatedDomainEvent(team.Id, team.Name));

            return team;
        }

        public Result Update(string name, string? city)
        {
            var validation = Validate(name, city);
            if (validation.IsFailure)
                return validation;

            Name = name.Trim();
            City = Normalize(city);

            return Result.Success();
        }

        public Result<Player> AddPlayer(string firstName, string lastName, int number, PlayerPosition position)
        {
            if (_players.Any(p => p.Number == number))
                return TeamErrors.NumberTaken(number);

            var result = Player.Create(Id, firstName, lastName, number, position);
            if (result.IsFailure)
                return result.Error;

            _players.Add(result.Value);

            return result.Value;
        }

        private static Result Validate(string name, string? city)
        {
            if (string.IsNullOrWhiteSpace(name))
                return TeamErrors.NameRequired;

            if (name.Trim().Length > NameMaxLength)
                return TeamErrors.NameTooLong;

            if (city?.Trim().Length > CityMaxLength)
                return TeamErrors.CityTooLong;

            return Result.Success();
        }

        private static string? Normalize(string? value) =>
            string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
