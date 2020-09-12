using System.Collections.Generic;

namespace SmartDomainDrivenDesign.Domain.Abstract
{
    public interface IDomainEventEntity
    {
        /// <summary>
        /// Entity's DomainEvent
        /// </summary>
        public IReadOnlyCollection<IDomainEvent> DomainEvents { get; }

        /// <summary>
        /// Adds an event to this entity domain events.
        /// </summary>
        /// <param name="eventItem"></param>
        void AddDomainEvent(IDomainEvent eventItem);

        /// <summary>
        /// Clears all the events in the entity.
        /// </summary>
        /// <param name="eventItem"></param>
        void RemoveDomainEvent(IDomainEvent eventItem);

        /// <summary>
        /// Clears all the events in the entity.
        /// </summary>
        /// <param name="eventItem"></param>
        void ClearDomainEvents();
    }
}
