using Convey;
using Convey.CQRS.Commands;
using Convey.CQRS.Queries;
using Convey.Docs.Swagger;
using Convey.WebApi;
using Convey.WebApi.CQRS;
using Convey.WebApi.Swagger;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using SmartDomainDrivenDesign.Infrastructure.AspNetCore;
using SmartDomainDrivenDesign.Infrastructure.Convey;
using SmartDomainDrivenDesign.OrderSample.Application.Orders;
using SmartDomainDrivenDesign.OrderSample.Infrastructure.Data;
using System.Threading.Tasks;

namespace SmartDomainDrivenDesign.WebApiExample
{
    public static class Program
    {
        public static Task Main(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddSmartDbContextPool<OrdersDbContext>(opt => opt.UseSqlite("Data Source=orders.db"));

                    services.AddConvey()
                        .AddWebApi()
                        .AddWebApiSwaggerDocs(options => options
                            .Enable(true)
                            .WithName("Order Sample API")
                            .WithTitle("Order Sample API")
                            .WithVersion("1")
                            .WithRoutePrefix("swagger"))
                        .AddCommandHandlers()
                        .AddQueryHandlers()
                        .AddInMemoryCommandDispatcher()
                        .AddInMemoryQueryDispatcher()
                        .Build();
                })
                .Configure(app =>
                {
                    app.UseSwaggerDocs()
                        .UsePublicContracts<Contract>()
                        .UseDispatcherEndpoints(endpoints => endpoints
                            .Get<GetOrdersRequest, GetOrdersResponse>("api/Order1")
                            .Post<PlaceOrderRequest>("api/Order2")
                        );
                })
                .ConfigureLogging((ctx, builder) =>
                {
                    builder.ClearProviders();

                    if (ctx.HostingEnvironment.IsDevelopment()) builder.AddConsole();
                })
                .UseSerilog((ctx, builder) => builder.Enrich.FromLogContext()
                        .ReadFrom.Configuration(ctx.Configuration)
                        .WriteTo.Async(async => async.Debug())
                        .WriteTo.Seq("http://localhost:5341", apiKey: "c7QB5qUurSbA3E6Sn6iM"), writeToProviders: true)
                .Build().RunAsync();
        }
    }
}
