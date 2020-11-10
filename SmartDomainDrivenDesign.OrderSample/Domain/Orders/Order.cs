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

    public sealed class Order : Entity<Order, OrderId>, IAggregateRoot
    {
        public enum OrderStatus { Placed, Confirmed, Paid }

        public BuyerId BuyerId { get; private set; }

        #region Navigation Properties

        private readonly ICollection<OrderLine> lines;
        public IEnumerable<OrderLine> Lines => lines;

        #endregion

        #region Readonly Properties

        public Price Total => this.lines.Sum(i => i.UnitPrice);

        #endregion

        #region Constructors

        private Order() { }

        private Order(OrderId orderId, BuyerId buyerId, IEnumerable<OrderLine> lines)
        {
            this.Id = orderId;
            this.BuyerId = buyerId;
            this.lines = lines.ToList();

            this.AddDomainEvent(new OrderPlaced(this.Id, this.lines.Select(l => l.ItemId)));
        }

        public static Order PlaceOrder(OrderId orderId, BuyerId buyerId, IEnumerable<(decimal quantity, ItemId itemId)> lines)
            => new Order(orderId, buyerId, lines.Select(q => OrderLine.CreateForItem(q.quantity, q.itemId)));

        #endregion

        public void ConfirmItemPrices(Dictionary<ItemId, Price> prices)
        {
            foreach (OrderLine line in this.lines)
            {
                if (!prices.TryGetValue(line.ItemId, out Price validPrice))
                    throw new ArgumentException("Missing item price with ItemId: " + line.ItemId);

                line.ConfirmPrice(validPrice);
            }

            this.AddDomainEvent(new OrderConfirmed(this.Id));
        }
    }
}