using SmartDomainDrivenDesign.Domain.Abstract;
using SmartDomainDrivenDesign.OrderSample.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartDomainDrivenDesign.OrderSample.Domain.Items
{
    public class Item : Entity<Item>
    {
        public string Name { get; set; }
        public Price UnitPrice { get; set; }
        public string MeasureUnits { get; set; }

        protected Item() { }

        public Item(string name, Price unitPrice, string measureUnits)
        {
            this.Name = name;
            this.UnitPrice = unitPrice;
            this.MeasureUnits = measureUnits;
        }
    }
}
