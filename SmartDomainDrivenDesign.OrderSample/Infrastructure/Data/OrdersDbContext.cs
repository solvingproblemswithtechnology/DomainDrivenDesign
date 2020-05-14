using Microsoft.EntityFrameworkCore;
using SmartDomainDrivenDesign.Infrastructure.EntityFrameworkCore;
using SmartDomainDrivenDesign.OrderSample.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartDomainDrivenDesign.OrderSample.Infrastructure.Data
{
    public class OrdersDbContext : SmartDbContext
    {
        public OrdersDbContext(DbContextOptions<OrdersDbContext> options) : base(options)
        {
        }

        #region Entities

        public Order Orders { get; set; }

        public OrderLine OrderLines { get; set; }

        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>(b =>
            {
                b.HasKey(o => o.Id);
                b.Metadata.FindNavigation(nameof(Order.Lines)).SetPropertyAccessMode(PropertyAccessMode.Field);
            });

            modelBuilder.Entity<OrderLine>(b =>
            {
                b.HasKey(o => o.Id);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
