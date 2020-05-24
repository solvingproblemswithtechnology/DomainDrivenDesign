using System;
using System.Collections.Generic;

namespace SmartDomainDrivenDesign.Domain.Abstract
{
    public class GuidEntityId : EntityId
    {
        public Guid Id { get; protected set; }

        private GuidEntityId() { }

        public GuidEntityId(Guid id)
        {
            this.Id = id;
        }

        public static GuidEntityId New() => new GuidEntityId(Guid.NewGuid());

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Id;
        }
    }
}
