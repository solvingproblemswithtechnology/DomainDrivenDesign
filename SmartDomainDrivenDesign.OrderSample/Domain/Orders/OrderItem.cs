using SmartDomainDrivenDesign.Domain.Abstract;
using SmartDomainDrivenDesign.OrderSample.Domain.Items;
using SmartDomainDrivenDesign.OrderSample.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartDomainDrivenDesign.OrderSample.Domain.Orders
{
    public class OrderItem : Entity<OrderItem>
    {
        public string Item { get; set; }
        public Quantity Quantity { get; set; }
        public Price UnitPrice { get; set; }

        public Price Total => this.UnitPrice * this.Quantity.Units;

        protected OrderItem() { }

        public OrderItem(string item, Quantity quantity, Price unitPrice)
        {
            this.Item = item;
            this.Quantity = quantity;
            this.UnitPrice = unitPrice;
        }

        public static OrderItem CreateFromItem(decimal quantity, Item item) 
            => new OrderItem(item.Name, new Quantity(quantity, item.MeasureUnits), item.UnitPrice);
    }
}
