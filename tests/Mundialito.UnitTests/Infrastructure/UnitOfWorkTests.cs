using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Mundialito.Application.Abstractions;
using Mundialito.Domain.Matches;
using Mundialito.Domain.Teams;
using Mundialito.Infrastructure.Events;
using Mundialito.Infrastructure.Persistence;
using Mundialito.Infrastructure.Repositories;

namespace Mundialito.UnitTests.Infrastructure
{
    public sealed class UnitOfWorkTests : IDisposable
    {
        private readonly SqliteConnection _connection = new("DataSource=:memory:");
        private readonly RecordingTeamCreatedHandler _handler = new();
        private readonly ServiceProvider _services;

        public UnitOfWorkTests()
        {
            _connection.Open();

            _services = new ServiceCollection()
                .AddLogging()
                .AddDbContext<AppDbContext>(options => options.UseSqlite(_connection))
                .AddScoped<IUnitOfWork, UnitOfWork>()
                .AddScoped<ITeamRepository, TeamRepository>()
                .AddScoped<IMatchRepository, MatchRepository>()
                .AddScoped<DomainEventDispatcher>()
                .AddSingleton<IDomainEventHandler<TeamCreatedDomainEvent>>(_handler)
                .BuildServiceProvider();

            using var scope = _services.CreateScope();
            scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.EnsureCreated();
        }

        [Fact]
        public async Task Agregar_al_repositorio_no_guarda_nada_hasta_el_commit()
        {
            using var scope = _services.CreateScope();
            var teams = scope.ServiceProvider.GetRequiredService<ITeamRepository>();

            teams.Add(Team.Create("Halcones", null).Value);

            Assert.Equal(0, CountTeams());

            await scope.ServiceProvider.GetRequiredService<IUnitOfWork>().CommitAsync();

            Assert.Equal(1, CountTeams());
        }

        [Fact]
        public async Task Un_commit_guarda_los_cambios_de_varios_repositorios_juntos()
        {
            using var scope = _services.CreateScope();
            var teams = scope.ServiceProvider.GetRequiredService<ITeamRepository>();
            var matches = scope.ServiceProvider.GetRequiredService<IMatchRepository>();

            var home = Team.Create("Halcones", null).Value;
            var away = Team.Create("Jaguares", null).Value;
            teams.Add(home);
            teams.Add(away);
            matches.Add(Match.Schedule(home.Id, away.Id, DateTime.UtcNow, null).Value);

            var result = await scope.ServiceProvider.GetRequiredService<IUnitOfWork>().CommitAsync();

            Assert.True(result.IsSuccess);
            Assert.Equal(2, CountTeams());
            Assert.Equal(1, CountMatches());
        }

        [Fact]
        public async Task Los_eventos_se_publican_despues_del_commit_y_se_limpian()
        {
            using var scope = _services.CreateScope();
            var team = Team.Create("Halcones", null).Value;
            scope.ServiceProvider.GetRequiredService<ITeamRepository>().Add(team);

            Assert.Empty(_handler.Received);

            await scope.ServiceProvider.GetRequiredService<IUnitOfWork>().CommitAsync();

            var published = Assert.Single(_handler.Received);
            Assert.Equal(team.Id, published.TeamId);
            Assert.Empty(team.DomainEvents);
        }

        [Fact]
        public async Task Si_el_commit_falla_no_se_guarda_nada_ni_se_publican_eventos()
        {
            using var scope = _services.CreateScope();
            var teams = scope.ServiceProvider.GetRequiredService<ITeamRepository>();

            teams.Add(Team.Create("Halcones", null).Value);
            teams.Add(Team.Create("Halcones", null).Value);

            await Assert.ThrowsAsync<DbUpdateException>(
                () => scope.ServiceProvider.GetRequiredService<IUnitOfWork>().CommitAsync());

            Assert.Equal(0, CountTeams());
            Assert.Empty(_handler.Received);
        }

        private int CountTeams()
        {
            using var scope = _services.CreateScope();
            return scope.ServiceProvider.GetRequiredService<AppDbContext>().Teams.Count();
        }

        private int CountMatches()
        {
            using var scope = _services.CreateScope();
            return scope.ServiceProvider.GetRequiredService<AppDbContext>().Matches.Count();
        }

        public void Dispose()
        {
            _services.Dispose();
            _connection.Dispose();
        }
    }
}
