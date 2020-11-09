using System;
using System.Collections.Generic;

namespace SmartDomainDrivenDesign.OrderSample.Domain.Items
{
    public interface IItemRepository
    {
        public ItemPrice FindItemPriceByDate(ItemId itemId, DateTimeOffset validityDate);
    }
}
