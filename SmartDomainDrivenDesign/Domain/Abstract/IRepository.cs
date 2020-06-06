using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SmartDomainDrivenDesign.Domain.Abstract
{
    public interface IRepository<TEntity> : IRepository<TEntity, GuidEntityId>
        where TEntity : Entity<TEntity>, IAggregateRoot
    {
    }

    public interface IRepository<TEntity, TIdentifier>
        where TEntity : Entity<TEntity, TIdentifier>, IAggregateRoot
        where TIdentifier : EntityId
    {
        IUnitOfWork UnitOfWork { get; }

        ValueTask<TEntity> FindAsync(TIdentifier identifier);

        void Add(TEntity entity);
        void AddRange(IEnumerable<TEntity> entities);

        void Update(TEntity entity);
        void UpdateRange(IEnumerable<TEntity> entities);

        void Delete(TEntity entity);
        void DeleteRange(IEnumerable<TEntity> entities);

        IQueryable<TEntity> GetBy(Expression<Func<TEntity, bool>> filter);
    }
}
