using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartDomainDrivenDesign.Domain.Abstract
{
    /// <summary>
    /// Represents a ValueObject.
    /// </summary>
    public abstract class ValueObject : IEquatable<ValueObject>
    {
        /// <summary>
        /// Equal operator
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public static bool operator ==(ValueObject left, ValueObject right)
        {
            if (left is null ^ right is null) // Si uno de los dos es null, no son iguales
                return false;

            return left?.Equals(right) != false;
        }

        /// <summary>
        /// Equal operator
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public static bool operator !=(ValueObject left, ValueObject right) => !(left == right);

        /// <summary>
        /// Significant values for the Object
        /// </summary>
        /// <returns></returns>
        protected abstract IEnumerable<object> GetAtomicValues();

        /// <summary>
        /// Equals
        /// </summary>
        /// <param name="obj"></param>
        public override bool Equals(object obj) => obj is ValueObject other && this.Equals(other);

        /// <summary>
        /// Equals
        /// </summary>
        /// <param name="other"></param>
        public bool Equals(ValueObject other)
        {
            if (other == null || other.GetType() != this.GetType())
                return false;

            IEnumerator<object> thisValues = this.GetAtomicValues().GetEnumerator();
            IEnumerator<object> otherValues = other.GetAtomicValues().GetEnumerator();

            while (thisValues.MoveNext() && otherValues.MoveNext())
            {
                if (thisValues.Current is null ^ otherValues.Current is null)
                    return false;

                if (thisValues.Current?.Equals(otherValues.Current) == false)
                    return false;
            }

            return !thisValues.MoveNext() && !otherValues.MoveNext();
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode() => this.GetAtomicValues().Select(x => x?.GetHashCode() ?? 0).Aggregate((x, y) => x ^ y);

        /// <summary>
        /// Default implementation using the significant values.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString() => string.Join(", ", this.GetAtomicValues());
    }
}
