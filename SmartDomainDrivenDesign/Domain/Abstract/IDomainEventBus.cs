using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartDomainDrivenDesign.Domain.Abstract
{
    public interface IDomainEventBus
    {
        Task Publish(IDomainEvent domainEvent);
    }
}
