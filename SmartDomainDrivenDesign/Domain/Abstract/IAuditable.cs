using System;

namespace SmartDomainDrivenDesign.Domain.Abstract
{
    /// <summary>
    /// Represents an entity that can be Auditable by user and date on creation and modification.
    /// </summary>
    public interface IAuditable
    {
        /// <summary>
        /// User who created the entity
        /// </summary>
        string CreationUser { get; set; }

        /// <summary>
        /// User who updated the entity
        /// </summary>
        string UpdateUser { get; set; }

        /// <summary>
        /// DateTime of creation
        /// </summary>
        DateTime CreationDateTime { get; set; }

        /// <summary>
        /// DateTime of update
        /// </summary>
        DateTime UpdateDateTime { get; set; }
    }
}
