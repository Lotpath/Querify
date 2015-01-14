using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Linq;
using Remotion.Linq;

namespace Querify
{
    internal class NHibernateFutureConverter : IFutureConverter
    {
        public bool CanHandle<T>(IQueryable<T> queryable)
        {
            return queryable is QueryableBase<T>;
        }

        public IEnumerable<T> ToFuture<T>(IQueryable<T> queryable)
        {
            return queryable.ToFuture();
        }

        public Lazy<T> ToFutureValue<T>(IQueryable<T> queryable)
        {
            return new Lazy<T>(() => queryable.ToFutureValue().Value);
        }

        public Lazy<TResult> ToFutureValue<TSource, TResult>(IQueryable<TSource> queryable, Expression<Func<IQueryable<TSource>, TResult>> selector)
        {
            return new Lazy<TResult>(() => queryable.ToFutureValue(selector).Value);
        }
    }
}