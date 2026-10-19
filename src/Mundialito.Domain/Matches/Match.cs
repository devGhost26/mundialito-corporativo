using Mundialito.Domain.Common;
using Mundialito.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mundialito.Domain.Matches
{
    public sealed class Match : Entity
    {
        public const int VenueMaxLength = 100;

        private readonly List<Goal> _goals = [];

        private Match() { }

        private Match(Guid id, Guid homeTeamId, Guid awayTeamId, DateTime scheduledAtUtc, string? venue)
            : base(id)
        {
            HomeTeamId = homeTeamId;
            AwayTeamId = awayTeamId;
            ScheduledAtUtc = scheduledAtUtc;
            Venue = venue;
            Status = MatchStatus.Scheduled;
        }

        public Guid HomeTeamId { get; private set; }
        public Guid AwayTeamId { get; private set; }
        public DateTime ScheduledAtUtc { get; private set; }
        public string? Venue { get; private set; }
        public MatchStatus Status { get; private set; }
        public int? HomeGoals { get; private set; }
        public int? AwayGoals { get; private set; }

        public IReadOnlyCollection<Goal> Goals => _goals.AsReadOnly();

        public static Result<Match> Schedule(Guid homeTeamId, Guid awayTeamId, DateTime scheduledAtUtc, string? venue)
        {
            if (homeTeamId == awayTeamId)
                return MatchErrors.SameTeams;

            if (venue?.Trim().Length > VenueMaxLength)
                return MatchErrors.VenueTooLong;

            return new Match(Guid.CreateVersion7(), homeTeamId, awayTeamId, scheduledAtUtc, Normalize(venue));
        }

        public Result Reschedule(DateTime scheduledAtUtc, string? venue)
        {
            if (Status == MatchStatus.Played)
                return MatchErrors.AlreadyPlayed;

            if (venue?.Trim().Length > VenueMaxLength)
                return MatchErrors.VenueTooLong;

            ScheduledAtUtc = scheduledAtUtc;
            Venue = Normalize(venue);

            return Result.Success();
        }

        public Result RegisterResult(int homeGoals, int awayGoals, IReadOnlyCollection<ScoredGoal> scoredGoals)
        {
            if (Status == MatchStatus.Played)
                return MatchErrors.AlreadyPlayed;

            if (homeGoals < 0 || awayGoals < 0)
                return MatchErrors.NegativeScore;

            if (scoredGoals.Any(g => g.TeamId != HomeTeamId && g.TeamId != AwayTeamId))
                return MatchErrors.GoalTeamNotInMatch;

            var homeCount = scoredGoals.Count(g => g.TeamId == HomeTeamId);
            var awayCount = scoredGoals.Count - homeCount;

            if (homeCount != homeGoals || awayCount != awayGoals)
                return MatchErrors.ScoreMismatch;

            var goals = new List<Goal>();
            foreach (var scored in scoredGoals)
            {
                var goal = Goal.Create(Id, scored.PlayerId, scored.TeamId, scored.Minute);
                if (goal.IsFailure)
                    return goal.Error;

                goals.Add(goal.Value);
            }

            _goals.AddRange(goals);
            HomeGoals = homeGoals;
            AwayGoals = awayGoals;
            Status = MatchStatus.Played;

            Raise(new MatchResultRegisteredDomainEvent(Id, HomeTeamId, AwayTeamId, homeGoals, awayGoals));

            return Result.Success();
        }

        private static string? Normalize(string? value) =>
            string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
