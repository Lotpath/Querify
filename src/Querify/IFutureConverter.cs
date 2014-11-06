using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Querify
{
    public interface IFutureConverter
    {
        bool CanHandle<T>(IQueryable<T> queryable);
        IEnumerable<T> ToFuture<T>(IQueryable<T> queryable);
        Lazy<T> ToFutureValue<T>(IQueryable<T> queryable);
        Lazy<TResult> ToFutureValue<TSource, TResult>(IQueryable<TSource> queryable, Expression<Func<IQueryable<TSource>, TResult>> selector);
    }
}