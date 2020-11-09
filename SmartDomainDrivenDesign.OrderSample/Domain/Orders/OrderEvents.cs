using SmartDomainDrivenDesign.Domain.Abstract;
using SmartDomainDrivenDesign.OrderSample.Domain.Items;
using System.Collections.Generic;

namespace SmartDomainDrivenDesign.OrderSample.Domain.Orders
{
    public class OrderPlaced : IDomainEvent
    {
        public OrderId OrderId { get; set; }
        public IEnumerable<ItemId> Items { get; set; }

        public OrderPlaced(OrderId orderId, IEnumerable<ItemId> items)
        {
            this.OrderId = orderId;
            this.Items = items;
        }
    }

    public class OrderConfirmed : IDomainEvent
    {
        public OrderId OrderId { get; set; }

        public OrderConfirmed(OrderId orderId)
        {
            this.OrderId = orderId;
        }
    }
}
