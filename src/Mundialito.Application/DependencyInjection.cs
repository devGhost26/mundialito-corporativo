using Microsoft.Extensions.DependencyInjection;
using Mundialito.Application.Abstractions;
using Mundialito.Application.Behaviors;
using Mundialito.Application.Matches.EventHandlers;
using Mundialito.Application.Teams.EventHandlers;
using Mundialito.Domain.Matches;
using Mundialito.Domain.Teams;

namespace Mundialito.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediator(options =>
            {
                options.ServiceLifetime = ServiceLifetime.Scoped;
                options.PipelineBehaviors = [typeof(LoggingBehavior<,>)];
            });
            services.AddScoped<IDomainEventHandler<TeamCreatedDomainEvent>, TeamCreatedDomainEventHandler>();
            services.AddScoped<IDomainEventHandler<MatchResultRegisteredDomainEvent>, MatchResultRegisteredDomainEventHandler>();

            return services;
        }
    }
}
