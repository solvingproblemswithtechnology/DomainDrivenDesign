using SmartDomainDrivenDesign.Domain.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartDomainDrivenDesign.OrderSample.Domain.Users
{
    public class User : Entity<User>
    {
        // TODO Name ValueObject
        public string Name { get; set; }
    }
}
