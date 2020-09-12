using SmartDomainDrivenDesign.Domain.Abstract;

namespace SmartDomainDrivenDesign.OrderSample.Domain.Users
{
    public class BuyerId : GuidEntityId
    {
        public BuyerId(System.Guid id) : base(id) { }
    }

    public class Buyer : Entity<Buyer, BuyerId>
    {
        // TODO Name ValueObject
        public string Name { get; }

        public Buyer(string name)
        {
            this.Name = name;
        }
    }
}
