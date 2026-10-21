using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Mundialito.Application.Abstractions;
using Mundialito.Domain.Common.Results;
using Mundialito.Domain.Matches;
using Mundialito.Domain.Teams;

namespace Mundialito.Infrastructure.Persistence.Seed
{
    internal sealed class DatabaseInitializer(
        AppDbContext dbContext,
        ITeamRepository teamRepository,
        IMatchRepository matchRepository,
        IUnitOfWork unitOfWork,
        ILogger<DatabaseInitializer> logger)
    {
        private const int MaxAttempts = 10;
        private static readonly TimeSpan RetryDelay = TimeSpan.FromSeconds(5);

        public async Task InitializeAsync(CancellationToken cancellationToken)
        {
            await MigrateAsync(cancellationToken);

            if (await dbContext.Teams.AnyAsync(cancellationToken))
            {
                logger.LogInformation("La base ya tiene datos; no se carga el seed");
                return;
            }

            await SeedAsync(cancellationToken);
        }

        private async Task MigrateAsync(CancellationToken cancellationToken)
        {
            for (var attempt = 1; ; attempt++)
            {
                try
                {
                    await dbContext.Database.MigrateAsync(cancellationToken);
                    logger.LogInformation("Migraciones aplicadas");
                    return;
                }
                catch (Exception ex) when (attempt < MaxAttempts && IsConnectionFailure(ex))
                {
                    logger.LogWarning(
                        "La base de datos no responde (intento {Attempt} de {MaxAttempts}); reintento en {DelaySeconds} s",
                        attempt, MaxAttempts, RetryDelay.TotalSeconds);

                    await Task.Delay(RetryDelay, cancellationToken);
                }
            }
        }

        private static bool IsConnectionFailure(Exception ex) =>
            ex is SqlException || ex.InnerException is SqlException;

        private async Task SeedAsync(CancellationToken cancellationToken)
        {
            var teams = SeedData.Teams.Select(CreateTeam).ToList();
            teams.ForEach(teamRepository.Add);

            var (tecnologia, finanzas, operaciones, recursosHumanos) = (teams[0], teams[1], teams[2], teams[3]);
            var today = DateTime.UtcNow.Date.AddHours(20);

            var played1 = Schedule(tecnologia, finanzas, today.AddDays(-14), "Cancha 1");
            var played2 = Schedule(operaciones, recursosHumanos, today.AddDays(-10), "Cancha 2");
            var played3 = Schedule(tecnologia, operaciones, today.AddDays(-7), "Cancha 1");
            Schedule(tecnologia, recursosHumanos, today.AddDays(3), "Cancha 2");
            Schedule(finanzas, operaciones, today.AddDays(7), "Cancha 1");
            Schedule(finanzas, recursosHumanos, today.AddDays(10), "Cancha 2");

            Ensure(played1.RegisterResult(2, 1,
            [
                Goal(tecnologia, 9, 23),
            Goal(finanzas, 8, 51),
            Goal(tecnologia, 9, 78)
            ]));

            Ensure(played2.RegisterResult(0, 0, []));

            Ensure(played3.RegisterResult(1, 1,
            [
                Goal(operaciones, 9, 12),
            Goal(tecnologia, 8, 66)
            ]));

            var commitResult = await unitOfWork.CommitAsync(cancellationToken);
            Ensure(commitResult);

            logger.LogInformation(
                "Seed cargado: {Teams} equipos, {Players} jugadores, {Matches} partidos",
                teams.Count, teams.Sum(t => t.Players.Count), 6);
        }

        private static Team CreateTeam(SeedData.TeamSeed seed)
        {
            var team = Ensure(Team.Create(seed.Name, seed.City));

            foreach (var player in seed.Players)
                Ensure(team.AddPlayer(player.FirstName, player.LastName, player.Number, player.Position));

            return team;
        }

        private Match Schedule(Team home, Team away, DateTime scheduledAtUtc, string venue)
        {
            var match = Ensure(Match.Schedule(home.Id, away.Id, scheduledAtUtc, venue));
            matchRepository.Add(match);
            return match;
        }

        private static ScoredGoal Goal(Team team, int number, int minute)
        {
            var player = team.Players.Single(p => p.Number == number);
            return new ScoredGoal(player.Id, team.Id, minute);
        }

        private static void Ensure(Result result)
        {
            if (result.IsFailure)
                throw new InvalidOperationException($"El seed no es válido: {result.Error.Code} - {result.Error.Message}");
        }

        private static T Ensure<T>(Result<T> result)
        {
            Ensure((Result)result);
            return result.Value;
        }
    }
}
