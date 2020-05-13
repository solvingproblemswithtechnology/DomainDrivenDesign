using SmartDomainDrivenDesign.Domain.Abstract;

namespace SmartDomainDrivenDesign.OrderSample.Domain.Users
{
    public class User : Entity<User>
    {
        // TODO Name ValueObject
        public string Name { get; }

        public User(string name)
        {
            this.Name = name;
        }
    }
}
