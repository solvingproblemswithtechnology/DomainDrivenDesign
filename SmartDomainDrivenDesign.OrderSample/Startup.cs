using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SmartDomainDrivenDesign.Infrastructure.AspNetCore;
using SmartDomainDrivenDesign.OrderSample.Infrastructure.Data;

namespace SmartDomainDrivenDesign.WebApiExample
{
    public class Startup
    {
        private readonly IConfiguration configuration;
        private readonly IHostEnvironment environment;

        public Startup(IConfiguration configuration, IHostEnvironment environment)
        {
            this.configuration = configuration;
            this.environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSmartDbContextPool<OrdersDbContext>(opt => opt.UseSqlite("Data Source=orders.db"));
            var unused = services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
