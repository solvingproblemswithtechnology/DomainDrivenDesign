using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Logging;

namespace SmartDomainDrivenDesign.WebApiExample
{
    public static class Program
    {
        public static void Main(string[] args) => CreateHostBuilder(args).Build().Run();

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureLogging((ctx, builder) =>
                {
                    builder.ClearProviders();

                    if (ctx.HostingEnvironment.IsDevelopment()) builder.AddConsole();
                })
                .UseSerilog((ctx, builder) => builder.Enrich.FromLogContext()
                        .ReadFrom.Configuration(ctx.Configuration)
                        .WriteTo.Async(async => async.Debug())
                        .WriteTo.Seq("http://localhost:9000", apiKey: "bXJd0BOdMJILdcEaJ7hs"), writeToProviders: true);
    }
}
