using SmartDomainDrivenDesign.Domain.Abstract;
using SmartDomainDrivenDesign.OrderSample.Domain.Items;
using SmartDomainDrivenDesign.OrderSample.Domain.Shared;
using SmartDomainDrivenDesign.OrderSample.Domain.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartDomainDrivenDesign.OrderSample.Domain.Orders
{
    public class Order : Entity<Order>
    {
        private List<OrderItem> orderItems;

        public string User { get; }

        public IEnumerable<OrderItem> OrderItems => orderItems;
        public Price Total => this.orderItems.Aggregate(new Price(0, "€"), (s, n) => s + n.UnitPrice);

        public Order(string user, IEnumerable<OrderItem> orderItems)
        {
            this.User = user;
            this.orderItems = orderItems.ToList();
        }

        public static Order PlaceOrder(User user, IEnumerable<(decimal quantity, Item item)> quantities) 
            => new Order(user.Name, quantities.Select(q => OrderItem.CreateFromItem(q.quantity, q.item)));
    }
}
