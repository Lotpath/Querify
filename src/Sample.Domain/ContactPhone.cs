using System;

namespace Sample.Domain
{
    public class ContactPhone
    {
        public virtual Guid Id { get; protected set; }
        public virtual string Type { get; set; }
        public virtual string Number { get; set; }
    }
}
