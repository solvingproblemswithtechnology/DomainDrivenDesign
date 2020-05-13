using SmartDomainDrivenDesign.Domain.Abstract;
using SmartDomainDrivenDesign.OrderSample.Domain.Shared;

namespace SmartDomainDrivenDesign.OrderSample.Domain.Items
{
    public class Item : Entity<Item>
    {
        public string Name { get; set; }
        public Price UnitPrice { get; set; }
        public string MeasureUnits { get; set; }

        public Item(string name, Price unitPrice, string measureUnits)
        {
            this.Name = name;
            this.UnitPrice = unitPrice;
            this.MeasureUnits = measureUnits;
        }
    }
}
