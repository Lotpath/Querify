using System;
using System.Collections.Generic;

namespace Sample.Domain
{
    public class Parent
    {
        protected Parent() { }

        public Parent(Guid id)
        {
            Id = id;
        }

        public virtual Guid Id { get; protected set; }

        public virtual string Name { get; set; }

        public virtual Child Firstborn { get; protected set; }

        public virtual void SetFirstborn(Child child)
        {
            child.Id = Id;
            Firstborn = child;
        }
    }

    public class Child
    {
        public Child()
        {
            Grandkids = new List<Grandchild>();
        }

        public virtual Guid Id { get; protected internal set; }

        public virtual string Name { get; set; }

        public virtual IList<Grandchild> Grandkids { get; protected set; }
    }

    public class Grandchild
    {
        protected Grandchild() { }

        public Grandchild(Guid id)
        {
            Id = id;
        }

        public virtual Guid Id { get; protected set; }

        public virtual string Name { get; set; }
    }
}