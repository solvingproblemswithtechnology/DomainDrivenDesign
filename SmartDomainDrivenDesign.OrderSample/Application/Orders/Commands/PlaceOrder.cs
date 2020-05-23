using Convey.CQRS.Commands;
using SmartDomainDrivenDesign.Infrastructure.Convey;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartDomainDrivenDesign.OrderSample.Application.Orders
{
    [Contract]
    public class PlaceOrderRequest : ICommand
    {
        public PlaceOrderRequest(Guid orderId, Guid userId, IEnumerable<PlaceOrderItemQuantity> itemsIds)
        {
            this.OrderId = orderId;
            this.UserId = userId;
            this.ItemsIds = itemsIds;
        }

        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public IEnumerable<PlaceOrderItemQuantity> ItemsIds { get; set; }
    }

    public class PlaceOrderItemQuantity
    {
        public decimal Units { get; set; }
        public Guid ItemId { get; set; }
    }

    public class PlaceOrderHandler : ICommandHandler<PlaceOrderRequest>
    {
        public PlaceOrderHandler()
        {

        }

        public async Task HandleAsync(PlaceOrderRequest command)
        {
        }
    }
}
