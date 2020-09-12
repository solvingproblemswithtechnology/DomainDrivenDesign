using System;
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
    public abstract class Entity<TEntity, TIdentifier> : IEquatable<Entity<TEntity, TIdentifier>>, IDomainEventEntity
        where TEntity : Entity<TEntity, TIdentifier>
        where TIdentifier : EntityId
    {
        /// <summary>
        /// Unique identifier
        /// </summary>
        [Key]
        public virtual TIdentifier Id { get; protected set; }

        #region Domain Events

        /// <summary>
        /// Real list of domain events
        /// </summary>
        [NotMapped]
        private List<IDomainEvent> domainEvents;

        /// <summary>
        /// Entity's DomainEvent
        /// </summary>
        [NotMapped]
        public IReadOnlyCollection<IDomainEvent> DomainEvents => domainEvents?.AsReadOnly();

        /// <summary>
        /// Adds an event to this entity domain events.
        /// </summary>
        /// <param name="eventItem"></param>
        public void AddDomainEvent(IDomainEvent domainEvent) => (this.domainEvents = domainEvents ?? new List<IDomainEvent>()).Add(domainEvent);

        /// <summary>
        /// Clears all the events in the entity.
        /// </summary>
        /// <param name="eventItem"></param>
        public void RemoveDomainEvent(IDomainEvent domainEvent) => this.domainEvents?.Remove(domainEvent);

        /// <summary>
        /// Clears all the events in the entity.
        /// </summary>
        /// <param name="eventItem"></param>
        public void ClearDomainEvents() => this.domainEvents?.Clear();

        #endregion

        #region Equals

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

        /// <summary>
        /// Override. Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode() => 2108858624 + this.Id.GetHashCode();

        /// <summary>
        /// Overriden for true equality (No reference equality)
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(Entity<TEntity, TIdentifier> left, Entity<TEntity, TIdentifier> right)
            => EqualityComparer<Entity<TEntity, TIdentifier>>.Default.Equals(left, right);

        /// <summary>
        /// Overriden for true equality (No reference equality)
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public static bool operator !=(Entity<TEntity, TIdentifier> left, Entity<TEntity, TIdentifier> right) => !(left == right);

        #endregion
    }
}
