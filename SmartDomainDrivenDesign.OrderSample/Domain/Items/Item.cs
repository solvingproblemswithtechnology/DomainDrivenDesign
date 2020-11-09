using SmartDomainDrivenDesign.Domain.Abstract;
using SmartDomainDrivenDesign.OrderSample.Domain.Shared;
using System;
using System.Collections.Generic;

namespace SmartDomainDrivenDesign.OrderSample.Domain.Items
{
    public class ItemId : GuidEntityId
    {
        public ItemId(Guid id) : base(id) { }
    }

    public class Item : Entity<Item, ItemId>
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string MeasureUnit { get; private set; }
        public ItemPrice CurrentPrice { get; private set; }
        public ICollection<ItemPrice> Prices { get; private set; }

        public Item(string name, string description, string measureUnit, ItemPrice currentPrice)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException($"{nameof(name)} no puede ser NULL ni estar vacío.", nameof(name));
            if (string.IsNullOrEmpty(measureUnit))
                throw new ArgumentException($"{nameof(measureUnit)} no puede ser NULL ni estar vacío.", nameof(measureUnit));

            this.Name = name;
            this.Description = description;
            this.MeasureUnit = measureUnit;
            this.CurrentPrice = currentPrice ?? throw new ArgumentNullException(nameof(currentPrice));
            this.Prices = new List<ItemPrice>() { currentPrice };
        }
    }
}