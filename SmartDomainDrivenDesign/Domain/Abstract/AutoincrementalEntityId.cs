using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartDomainDrivenDesign.Domain.Abstract
{
    public class AutoIncrementalEntityId : EntityId
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; protected set; }

        private AutoIncrementalEntityId() { }

        public AutoIncrementalEntityId(long id)
        {
            this.Id = id;
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Id;
        }
    }
}
