﻿using SmartDomainDrivenDesign.Domain.Abstract;
using SmartDomainDrivenDesign.OrderSample.Domain.Items;
using SmartDomainDrivenDesign.OrderSample.Domain.Shared;

namespace SmartDomainDrivenDesign.OrderSample.Domain.Orders
{
    public class OrderLine : Entity<OrderLine, AutoIncrementalEntityId>
    {
        public string Item { get; private set; }
        public Quantity Quantity { get; private set; }
        public Price UnitPrice { get; private set; }

        public Price Total => this.UnitPrice * this.Quantity.Units;

        private OrderLine() { }

        public OrderLine(string item, Quantity quantity, Price unitPrice)
        {
            this.Item = item;
            this.Quantity = quantity;
            this.UnitPrice = unitPrice;
        }

        public static OrderLine CreateForItem(decimal quantity, ItemId itemId)
            => new OrderLine(item.Name, new Quantity(quantity, item.MeasureUnits), item.UnitPrice);
    }
}
