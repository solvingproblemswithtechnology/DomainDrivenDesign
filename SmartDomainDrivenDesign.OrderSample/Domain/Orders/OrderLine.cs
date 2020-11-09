using SmartDomainDrivenDesign.Domain.Abstract;
using SmartDomainDrivenDesign.OrderSample.Domain.Items;
using SmartDomainDrivenDesign.OrderSample.Domain.Shared;

namespace SmartDomainDrivenDesign.OrderSample.Domain.Orders
{
    public class OrderLine : Entity<OrderLine, AutoIncrementalEntityId>
    {
        public ItemId ItemId { get; private set; }
        public decimal Quantity { get; private set; }
        public Price UnitPrice { get; private set; }

        public Price Total => this.UnitPrice * this.Quantity;

        private OrderLine() { }

        public OrderLine(ItemId itemId, decimal quantity)
        {
            this.ItemId = itemId;
            this.Quantity = quantity;
        }

        public static OrderLine CreateForItem(decimal quantity, ItemId itemId)
            => new OrderLine(itemId, quantity);

        public void ConfirmPrice(Price validUnitPrice)
        {
            this.UnitPrice = validUnitPrice;
        }
    }
}
