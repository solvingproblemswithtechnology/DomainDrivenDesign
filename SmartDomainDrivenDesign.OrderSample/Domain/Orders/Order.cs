using SmartDomainDrivenDesign.Domain.Abstract;
using SmartDomainDrivenDesign.OrderSample.Domain.Items;
using SmartDomainDrivenDesign.OrderSample.Domain.Shared;
using SmartDomainDrivenDesign.OrderSample.Domain.Users;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartDomainDrivenDesign.OrderSample.Domain.Orders
{
    public class OrderId : GuidEntityId
    {
        public OrderId(Guid id) : base(id) { }
    }

    public class Order : Entity<Order, OrderId>, IAggregateRoot
    {
        public BuyerId BuyerId { get; private set; }

        #region Navigation Properties

        private readonly ICollection<OrderLine> lines;
        public IEnumerable<OrderLine> Lines => lines;

        #endregion

        #region Readonly Properties

        public Price Total => this.lines.Sum(i => i.UnitPrice);

        #endregion

        private Order() { }

        public Order(OrderId orderId, BuyerId buyerId, IEnumerable<OrderLine> lines)
        {
            this.Id = orderId;
            this.BuyerId = buyerId;
            this.lines = lines.ToList();
        }

        public static Order PlaceOrder(OrderId orderId, BuyerId buyerId, IEnumerable<(decimal quantity, ItemId itemId)> quantities)
            => new Order(orderId, buyerId, quantities.Select(q => OrderLine.CreateForItem(q.quantity, q.itemId)));
    }
}
