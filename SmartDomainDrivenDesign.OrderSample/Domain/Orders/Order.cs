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
        public string User { get; private set; }

        #region Navigation Properties

        private readonly ICollection<OrderLine> lines;
        public IEnumerable<OrderLine> Lines => lines;

        #endregion

        #region Readonly Properties

        public Price Total => this.lines.Sum(i => i.UnitPrice);

        #endregion

        private Order() { }

        public Order(string user, IEnumerable<OrderLine> lines)
        {
            this.User = user;
            this.lines = lines.ToList();
        }

        public static Order PlaceOrder(User user, IEnumerable<(decimal quantity, Item item)> quantities)
            => new Order(user.Name, quantities.Select(q => OrderLine.CreateForItem(q.quantity, q.item)));
    }
}
