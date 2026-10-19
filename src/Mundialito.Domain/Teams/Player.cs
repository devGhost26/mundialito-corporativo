using Mundialito.Domain.Common;
using Mundialito.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mundialito.Domain.Teams
{
    public sealed class Player : Entity
    {
        public const int NameMaxLength = 60;

        private Player() { }

        private Player(Guid id, Guid teamId, string firstName, string lastName, int number, PlayerPosition position)
            : base(id)
        {
            TeamId = teamId;
            FirstName = firstName;
            LastName = lastName;
            Number = number;
            Position = position;
        }

        public Guid TeamId { get; private set; }
        public string FirstName { get; private set; } = string.Empty;
        public string LastName { get; private set; } = string.Empty;
        public int Number { get; private set; }
        public PlayerPosition Position { get; private set; }

        internal static Result<Player> Create(
            Guid teamId, string firstName, string lastName, int number, PlayerPosition position)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                return PlayerErrors.FirstNameRequired;

            if (string.IsNullOrWhiteSpace(lastName))
                return PlayerErrors.LastNameRequired;

            if (number is < 1 or > 99)
                return PlayerErrors.InvalidNumber;

            if (!Enum.IsDefined(position))
                return PlayerErrors.InvalidPosition;

            return new Player(Guid.CreateVersion7(), teamId, firstName.Trim(), lastName.Trim(), number, position);
        }
    }
}
