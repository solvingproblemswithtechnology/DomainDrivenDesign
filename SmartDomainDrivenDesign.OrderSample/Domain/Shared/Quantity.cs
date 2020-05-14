using SmartDomainDrivenDesign.Domain.Abstract;
using System.Collections.Generic;

namespace SmartDomainDrivenDesign.OrderSample.Domain.Shared
{
    /// <summary>
    /// Represents the quantity and its operations
    /// </summary>
    public class Quantity : ValueObject
    {
        public decimal Units { get; }
        public string MeasureUnits { get; set; }

        private Quantity() { }

        public Quantity(decimal units, string measureUnits)
        {
            this.Units = units;
            this.MeasureUnits = measureUnits;
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Units;
            yield return MeasureUnits;
        }
    }
}
