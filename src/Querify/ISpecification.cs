using System;
using System.Linq.Expressions;

namespace Querify
{
    public interface ISpecification<T>
    {
        Expression<Func<T, bool>> IsSatisfiedBy();
    }
}