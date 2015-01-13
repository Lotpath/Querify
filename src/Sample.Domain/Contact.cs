using System;
using System.Collections.Generic;

namespace Sample.Domain
{
    public class Contact
    {
        public Contact()
        {
            PhoneNumbers = new List<ContactPhone>();
        }

        public virtual Guid Id { get; protected set; }
        public virtual string Name { get; set; }

        public virtual IList<ContactPhone> PhoneNumbers { get; protected set; }
    }
}