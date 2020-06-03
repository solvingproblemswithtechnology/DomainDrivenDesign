using SmartDomainDrivenDesign.Domain.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SmartDomainDrivenDesign.Infrastructure.EntityFrameworkCore
{
    public class EntityFrameworkRepository<TEntity> : EntityFrameworkRepository<TEntity, GuidEntityId>, IRepository<TEntity>
        where TEntity : Entity<TEntity>, IAggregateRoot
    {
        public EntityFrameworkRepository(SmartDbContext context) : base(context)
        {
        }
    }

    public class EntityFrameworkRepository<TEntity, TIdentifier> : IRepository<TEntity, TIdentifier>
        where TEntity : Entity<TEntity, TIdentifier>, IAggregateRoot
        where TIdentifier : EntityId
    {
        private readonly SmartDbContext context;

        public EntityFrameworkRepository(SmartDbContext context)
        {
            this.context = context;
        }

        public ValueTask<TEntity> FindAsync(TIdentifier identifier) => this.context.FindAsync<TEntity>();

        public void Add(TEntity entity) => this.context.Set<TEntity>().Add(entity);
        public void AddRange(IEnumerable<TEntity> entities) => this.context.Set<TEntity>().AddRange(entities);

        public void Update(TEntity entity) => this.context.Set<TEntity>().Update(entity);
        public void UpdateRange(IEnumerable<TEntity> entities) => this.context.Set<TEntity>().UpdateRange(entities);

        public void Delete(TEntity entity) => this.context.Set<TEntity>().Remove(entity);
        public void DeleteRange(IEnumerable<TEntity> entities) => this.context.Set<TEntity>().RemoveRange(entities);

        public IQueryable<TEntity> GetBy(Expression<Func<TEntity, bool>> filter) => this.context.Set<TEntity>().Where(filter);
    }
}
