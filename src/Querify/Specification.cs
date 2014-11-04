using System;
using System.Linq.Expressions;

namespace Querify
{
    public abstract class Specification<T> : ISpecification<T>
    {
        public abstract Expression<Func<T, bool>> IsSatisfiedBy();

        public static implicit operator Expression<Func<T, bool>>(Specification<T> f)
        {
            return f.IsSatisfiedBy();
        }
    }
}