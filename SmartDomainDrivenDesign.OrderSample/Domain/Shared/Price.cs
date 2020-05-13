using SmartDomainDrivenDesign.Domain.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartDomainDrivenDesign.OrderSample.Domain.Shared
{
    /// <summary>
    /// Represents the price and its operations
    /// </summary>
    public class Price : ValueObject
    {
        public decimal Amount { get; }
        public string Currency { get; }

        protected Price() { }

        public Price(decimal amount, string currency)
        {
            this.Amount = amount;
            this.Currency = currency;
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Amount;
            yield return Currency;
        }

        public static Price operator *(Price price, decimal multiplier) => new Price(price.Amount * multiplier, price.Currency);
        public static Price operator +(Price left, Price right)
        {
            // TODO DomainException
            if (left.Currency != right.Currency) throw new ApplicationException("Only supported prices with the same Currency.");

            return new Price(left.Amount + right.Amount, left.Currency);
        }
    }
}
