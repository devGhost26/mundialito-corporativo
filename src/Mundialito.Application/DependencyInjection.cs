using Microsoft.Extensions.DependencyInjection;
using Mundialito.Application.Abstractions;
using Mundialito.Application.Teams.EventHandlers;
using Mundialito.Domain.Matches;
using Mundialito.Domain.Teams;

namespace Mundialito.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IDomainEventHandler<TeamCreatedDomainEvent>, TeamCreatedDomainEventHandler>();
            services.AddScoped<IDomainEventHandler<MatchResultRegisteredDomainEvent>, MatchResultRegisteredDomainEventHandler>();

            return services;
        }
    }
}
