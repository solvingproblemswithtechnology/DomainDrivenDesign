using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartDomainDrivenDesign.Domain.Abstract
{
    /// <summary>
    /// Represents an Entity of the Domain with a Guid as Id.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class Entity<TEntity> : Entity<TEntity, GuidEntityId> where TEntity : Entity<TEntity>
    {
    }

    /// <summary>
    /// Represents an Entity of the Domain
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class Entity<TEntity, TIdentifier> where TEntity : Entity<TEntity, TIdentifier> where TIdentifier : EntityId
    {
        /// <summary>
        /// Unique identifier
        /// </summary>
        [Key]
        public virtual TIdentifier Id { get; protected set; }

        /// <summary>
        /// Equals using the Id if it's an entity
        /// </summary>
        /// <param name="obj">Object to compare</param>
        public override bool Equals(object obj) => this.Equals(obj as Entity<TEntity, TIdentifier>);

        /// <summary>
        /// Equals using the Id
        /// </summary>
        /// <param name="other">Entity to compare</param>
        public bool Equals(Entity<TEntity, TIdentifier> other) => other != null && this.Id == other.Id;

        public override int GetHashCode() => 2108858624 + this.Id.GetHashCode();

        public static bool operator ==(Entity<TEntity, TIdentifier> left, Entity<TEntity, TIdentifier> right)
            => EqualityComparer<Entity<TEntity, TIdentifier>>.Default.Equals(left, right);

        public static bool operator !=(Entity<TEntity, TIdentifier> left, Entity<TEntity, TIdentifier> right) => !(left == right);
    }
}
