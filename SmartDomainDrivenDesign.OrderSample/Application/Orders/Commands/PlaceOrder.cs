using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SmartDomainDrivenDesign.OrderSample.Application.Orders.Commands
{
    public static class PlaceOrder
    {
        public class Request : IRequest
        {
            public Guid OrderId { get; set; }
            public Guid UserId { get; set; }
            public IEnumerable<ItemQuantity> ItemsIds { get; set; }
        }

        public class ItemQuantity
        {
            public decimal Units { get; set; }
            public Guid ItemId { get; set; }
        }

        public class Handler : IRequestHandler<Request>
        {
            public Handler()
            {
            }

            public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {
                return Unit.Value;
            }
        }
    }
}
