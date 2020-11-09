using SmartDomainDrivenDesign.Domain.Abstract;
using SmartDomainDrivenDesign.OrderSample.Domain.Shared;
using System;

namespace SmartDomainDrivenDesign.OrderSample.Domain.Items
{
    public class ItemPrice : Entity<ItemPrice, AutoIncrementalEntityId>
    {
        public ItemId ItemId { get; private set; }
        public Price UnitPrice { get; private set; }
        public DateTimeOffset ValidPeriodStart { get; private set; }

        public ItemPrice(Price unitPrice, DateTimeOffset validPeriodStart)
        {
            this.UnitPrice = unitPrice;
            this.ValidPeriodStart = validPeriodStart;
        }
    }
}