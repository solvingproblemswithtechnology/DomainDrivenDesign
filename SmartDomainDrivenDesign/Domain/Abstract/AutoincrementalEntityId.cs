using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SmartDomainDrivenDesign.Domain.Abstract
{
    public class AutoincrementalEntityId : EntityId
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; protected set; }

        private AutoincrementalEntityId() { }

        public AutoincrementalEntityId(long id)
        {
            this.Id = id;
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Id;
        }
    }
}
