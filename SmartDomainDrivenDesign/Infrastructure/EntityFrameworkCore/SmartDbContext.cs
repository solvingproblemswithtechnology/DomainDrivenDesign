using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartDomainDrivenDesign.Domain.Abstract;
using SmartDomainDrivenDesign.Domain.Shared;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SmartDomainDrivenDesign.Infrastructure.EntityFrameworkCore
{
    /// <summary>
    /// Proporciona la funcionalidad de Auditoría y de añadir los ValueObjects automáticamente
    /// </summary>
    public abstract class SmartDbContext : DbContext, IUnitOfWork
    {
        /// <summary>
        /// Event bus
        /// </summary>
        private readonly IDomainEventBus domainEventBus;

        /// <summary>
        /// The current user to Audit
        /// </summary>
        public string CurrentUser { get; set; }

        /// <summary>
        /// Default constructor. Use the one with IDomainEventBus
        /// </summary>
        /// <param name="options"></param>
        protected SmartDbContext(DbContextOptions options) : base(options) { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="options"></param>
        /// <param name="domainEventBus"></param>
        protected SmartDbContext(DbContextOptions options, IDomainEventBus domainEventBus) : base(options)
        {
            this.domainEventBus = domainEventBus;
        }

        #region OnModelCreating

        /// <summary>
        ///     Override this method to further configure the model that was discovered by convention from the entity types
        ///     exposed in <see cref="DbSet{TEntity}" /> properties on your derived context. The resulting model may be cached
        ///     and re-used for subsequent instances of your derived context.
        /// </summary>
        /// <remarks>
        ///     If a model is explicitly set on the options for this context (via <see cref="M:Microsoft.EntityFrameworkCore.DbContextOptionsBuilder.UseModel(Microsoft.EntityFrameworkCore.Metadata.IModel)" />)
        ///     then this method will not be run.
        /// </remarks>
        /// <param name="modelBuilder">
        ///     The builder being used to construct the model for this context. Databases (and other extensions) typically
        ///     define extension methods on this object that allow you to configure aspects of the model that are specific
        ///     to a given database.
        /// </param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            this.ConfigureAllValueObjectAsOwned(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }

        #endregion

        #region SaveChanges

        /// <summary>
        ///     <para>
        ///         Saves all changes made in this context to the database.
        ///     </para>
        ///     <para>
        ///         This method will automatically call <see cref="M:Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker.DetectChanges" /> to discover any
        ///         changes to entity instances before saving to the underlying database. This can be disabled via
        ///         <see cref="P:Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker.AutoDetectChangesEnabled" />.
        ///     </para>
        /// </summary>
        /// <returns>
        ///     The number of state entries written to the database.
        /// </returns>
        /// <exception cref="DbUpdateException">
        ///     An error is encountered while saving to the database.
        /// </exception>
        /// <exception cref="DbUpdateConcurrencyException">
        ///     A concurrency violation is encountered while saving to the database.
        ///     A concurrency violation occurs when an unexpected number of rows are affected during save.
        ///     This is usually because the data in the database has been modified since it was loaded into memory.
        /// </exception>
        public override int SaveChanges()
        {
            this.UpdateAuditables();
            return base.SaveChanges();
        }

        /// <summary>
        ///     <para>
        ///         Saves all changes made in this context to the database.
        ///     </para>
        ///     <para>
        ///         This method will automatically call <see cref="M:Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker.DetectChanges" /> to discover any
        ///         changes to entity instances before saving to the underlying database. This can be disabled via
        ///         <see cref="P:Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker.AutoDetectChangesEnabled" />.
        ///     </para>
        /// </summary>
        /// <param name="acceptAllChangesOnSuccess">
        ///     Indicates whether <see cref="M:Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker.AcceptAllChanges" /> is called after the changes have
        ///     been sent successfully to the database.
        /// </param>
        /// <returns>
        ///     The number of state entries written to the database.
        /// </returns>
        /// <exception cref="DbUpdateException">
        ///     An error is encountered while saving to the database.
        /// </exception>
        /// <exception cref="DbUpdateConcurrencyException">
        ///     A concurrency violation is encountered while saving to the database.
        ///     A concurrency violation occurs when an unexpected number of rows are affected during save.
        ///     This is usually because the data in the database has been modified since it was loaded into memory.
        /// </exception>
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            this.UpdateAuditables();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        /// <summary>
        ///     <para>
        ///         Saves all changes made in this context to the database.
        ///     </para>
        ///     <para>
        ///         This method will automatically call <see cref="M:Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker.DetectChanges" /> to discover any
        ///         changes to entity instances before saving to the underlying database. This can be disabled via
        ///         <see cref="P:Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker.AutoDetectChangesEnabled" />.
        ///     </para>
        ///     <para>
        ///         Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
        ///         that any asynchronous operations have completed before calling another method on this context.
        ///     </para>
        /// </summary>
        /// <param name="acceptAllChangesOnSuccess">
        ///     Indicates whether <see cref="M:Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker.AcceptAllChanges" /> is called after the changes have
        ///     been sent successfully to the database.
        /// </param>
        /// <param name="cancellationToken"> A <see cref="CancellationToken" /> to observe while waiting for the task to complete. </param>
        /// <returns>
        ///     A task that represents the asynchronous save operation. The task result contains the
        ///     number of state entries written to the database.
        /// </returns>
        /// <exception cref="DbUpdateException">
        ///     An error is encountered while saving to the database.
        /// </exception>
        /// <exception cref="DbUpdateConcurrencyException">
        ///     A concurrency violation is encountered while saving to the database.
        ///     A concurrency violation occurs when an unexpected number of rows are affected during save.
        ///     This is usually because the data in the database has been modified since it was loaded into memory.
        /// </exception>
        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            this.UpdateAuditables();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        /// <summary>
        ///     <para>
        ///         Saves all changes made in this context to the database.
        ///     </para>
        ///     <para>
        ///         This method will automatically call <see cref="M:Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker.DetectChanges" /> to discover any
        ///         changes to entity instances before saving to the underlying database. This can be disabled via
        ///         <see cref="P:Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker.AutoDetectChangesEnabled" />.
        ///     </para>
        ///     <para>
        ///         Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
        ///         that any asynchronous operations have completed before calling another method on this context.
        ///     </para>
        /// </summary>
        /// <param name="cancellationToken"> A <see cref="CancellationToken" /> to observe while waiting for the task to complete. </param>
        /// <returns>
        ///     A task that represents the asynchronous save operation. The task result contains the
        ///     number of state entries written to the database.
        /// </returns>
        /// <exception cref="DbUpdateException">
        ///     An error is encountered while saving to the database.
        /// </exception>
        /// <exception cref="DbUpdateConcurrencyException">
        ///     A concurrency violation is encountered while saving to the database.
        ///     A concurrency violation occurs when an unexpected number of rows are affected during save.
        ///     This is usually because the data in the database has been modified since it was loaded into memory.
        /// </exception>
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            this.UpdateAuditables();
            return base.SaveChangesAsync(cancellationToken);
        }

        #endregion

        #region IUnitOfWork

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            var entites = this.ChangeTracker.Entries<IDomainEventEntity>().Select(e => e.Entity);
            await domainEventBus.DispatchDomainEventsAsync(entites).ConfigureAwait(false);

            await this.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return true;
        }

        #endregion

        #region Utilities

#pragma warning disable EF1001 // Internal EF Core API usage.
        /// <summary>
        /// Checks all the ValueObject properties and configures it as Owned
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected virtual void ConfigureAllValueObjectAsOwned(ModelBuilder modelBuilder)
        {
            //var navigationToValueObjects = modelBuilder.Model.GetEntityTypes()
            //    .SelectMany(et => et.GetNavigations()
            //        .Where(n => typeof(ValueObject).IsAssignableFrom(n.GetTargetType().ClrType)))
            //    .Select(n => n.ClrType)
            //    .Distinct()
            //    .ToList();

            //if (navigationToValueObjects.Any())
            //{
            //    foreach (Type valueObject in navigationToValueObjects)
            //    {
            //        modelBuilder.Owned(valueObject);
            //    }
            //}

            // Versión Entity Framework Core 3.x
            foreach (IMutableEntityType e in modelBuilder.Model.GetEntityTypes())
            {
                System.Collections.Generic.List<IMutableNavigation> navigationToValueObjects = e.GetNavigations().Where(n => typeof(ValueObject).IsAssignableFrom(n.GetTargetType().ClrType)).ToList();
                if (navigationToValueObjects.Any())
                {
                    EntityTypeBuilder builder = new EntityTypeBuilder(e);
                    foreach (IMutableNavigation ownedNavigation in navigationToValueObjects)
                    {
                        builder.OwnsOne(ownedNavigation.GetTargetType().ClrType, ownedNavigation.Name);
                        Console.WriteLine();
                    }
                }
            }
        }

        /// <summary>
        /// Updates properties in all IAuditble entities
        /// </summary>
        protected virtual void UpdateAuditables()
        {
            foreach (EntityEntry e in this.ChangeTracker.Entries().Where(e => e.State == EntityState.Added
                && typeof(IAuditable).IsAssignableFrom(e.Entity.GetType())))
            {
                e.Property(nameof(IAuditable.CreationDateTime)).CurrentValue = DateTime.Now;
                e.Property(nameof(IAuditable.UpdateDateTime)).CurrentValue = DateTime.Now;
                e.Property(nameof(IAuditable.CreationUser)).CurrentValue = this.CurrentUser;
                e.Property(nameof(IAuditable.UpdateUser)).CurrentValue = this.CurrentUser;
            }

            foreach (EntityEntry e in this.ChangeTracker.Entries().Where(e => e.State == EntityState.Modified
                && typeof(IAuditable).IsAssignableFrom(e.Entity.GetType())))
            {
                e.Property(nameof(IAuditable.UpdateDateTime)).CurrentValue = DateTime.Now;
                e.Property(nameof(IAuditable.UpdateUser)).CurrentValue = this.CurrentUser;
            }
        }

        #endregion
    }
}