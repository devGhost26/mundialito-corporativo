using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mundialito.Application.Abstractions;
using Mundialito.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mundialito.Infrastructure.Events
{
    internal sealed class DomainEventDispatcher(IServiceProvider serviceProvider, ILogger<DomainEventDispatcher> logger)
    {
        public async Task DispatchAsync(IReadOnlyCollection<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
        {
            foreach (var domainEvent in domainEvents)
            {
                var eventName = domainEvent.GetType().Name;

                logger.LogDebug("Publicando evento {EventName} {EventId}", eventName, domainEvent.EventId);

                var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());
                var handleMethod = handlerType.GetMethod("HandleAsync")!;

                foreach (var handler in serviceProvider.GetServices(handlerType))
                {
                    try
                    {
                        await (Task)handleMethod.Invoke(handler, [domainEvent, cancellationToken])!;
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Falló un handler del evento {EventName} {EventId}", eventName, domainEvent.EventId);
                    }
                }
            }
        }
    }
}
