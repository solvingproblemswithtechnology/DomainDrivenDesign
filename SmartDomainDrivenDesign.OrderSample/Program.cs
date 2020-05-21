using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Threading.Tasks;
using Convey;
using SmartDomainDrivenDesign.Infrastructure.AspNetCore;
using SmartDomainDrivenDesign.OrderSample.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Convey.WebApi;
using Convey.WebApi.CQRS;
using SmartDomainDrivenDesign.Infrastructure.Convey;
using SmartDomainDrivenDesign.OrderSample.Application.Orders.Commands;
using Convey.WebApi.Swagger;
using Convey.Docs.Swagger;
using Convey.CQRS.Commands;
using Convey.CQRS.Queries;

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
                            .WithVersion("1"))
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
                            .Post<PlaceOrderRequest>("api/Orders")
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
