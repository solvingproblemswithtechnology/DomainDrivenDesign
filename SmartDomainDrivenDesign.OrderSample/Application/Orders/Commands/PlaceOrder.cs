using Convey.CQRS.Commands;
using MediatR;
using SmartDomainDrivenDesign.Domain.Abstract;
using SmartDomainDrivenDesign.Infrastructure.Convey;
using SmartDomainDrivenDesign.OrderSample.Domain.Items;
using SmartDomainDrivenDesign.OrderSample.Domain.Orders;
using SmartDomainDrivenDesign.OrderSample.Domain.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartDomainDrivenDesign.OrderSample.Application.Orders
{
    [Contract]
    public class PlaceOrderRequest : ICommand
    {
        public Guid OrderId { get; set; }
        public Guid BuyerId { get; set; }
        public IEnumerable<PlaceOrderItemQuantity> ItemQuentities { get; set; }

        public PlaceOrderRequest(Guid orderId, Guid userId, IEnumerable<PlaceOrderItemQuantity> itemsIds)
        {
            this.OrderId = orderId;
            this.BuyerId = userId;
            this.ItemQuentities = itemsIds;
        }
    }

    public class PlaceOrderItemQuantity
    {
        public decimal Quantity { get; set; }
        public Guid ItemId { get; set; }
    }

    public class PlaceOrderHandler : ICommandHandler<PlaceOrderRequest>
    {
        private readonly IRepository<Order, OrderId> orderRepository;

        public PlaceOrderHandler(IRepository<Order, OrderId> orderRepository)
        {
            this.orderRepository = orderRepository;
        }

        public async Task HandleAsync(PlaceOrderRequest command)
        {
            OrderId orderId = new OrderId(command.OrderId);
            BuyerId buyerId = new BuyerId(command.BuyerId);
            //Order order = Order.PlaceOrder(orderId, buyerId, command.ItemQuentities.Select(id => (id.Quantity, new ItemId(id.ItemId))));

            //orderRepository.Add(order);

            await orderRepository.UnitOfWork.SaveEntitiesAsync().ConfigureAwait(false);
        }
    }
}