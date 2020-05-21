using Convey.CQRS.Commands;
using MediatR;
using SmartDomainDrivenDesign.Infrastructure.Convey;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartDomainDrivenDesign.OrderSample.Application.Orders.Commands
{
    [Contract]
    public class PlaceOrderRequest : ICommand
    {
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
        public async Task HandleAsync(PlaceOrderRequest command)
        {
        }
    }
}
