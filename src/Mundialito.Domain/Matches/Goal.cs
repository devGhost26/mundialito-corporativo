using Mundialito.Domain.Common;
using Mundialito.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mundialito.Domain.Matches
{
    public sealed class Goal : Entity
    {
        public const int MaxMinute = 120;

        private Goal() { }

        private Goal(Guid id, Guid matchId, Guid playerId, Guid teamId, int minute) : base(id)
        {
            MatchId = matchId;
            PlayerId = playerId;
            TeamId = teamId;
            Minute = minute;
        }

        public Guid MatchId { get; private set; }
        public Guid PlayerId { get; private set; }
        public Guid TeamId { get; private set; }
        public int Minute { get; private set; }

        internal static Result<Goal> Create(Guid matchId, Guid playerId, Guid teamId, int minute)
        {
            if (minute is < 1 or > MaxMinute)
                return MatchErrors.InvalidMinute;

            return new Goal(Guid.CreateVersion7(), matchId, playerId, teamId, minute);
        }
    }
}
