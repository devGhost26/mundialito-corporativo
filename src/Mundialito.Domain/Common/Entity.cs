using System;
using System.Collections.Generic;
using System.Text;

namespace Mundialito.Domain.Common
{
    public abstract class Entity
    {
        private readonly List<IDomainEvent> _domainEvents = [];
        protected Entity(Guid id)
        {
            Id = id;
        }

        protected Entity() { }
        public Guid Id { get; protected set; }
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
        protected void Raise(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
        public void ClearDomainEvents() => _domainEvents.Clear();

    }
}
