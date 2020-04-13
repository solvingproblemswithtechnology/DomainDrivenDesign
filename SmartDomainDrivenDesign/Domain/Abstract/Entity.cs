using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartDomainDrivenDesign.Domain.Abstract
{
    /// <summary>
    /// Represents an Entity of the Domain
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Entity<T> where T : Entity<T>
    {
        /// <summary>
        /// Unique identifier
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual long Id { get; protected set; }

        /// <summary>
        /// Equals using the Id if it's an entity
        /// </summary>
        /// <param name="obj">Object to compare</param>
        public override bool Equals(object obj) => this.Equals(obj as Entity<T>);

        /// <summary>
        /// Equals using the Id
        /// </summary>
        /// <param name="other">Entity to compare</param>
        public bool Equals(Entity<T> other) => other != null && this.Id == other.Id;

        public override int GetHashCode() => 2108858624 + this.Id.GetHashCode();

        public static bool operator ==(Entity<T> left, Entity<T> right)
            => EqualityComparer<Entity<T>>.Default.Equals(left, right);

        public static bool operator !=(Entity<T> left, Entity<T> right) => !(left == right);
    }
}
