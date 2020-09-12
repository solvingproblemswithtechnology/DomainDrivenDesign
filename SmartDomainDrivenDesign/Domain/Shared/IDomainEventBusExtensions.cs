using SmartDomainDrivenDesign.Domain.Abstract;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartDomainDrivenDesign.Domain.Shared
{
    public static class IDomainEventBusExtensions
    {
        /// <summary>
        /// Dispatches all Domain Events.
        /// </summary>
        /// <param name="eventBus"></param>
        /// <param name="entities"></param>
        /// <returns></returns>
        public static async Task DispatchDomainEventsAsync(this IDomainEventBus eventBus, IEnumerable<IDomainEventEntity> entities)
        {
            foreach (IDomainEventEntity entity in entities)
            {
                foreach (IDomainEvent domainEvent in entity.DomainEvents)
                {
                    entity.RemoveDomainEvent(domainEvent);
                    await eventBus.Publish(domainEvent).ConfigureAwait(false);
                }
            }
        }
    }
}
