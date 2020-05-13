using SmartDomainDrivenDesign.Domain.Abstract;
using SmartDomainDrivenDesign.OrderSample.Domain.Items;
using SmartDomainDrivenDesign.OrderSample.Domain.Shared;
using SmartDomainDrivenDesign.OrderSample.Domain.Users;
using System.Collections.Generic;
using System.Linq;

namespace SmartDomainDrivenDesign.OrderSample.Domain.Orders
{
    public class Order : Entity<Order>
    {
        private ICollection<OrderLine> lines;

        public string User { get; private set; }

        public IEnumerable<OrderLine> Lines => lines;
        public Price Total => this.lines.Sum(i => i.UnitPrice);

        public Order(string user, IEnumerable<OrderLine> orderItems)
        {
            this.User = user;
            this.lines = orderItems.ToList();
        }

        public static Order PlaceOrder(User user, IEnumerable<(decimal quantity, Item item)> quantities)
            => new Order(user.Name, quantities.Select(q => OrderLine.CreateForItem(q.quantity, q.item)));
    }
}
