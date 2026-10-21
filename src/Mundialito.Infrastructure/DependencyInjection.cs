using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mundialito.Application.Abstractions;
using Mundialito.Application.Matches;
using Mundialito.Application.Standings;
using Mundialito.Application.Teams;
using Mundialito.Domain.Matches;
using Mundialito.Domain.Teams;
using Mundialito.Infrastructure.Events;
using Mundialito.Infrastructure.Idempotency;
using Mundialito.Infrastructure.Persistence;
using Mundialito.Infrastructure.Queries;
using Mundialito.Infrastructure.Repositories;

namespace Mundialito.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("Mundialito")
                ?? throw new InvalidOperationException("Falta la cadena de conexión 'Mundialito'.");

            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ITeamRepository, TeamRepository>();
            services.AddScoped<IMatchRepository, MatchRepository>();
            services.AddScoped<DomainEventDispatcher>();
            services.AddSingleton<IIdempotencyStore, IdempotencyStore>();
            services.AddSingleton(new SqlConnectionFactory(connectionString));
            services.AddScoped<ITeamQueries, TeamQueries>();
            services.AddScoped<IMatchQueries, MatchQueries>();
            services.AddScoped<IStandingsQueries, StandingsQueries>();
            return services;
        }



    }
}
